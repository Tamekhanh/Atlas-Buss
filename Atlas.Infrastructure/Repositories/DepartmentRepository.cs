using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Atlas.Infrastructure.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly AtlasDBContext _context;

        public DepartmentRepository(AtlasDBContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllAsync()
        {
            return await _context.Departments
                .Include(d => d.ParentDepartment)
                .Include(d => d.ChildDepartments)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Department> GetByIdAsync(int id)
        {
            return await _context.Departments
                .Include(d => d.ParentDepartment)
                .Include(d => d.ChildDepartments)
                .Include(d => d.EmployeeDepartments)
                .AsNoTracking()
                .FirstOrDefaultAsync(d => d.Id == id);
        }

        public async Task<IEnumerable<Department>> GetByParentIdAsync(int? parentId)
        {
            return await _context.Departments
                .Where(d => d.ParentDepartmentId == parentId)
                .Include(d => d.ChildDepartments)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> AddAsync(Department department)
        {
            await _context.Departments.AddAsync(department);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> UpdateAsync(Department department)
        {
            _context.Departments.Update(department);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department is null)
            {
                return false;
            }

            _context.Departments.Remove(department);
            return await _context.SaveChangesAsync() > 0;
        }
    }
}
