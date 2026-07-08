using Atlas.Core.Entities;
using Atlas.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Atlas.Services
{
	public class SalesOrderService : ISalesOrderService
	{
		private readonly ISalesOrderRepository _salesOrderRepository;
		private readonly IPartyRepository _partyRepository;
		private readonly IEmployeeRepository _employeeRepository;
		private readonly IProductRepository _productRepository;
		private readonly IWarehouseRepository _warehouseRepository;

		public SalesOrderService(
			ISalesOrderRepository salesOrderRepository,
			IPartyRepository partyRepository,
			IEmployeeRepository employeeRepository,
			IProductRepository productRepository,
			IWarehouseRepository warehouseRepository)
		{
			_salesOrderRepository = salesOrderRepository;
			_partyRepository = partyRepository;
			_employeeRepository = employeeRepository;
			_productRepository = productRepository;
			_warehouseRepository = warehouseRepository;
		}

		public async Task<IEnumerable<SalesOrder>> GetAllAsync()
		{
			return await _salesOrderRepository.GetAllAsync();
		}

		public async Task<SalesOrder?> GetByIdAsync(int id)
		{
			if (id <= 0)
			{
				return null;
			}

			return await _salesOrderRepository.GetByIdAsync(id);
		}

		public async Task<SalesOrder?> GetByOrderNumberAsync(string orderNumber)
		{
			if (string.IsNullOrWhiteSpace(orderNumber))
			{
				return null;
			}

			return await _salesOrderRepository.GetByOrderNumberAsync(orderNumber.Trim());
		}

		public async Task<bool> CreateAsync(SalesOrder order)
		{
			if (order == null)
			{
				return false;
			}

			if (string.IsNullOrWhiteSpace(order.OrderNumber))
			{
				return false;
			}

			var existingOrder = await _salesOrderRepository.GetByOrderNumberAsync(order.OrderNumber.Trim());
			if (existingOrder != null)
			{
				return false;
			}

			if (!await ValidateHeaderAsync(order))
			{
				return false;
			}

			if (!await ValidateAndHydrateDetailsAsync(order))
			{
				return false;
			}

			RecalculateTotals(order);
			order.OrderNumber = order.OrderNumber.Trim();
			order.CurrencyId = order.CurrencyId <= 0 ? 1 : order.CurrencyId;
			order.ExchangeRate = order.ExchangeRate <= 0 ? 1.0m : order.ExchangeRate;
			order.OrderStatusId = order.OrderStatusId <= 0 ? 1 : order.OrderStatusId;

			return await _salesOrderRepository.AddAsync(order);
		}

		public async Task<bool> UpdateAsync(SalesOrder order)
		{
			if (order == null || order.Id <= 0)
			{
				return false;
			}

			if (string.IsNullOrWhiteSpace(order.OrderNumber))
			{
				return false;
			}

			var existingOrder = await _salesOrderRepository.GetByOrderNumberAsync(order.OrderNumber.Trim());
			if (existingOrder != null && existingOrder.Id != order.Id)
			{
				return false;
			}

			if (!await ValidateHeaderAsync(order))
			{
				return false;
			}

			if (order.SalesOrderDetails == null || !order.SalesOrderDetails.Any())
			{
				return false;
			}

			if (!await ValidateAndHydrateDetailsAsync(order))
			{
				return false;
			}

			RecalculateTotals(order);
			order.OrderNumber = order.OrderNumber.Trim();
			order.CurrencyId = order.CurrencyId <= 0 ? 1 : order.CurrencyId;
			order.ExchangeRate = order.ExchangeRate <= 0 ? 1.0m : order.ExchangeRate;

			return await _salesOrderRepository.UpdateAsync(order);
		}

		public async Task<bool> DeleteAsync(int id)
		{
			if (id <= 0)
			{
				return false;
			}

			return await _salesOrderRepository.DeleteAsync(id);
		}

		public async Task<bool> UpdateStatusAsync(int id, int newStatusId)
		{
			if (id <= 0 || newStatusId <= 0)
			{
				return false;
			}

			var order = await _salesOrderRepository.GetByIdAsync(id);
			if (order == null)
			{
				return false;
			}

			order.OrderStatusId = newStatusId;
			return await _salesOrderRepository.UpdateAsync(order);
		}

		private async Task<bool> ValidateHeaderAsync(SalesOrder order)
		{
			if (order.EmployeeId <= 0 || order.CustomerId <= 0)
			{
				return false;
			}

			var employee = await _employeeRepository.GetByIdAsync(order.EmployeeId);
			if (employee == null || employee.IsDeleted)
			{
				return false;
			}

			var customer = await _partyRepository.GetByIdAsync(order.CustomerId);
			if (customer == null || customer.IsDeleted || !customer.IsCustomer)
			{
				return false;
			}

			if (order.SalesOrderDetails == null || !order.SalesOrderDetails.Any())
			{
				return false;
			}

			return true;
		}

		private async Task<bool> ValidateAndHydrateDetailsAsync(SalesOrder order)
		{
			foreach (var item in order.SalesOrderDetails)
			{
				if (item == null)
				{
					return false;
				}

				if (item.VariantId <= 0 || item.WarehouseId <= 0)
				{
					return false;
				}

				if (item.Quantity <= 0 || item.UnitPrice < 0 || item.Discount < 0 || item.TaxAmount < 0)
				{
					return false;
				}

				var variant = await _productRepository.GetVariantByIdAsync(item.VariantId);
				if (variant == null)
				{
					return false;
				}

				var warehouse = await _warehouseRepository.GetByIdAsync(item.WarehouseId);
				if (warehouse == null)
				{
					return false;
				}
			}

			return true;
		}

		private static void RecalculateTotals(SalesOrder order)
		{
			if (order.SalesOrderDetails == null || !order.SalesOrderDetails.Any())
			{
				order.TotalDiscount = 0;
				order.TotalTax = 0;
				order.TotalAmount = 0;
				return;
			}

			order.TotalDiscount = order.SalesOrderDetails.Sum(item => item.Discount);
			order.TotalTax = order.SalesOrderDetails.Sum(item => item.TaxAmount);
			order.TotalAmount = order.SalesOrderDetails.Sum(item => (item.Quantity * item.UnitPrice) - item.Discount + item.TaxAmount);
		}
	}
}
