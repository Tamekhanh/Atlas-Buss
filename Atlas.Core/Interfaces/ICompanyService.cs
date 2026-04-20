using Atlas.Core.Entities;
using Atlas.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace Atlas.Core.Interfaces
{
    public interface ICompanyService
    {
        Task<IEnumerable<Companies>> GetAllAsync();
        Task<Companies?> GetByIdAsync(int id);
        Task<bool> AddAsync(Companies companies);
        Task<bool> UpdateAsync(Companies companies);
        Task<bool> DeleteAsync(int id);
    }

    public interface IVendorCompanyService
    {
        Task<IEnumerable<VendorCompany>> GetAllAsync();
        Task<bool> CreateAsync(CompanyRegistrationRequest request);
    }

    public interface ICustomerCompanyService
    {
        Task<IEnumerable<CustomerCompany>> GetAllAsync();
        Task<bool> CreateAsync(CompanyRegistrationRequest request);
    }

    public interface IVendorPersonService
    {
        Task<IEnumerable<VendorPerson>> GetAllAsync();
        Task<bool> CreateAsync(PersonRegistrationRequest request);
    }

    public interface ICustomerPersonService
    {
        Task<IEnumerable<CustomerPerson>> GetAllAsync();
        Task<bool> CreateAsync(PersonRegistrationRequest request);
    }
}
