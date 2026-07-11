using System.Collections.Generic;
using System.Threading.Tasks;
using Atlas.Core.Entities;
using Atlas.Core.Interfaces;

namespace Atlas.Services.Attributes
{
    public class AttributeService : IAttributeService
    {
        private readonly IAttributeRepository _attributeRepository;

        public AttributeService(IAttributeRepository attributeRepository)
        {
            _attributeRepository = attributeRepository;
        }

        // =============================================
        // 1. LOẠI THUỘC TÍNH (AttributeType)
        // =============================================

        public async Task<IEnumerable<AttributeType>> GetAllAttributeTypesAsync()
        {
            return await _attributeRepository.GetAllAttributeTypesAsync();
        }

        public async Task<AttributeType?> GetAttributeTypeByIdAsync(int id)
        {
            return await _attributeRepository.GetAttributeTypeByIdAsync(id);
        }

        public async Task<AttributeType?> FindAttributeTypeByNameAsync(string attributeName)
        {
            return await _attributeRepository.FindAttributeTypeByNameAsync(attributeName);
        }

        public async Task<bool> CreateAttributeTypeAsync(AttributeType attributeType)
        {
            return await _attributeRepository.AddAttributeTypeAsync(attributeType);
        }

        public async Task<bool> UpdateAttributeTypeAsync(AttributeType attributeType)
        {
            return await _attributeRepository.UpdateAttributeTypeAsync(attributeType);
        }

        public async Task<bool> DeleteAttributeTypeAsync(int id)
        {
            return await _attributeRepository.DeleteAttributeTypeAsync(id);
        }

        // =============================================
        // 2. GIÁ TRỊ THUỘC TÍNH (AttributeValue)
        // =============================================

        public async Task<AttributeValue?> GetAttributeValueByIdAsync(int id)
        {
            return await _attributeRepository.GetAttributeValueByIdAsync(id);
        }

        public async Task<bool> CreateAttributeValueAsync(AttributeValue attributeValue)
        {
            return await _attributeRepository.AddAttributeValueAsync(attributeValue);
        }

        public async Task<bool> UpdateAttributeValueAsync(AttributeValue attributeValue)
        {
            return await _attributeRepository.UpdateAttributeValueAsync(attributeValue);
        }

        public async Task<bool> DeleteAttributeValueAsync(int id)
        {
            return await _attributeRepository.DeleteAttributeValueAsync(id);
        }
    }
}
