using Atlas.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Atlas.Services
{
	public interface ISalesOrderService
	{
		Task<IEnumerable<SalesOrder>> GetAllAsync();
		Task<SalesOrder?> GetByIdAsync(int id);
		Task<SalesOrder?> GetByOrderNumberAsync(string orderNumber);
		Task<bool> CreateAsync(SalesOrder order);
		Task<bool> UpdateAsync(SalesOrder order);
		Task<bool> DeleteAsync(int id);
		Task<bool> UpdateStatusAsync(int id, int newStatusId);
	}
}
