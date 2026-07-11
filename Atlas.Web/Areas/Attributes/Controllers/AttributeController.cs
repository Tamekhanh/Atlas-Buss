using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Atlas.Web.Areas.Attributes.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Atlas.Web.Areas.Attributes.Controllers
{
    [Area("Attributes")]
    [Authorize]
    public class AttributeController : Controller
    {
        private readonly IAttributeService _attributeService;
        private readonly ILogService _logService;

        public AttributeController(IAttributeService attributeService, ILogService logService)
        {
            _attributeService = attributeService;
            _logService = logService;
        }

        // GET: /Attributes/Index
        [Authorize(Policy = "AttributeView")]
        public async Task<IActionResult> Index()
        {
            var types = await _attributeService.GetAllAttributeTypesAsync();
            var model = types.Select(t => new AttributeTypeViewModel
            {
                Id = t.Id,
                AttributeName = t.AttributeName,
                Description = t.Description,
                Values = (t.Values ?? new List<AttributeValue>())
                    .Select(v => new AttributeValueViewModel
                    {
                        Id = v.Id,
                        Value = v.Value,
                        AttributeTypeId = t.Id
                    })
                    .ToList()
            }).ToList();

            return View("~/Areas/Attributes/Views/Attribute/Index.cshtml", model);
        }

        // GET: /Attributes/Create
        [Authorize(Policy = "AttributeManage")]
        public IActionResult Create()
        {
            return View("~/Areas/Attributes/Views/Attribute/Create.cshtml", new AttributeTypeViewModel());
        }

        // POST: /Attributes/Create
        [HttpPost]
        [Authorize(Policy = "AttributeManage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AttributeTypeViewModel model)
        {
            // Loại bỏ các dòng value rỗng
            model.Values = (model.Values ?? new List<AttributeValueViewModel>())
                .Where(v => !string.IsNullOrWhiteSpace(v.Value))
                .ToList();

            if (!ModelState.IsValid)
            {
                return View("~/Areas/Attributes/Views/Attribute/Create.cshtml", model);
            }

            var existing = await _attributeService.FindAttributeTypeByNameAsync(model.AttributeName);
            if (existing is not null)
            {
                ModelState.AddModelError(nameof(model.AttributeName), "An attribute with this name already exists.");
                return View("~/Areas/Attributes/Views/Attribute/Create.cshtml", model);
            }

            var attributeType = new AttributeType
            {
                AttributeName = model.AttributeName.Trim(),
                Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim(),
                Values = model.Values
                    .Select(v => new AttributeValue { Value = v.Value.Trim() })
                    .ToList()
            };

            var created = await _attributeService.CreateAttributeTypeAsync(attributeType);
            if (!created)
            {
                ModelState.AddModelError(string.Empty, "Could not create attribute.");
                return View("~/Areas/Attributes/Views/Attribute/Create.cshtml", model);
            }

            await LogAsync($"Created attribute: {attributeType.AttributeName} (ID: {attributeType.Id})");
            return RedirectToAction(nameof(Index));
        }

        // GET: /Attributes/Edit/{id}
        [Authorize(Policy = "AttributeManage")]
        public async Task<IActionResult> Edit(int id)
        {
            var type = await _attributeService.GetAttributeTypeByIdAsync(id);
            if (type is null)
            {
                return NotFound();
            }

            var model = new AttributeTypeViewModel
            {
                Id = type.Id,
                AttributeName = type.AttributeName,
                Description = type.Description,
                Values = (type.Values ?? new List<AttributeValue>())
                    .Select(v => new AttributeValueViewModel
                    {
                        Id = v.Id,
                        Value = v.Value,
                        AttributeTypeId = type.Id
                    })
                    .ToList()
            };

            return View("~/Areas/Attributes/Views/Attribute/Edit.cshtml", model);
        }

        // POST: /Attributes/Edit/{id}
        [HttpPost]
        [Authorize(Policy = "AttributeManage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, AttributeTypeViewModel model)
        {
            model.Values = (model.Values ?? new List<AttributeValueViewModel>())
                .Where(v => !string.IsNullOrWhiteSpace(v.Value))
                .ToList();

            if (id != model.Id || !ModelState.IsValid)
            {
                return View("~/Areas/Attributes/Views/Attribute/Edit.cshtml", model);
            }

            var updated = await _attributeService.UpdateAttributeTypeAsync(new AttributeType
            {
                Id = model.Id,
                AttributeName = model.AttributeName.Trim(),
                Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim()
            });

            if (!updated)
            {
                ModelState.AddModelError(string.Empty, "Could not update attribute.");
                return View("~/Areas/Attributes/Views/Attribute/Edit.cshtml", model);
            }

            await LogAsync($"Updated attribute: {model.AttributeName} (ID: {model.Id})");
            return RedirectToAction(nameof(Index));
        }

        // POST: /Attributes/Delete/{id}
        [HttpPost]
        [Authorize(Policy = "AttributeManage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _attributeService.DeleteAttributeTypeAsync(id);
            if (!deleted)
            {
                TempData["Error"] = "Could not delete this attribute. It may be in use by one or more product variants.";
            }
            else
            {
                await LogAsync($"Deleted attribute ID: {id}");
            }

            return RedirectToAction(nameof(Index));
        }

        // --- Inline value management (used on the management page) ---

        // POST: /Attributes/CreateValue  (JSON, dùng cho modal tạo nhanh)
        [HttpPost]
        [Authorize(Policy = "AttributeManage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateValue([FromBody] AttributeValueViewModel model)
        {
            if (model is null || string.IsNullOrWhiteSpace(model.Value) || model.AttributeTypeId <= 0)
            {
                return BadRequest(new { success = false, message = "Value and attribute are required." });
            }

            var attributeType = await _attributeService.GetAttributeTypeByIdAsync(model.AttributeTypeId);
            if (attributeType is null)
            {
                return NotFound(new { success = false, message = "Attribute not found." });
            }

            var value = new AttributeValue
            {
                AttributeTypeId = model.AttributeTypeId,
                Value = model.Value.Trim()
            };

            var created = await _attributeService.CreateAttributeValueAsync(value);
            if (!created)
            {
                return BadRequest(new { success = false, message = "Could not create value." });
            }

            return Json(new
            {
                success = true,
                id = value.Id,
                value = value.Value,
                attributeTypeId = value.AttributeTypeId,
                attributeName = attributeType.AttributeName
            });
        }

        // POST: /Attributes/DeleteValue/{id}
        [HttpPost]
        [Authorize(Policy = "AttributeManage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteValue(int id)
        {
            var deleted = await _attributeService.DeleteAttributeValueAsync(id);
            if (!deleted)
            {
                return BadRequest(new { success = false, message = "Could not delete this value. It may be in use by one or more product variants." });
            }

            await LogAsync($"Deleted attribute value ID: {id}");
            return Json(new { success = true });
        }

        // --- Endpoints dùng cho "tạo nhanh" trên trang tạo/sửa sản phẩm ---

        // POST: /Attributes/QuickCreateType  (JSON)
        [HttpPost]
        [Authorize(Policy = "AttributeManage")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> QuickCreateType([FromBody] AttributeTypeViewModel model)
        {
            if (model is null || string.IsNullOrWhiteSpace(model.AttributeName))
            {
                return BadRequest(new { success = false, message = "Attribute name is required." });
            }

            var trimmedName = model.AttributeName.Trim();
            var existing = await _attributeService.FindAttributeTypeByNameAsync(trimmedName);
            if (existing is not null)
            {
                return Ok(new
                {
                    success = true,
                    id = existing.Id,
                    attributeName = existing.AttributeName,
                    values = (existing.Values ?? new List<AttributeValue>())
                        .Select(v => new { id = v.Id, value = v.Value })
                });
            }

            var attributeType = new AttributeType
            {
                AttributeName = trimmedName,
                Description = string.IsNullOrWhiteSpace(model.Description) ? null : model.Description.Trim()
            };

            var created = await _attributeService.CreateAttributeTypeAsync(attributeType);
            if (!created)
            {
                return BadRequest(new { success = false, message = "Could not create attribute." });
            }

            await LogAsync($"Quick-created attribute: {attributeType.AttributeName} (ID: {attributeType.Id})");

            return Json(new
            {
                success = true,
                id = attributeType.Id,
                attributeName = attributeType.AttributeName,
                values = Array.Empty<object>()
            });
        }

        private async Task LogAsync(string message)
        {
            var employeeIdValue = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(employeeIdValue, out var employeeId))
            {
                await _logService.AddLogAsync(employeeId, message);
            }
        }
    }
}
