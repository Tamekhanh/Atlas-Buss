using System;
using System.Collections.Generic;
using System.Text;
using Atlas.Core.Entities;
using Atlas.Core.Models;

namespace Atlas.Core.Interfaces
{
    public interface ICompanyRepository
    {
        Task<IEnumerable<Companies>> GetAllCompaniesAsync();
        Task<Companies?> GetCompanyByIdAsync(int id);
        Task<bool> AddCompanyAsync(Companies company);
        Task<bool> UpdateCompanyAsync(Companies company);
        Task<bool> DeleteCompanyAsync(int id);
    }

    public interface IVendorCompanyRepository
    {
        Task<IEnumerable<VendorCompany>> GetAllVendorCompaniesAsync();
        Task<bool> CreateVendorCompanyAsync(CompanyRegistrationRequest request);
    }

    public interface ICustomerCompanyRepository
    {
        Task<IEnumerable<CustomerCompany>> GetAllCustomerCompaniesAsync();
        Task<bool> CreateCustomerCompanyAsync(CompanyRegistrationRequest request);
    }

    public interface IVendorPersonRepository
    {
        Task<IEnumerable<VendorPerson>> GetAllVendorPersonsAsync();
        Task<bool> CreateVendorPersonAsync(Atlas.Core.Models.PersonRegistrationRequest request);
    }

    public interface ICustomerPersonRepository
    {
        Task<IEnumerable<CustomerPerson>> GetAllCustomerPersonsAsync();
        Task<bool> CreateCustomerPersonAsync(Atlas.Core.Models.PersonRegistrationRequest request);
    }
}
