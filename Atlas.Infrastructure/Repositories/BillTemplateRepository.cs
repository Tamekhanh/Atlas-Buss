using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.Infrastructure.Repositories
{
    public class BillTemplateRepository : IBillTemplateRepository
    {
        private readonly AtlasDBContext _context;

        public BillTemplateRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<BillTemplates>> GetAllAsync()
        {
            return await _context.BillTemplates
                .AsNoTracking()
                .Where(t => !t.IsDeleted)
                .OrderByDescending(t => t.IsDefault)
                .ThenBy(t => t.TemplateName)
                .ToListAsync();
        }

        public async Task<BillTemplates?> GetByIdAsync(int id)
        {
            return await _context.BillTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted);
        }

        public async Task<BillTemplates?> GetDefaultAsync()
        {
            return await _context.BillTemplates
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.IsDefault && !t.IsDeleted);
        }

        public async Task<bool> AddAsync(BillTemplates template)
        {
            await _context.BillTemplates.AddAsync(template);
            if (template.IsDefault)
            {
                await ClearDefaultInternalAsync(template);
            }
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(BillTemplates template)
        {
            var existing = await _context.BillTemplates.FindAsync(template.Id);
            if (existing == null) return false;

            existing.TemplateName = template.TemplateName;
            existing.Description = template.Description;
            existing.PageSize = template.PageSize;
            existing.Orientation = template.Orientation;
            existing.OptionsJson = template.OptionsJson;
            existing.HeaderNote = template.HeaderNote;
            existing.FooterNote = template.FooterNote;

            if (template.IsDefault && !existing.IsDefault)
            {
                await ClearDefaultInternalAsync(existing);
                existing.IsDefault = true;
            }
            else if (!template.IsDefault && existing.IsDefault)
            {
                existing.IsDefault = false;
            }

            existing.UpdatedAt = DateTime.Now;
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var template = await _context.BillTemplates.FindAsync(id);
            if (template == null) return false;

            // Xóa mềm
            template.IsDeleted = true;
            template.UpdatedAt = DateTime.Now;
            return await _context.SaveChangesAsync() > 0;
        }

        // Gỡ cờ IsDefault của mọi mẫu khác (dùng trong Add/Update). Không gọi SaveChanges.
        private async Task ClearDefaultInternalAsync(BillTemplates keepExcluded)
        {
            var others = await _context.BillTemplates
                .Where(t => t.IsDefault && t.Id != keepExcluded.Id && !t.IsDeleted)
                .ToListAsync();

            foreach (var other in others)
            {
                other.IsDefault = false;
            }
        }

        public async Task<bool> ClearDefaultAsync()
        {
            var defaults = await _context.BillTemplates
                .Where(t => t.IsDefault && !t.IsDeleted)
                .ToListAsync();

            foreach (var item in defaults)
            {
                item.IsDefault = false;
            }

            return await _context.SaveChangesAsync() > 0;
        }
    }
}
