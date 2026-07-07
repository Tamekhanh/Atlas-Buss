using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.Services
{
    public class PurchaseOrderService : IPurchaseOrderService
    {
        private readonly IPurchaseOrderRepository _poRepository;
        private readonly IPartyRepository _partyRepository; // Cần để kiểm tra Vendor
        private readonly IProductRepository _productRepository;

        public PurchaseOrderService(IPurchaseOrderRepository poRepository, IPartyRepository partyRepository, IProductRepository productRepository)
        {
            _poRepository = poRepository;
            _partyRepository = partyRepository;
            _productRepository = productRepository;
        }

        public async Task<IEnumerable<PurchaseOrder>> GetAllAsync()
        {
            return await _poRepository.GetAllAsync();
        }

        public async Task<PurchaseOrder?> GetByIdAsync(int id)
        {
            if (id <= 0) return null;
            return await _poRepository.GetByIdAsync(id);
        }

        public async Task<PurchaseOrder?> GetByPONumberAsync(string poNumber)
        {
            if (string.IsNullOrWhiteSpace(poNumber)) return null;
            return await _poRepository.GetByPONumberAsync(poNumber);
        }

        public async Task<bool> CreateAsync(PurchaseOrder order)
        {
            if (order == null) return false;

            // 1. Kiểm tra mã PO không được để trống
            if (string.IsNullOrWhiteSpace(order.PONumber))
            {
                return false;
            }

            // 2. Kiểm tra mã PO có bị trùng trong hệ thống không
            var existingPo = await _poRepository.GetByPONumberAsync(order.PONumber);
            if (existingPo != null)
            {
                // Bạn có thể throw Exception hoặc return false tùy theo cách xử lý ở Controller
                return false; 
            }

            // 3. Kiểm tra VendorId có tồn tại và có phải là Vendor không
            var vendor = await _partyRepository.GetByIdAsync(order.VendorId);
            if (vendor == null || !vendor.IsVendor)
            {
                return false; // Vendor không tồn tại hoặc không phải là nhà cung cấp
            }

            // 4. Kiểm tra đơn hàng phải có ít nhất một sản phẩm chi tiết
            if (order.PurchaseOrderDetails == null || !order.PurchaseOrderDetails.Any())
            {
                return false;
            }

            if (!await ValidateAndHydrateDetailsAsync(order))
            {
                return false;
            }

            return await _poRepository.AddAsync(order);
        }

        public async Task<bool> UpdateAsync(PurchaseOrder order)
        {
            if (order == null || order.Id <= 0)
            {
                return false;
            }

            // Kiểm tra nếu thay đổi PONumber thì không được trùng với PO khác
            var existingPo = await _poRepository.GetByPONumberAsync(order.PONumber);
            if (existingPo != null && existingPo.Id != order.Id)
            {
                return false;
            }

            if (order.PurchaseOrderDetails != null && order.PurchaseOrderDetails.Any())
            {
                if (!await ValidateAndHydrateDetailsAsync(order))
                {
                    return false;
                }
            }

            return await _poRepository.UpdateAsync(order);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            if (id <= 0)
            {
                return false;
            }

            // Nghiệp vụ: Không cho phép xóa PO nếu trạng thái đã là "Đã nhận hàng" (ví dụ StatusId = 2)
            var order = await _poRepository.GetByIdAsync(id);
            if (order != null && order.OrderStatusId == 2) 
            {
                return false; 
            }

            return await _poRepository.DeleteAsync(id);
        }

        public async Task<bool> UpdateStatusAsync(int id, int newStatusId)
        {
            if (id <= 0) return false;

            var order = await _poRepository.GetByIdAsync(id);
            if (order == null) return false;

            order.OrderStatusId = newStatusId;

            // LOGIC MỞ RỘNG:
            // Nếu trạng thái chuyển sang "Received" (Đã nhận hàng), 
            // tại đây bạn sẽ gọi InventoryService để cộng kho.
            if (newStatusId == 2) 
            {
                // await _inventoryService.ReceiveGoodsAsync(order.PurchaseOrderDetails);
            }

            return await _poRepository.UpdateAsync(order);
        }

        private async Task<bool> ValidateAndHydrateDetailsAsync(PurchaseOrder order)
        {
            foreach (var item in order.PurchaseOrderDetails)
            {
                if (item.VariantId <= 0) return false;
                if (item.WarehouseId <= 0) return false;
                if (item.Quantity <= 0) return false;
                if (item.UnitPrice < 0) return false;

                var variant = await _productRepository.GetVariantByIdAsync(item.VariantId);
                if (variant == null)
                {
                    return false;
                }

                item.ProductId = variant.ProductId;
            }

            return true;
        }
    }
}