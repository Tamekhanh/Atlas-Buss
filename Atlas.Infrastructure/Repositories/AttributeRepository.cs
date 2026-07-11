using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.Infrastructure.Repositories
{
    public class AttributeRepository : IAttributeRepository
    {
        private readonly AtlasDBContext _context;

        public AttributeRepository(AtlasDBContext context)
        {
            _context = context;
        }

        // =============================================
        // 1. LOẠI THUỘC TÍNH (AttributeType)
        // =============================================

        public async Task<IEnumerable<AttributeType>> GetAllAttributeTypesAsync()
        {
            return await _context.AttributeTypes
                .Include(type => type.Values)
                .AsNoTracking()
                .OrderBy(type => type.Id)
                .ToListAsync();
        }

        public async Task<AttributeType?> GetAttributeTypeByIdAsync(int id)
        {
            return await _context.AttributeTypes
                .Include(type => type.Values)
                .AsNoTracking()
                .FirstOrDefaultAsync(type => type.Id == id);
        }

        public async Task<AttributeType?> FindAttributeTypeByNameAsync(string attributeName)
        {
            if (string.IsNullOrWhiteSpace(attributeName))
            {
                return null;
            }

            var trimmed = attributeName.Trim();
            return await _context.AttributeTypes
                .AsNoTracking()
                .FirstOrDefaultAsync(type => type.AttributeName == trimmed);
        }

        public async Task<bool> AddAttributeTypeAsync(AttributeType attributeType)
        {
            await _context.AttributeTypes.AddAsync(attributeType);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAttributeTypeAsync(AttributeType attributeType)
        {
            var existing = await _context.AttributeTypes
                .FirstOrDefaultAsync(type => type.Id == attributeType.Id);

            if (existing is null)
            {
                return false;
            }

            existing.AttributeName = attributeType.AttributeName;
            existing.Description = attributeType.Description;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAttributeTypeAsync(int id)
        {
            var existing = await _context.AttributeTypes
                .Include(type => type.Values)
                .FirstOrDefaultAsync(type => type.Id == id);

            if (existing is null)
            {
                return false;
            }

            // Nêu có giá trị thuộc tính đang được dùng bởi biến thể -> chặn xóa để giữ toàn vẹn dữ liệu.
            var valueIds = existing.Values.Select(value => value.Id).ToList();
            var hasUsages = valueIds.Count > 0 && await _context.VariantAttributeMappings
                .AnyAsync(mapping => valueIds.Contains(mapping.AttributeValueId));

            if (hasUsages)
            {
                return false;
            }

            _context.AttributeValues.RemoveRange(existing.Values);
            _context.AttributeTypes.Remove(existing);

            return await _context.SaveChangesAsync() > 0;
        }

        // =============================================
        // 2. GIÁ TRỊ THUỘC TÍNH (AttributeValue)
        // =============================================

        public async Task<AttributeValue?> GetAttributeValueByIdAsync(int id)
        {
            return await _context.AttributeValues
                .AsNoTracking()
                .FirstOrDefaultAsync(value => value.Id == id);
        }

        public async Task<AttributeValue?> FindAttributeValueAsync(int attributeTypeId, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            var trimmed = value.Trim();
            return await _context.AttributeValues
                .AsNoTracking()
                .FirstOrDefaultAsync(item => item.AttributeTypeId == attributeTypeId && item.Value == trimmed);
        }

        public async Task<bool> AddAttributeValueAsync(AttributeValue attributeValue)
        {
            await _context.AttributeValues.AddAsync(attributeValue);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAttributeValueAsync(AttributeValue attributeValue)
        {
            var existing = await _context.AttributeValues
                .FirstOrDefaultAsync(value => value.Id == attributeValue.Id);

            if (existing is null)
            {
                return false;
            }

            existing.Value = attributeValue.Value;
            existing.AttributeTypeId = attributeValue.AttributeTypeId;

            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAttributeValueAsync(int id)
        {
            var existing = await _context.AttributeValues
                .FirstOrDefaultAsync(value => value.Id == id);

            if (existing is null)
            {
                return false;
            }

            // Nếu đang được dùng bởi biến thể -> chặn xóa.
            var hasUsages = await _context.VariantAttributeMappings
                .AnyAsync(mapping => mapping.AttributeValueId == id);

            if (hasUsages)
            {
                return false;
            }

            _context.AttributeValues.Remove(existing);

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
