using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Core.Interfaces
{
    // Quản lý Thuộc tính (Attributes) tách riêng khỏi ProductRepository
    // để hỗ trợ trang quản trị thuộc tính + tạo nhanh trong lúc chọn biến thể.
    public interface IAttributeRepository
    {
        // --- Loại thuộc tính (AttributeType: Màu sắc, Kích thước...) ---
        Task<IEnumerable<AttributeType>> GetAllAttributeTypesAsync();
        Task<AttributeType?> GetAttributeTypeByIdAsync(int id);
        Task<AttributeType?> FindAttributeTypeByNameAsync(string attributeName);
        Task<bool> AddAttributeTypeAsync(AttributeType attributeType);
        Task<bool> UpdateAttributeTypeAsync(AttributeType attributeType);
        Task<bool> DeleteAttributeTypeAsync(int id);

        // --- Giá trị thuộc tính (AttributeValue: Đỏ, Xanh, L, XL...) ---
        Task<AttributeValue?> GetAttributeValueByIdAsync(int id);
        Task<AttributeValue?> FindAttributeValueAsync(int attributeTypeId, string value);
        Task<bool> AddAttributeValueAsync(AttributeValue attributeValue);
        Task<bool> UpdateAttributeValueAsync(AttributeValue attributeValue);
        Task<bool> DeleteAttributeValueAsync(int id);
    }
}
