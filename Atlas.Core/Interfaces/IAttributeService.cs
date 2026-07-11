using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Core.Interfaces
{
    // Service cho module Thuộc tính (Attributes) dùng cho trang quản lý
    // và cho việc tạo nhanh thuộc tính trên trang tạo/sửa sản phẩm.
    public interface IAttributeService
    {
        // --- Loại thuộc tính (AttributeType) ---
        Task<IEnumerable<AttributeType>> GetAllAttributeTypesAsync();
        Task<AttributeType?> GetAttributeTypeByIdAsync(int id);
        Task<AttributeType?> FindAttributeTypeByNameAsync(string attributeName);
        Task<bool> CreateAttributeTypeAsync(AttributeType attributeType);
        Task<bool> UpdateAttributeTypeAsync(AttributeType attributeType);
        Task<bool> DeleteAttributeTypeAsync(int id);

        // --- Giá trị thuộc tính (AttributeValue) ---
        Task<AttributeValue?> GetAttributeValueByIdAsync(int id);
        Task<bool> CreateAttributeValueAsync(AttributeValue attributeValue);
        Task<bool> UpdateAttributeValueAsync(AttributeValue attributeValue);
        Task<bool> DeleteAttributeValueAsync(int id);
    }
}
