namespace Atlas.Core.Entities
{
    public class Role
    {
        public int Id { get; set; }
        public string RoleName { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<EmployeeAccount> EmployeeAccounts { get; set; } = new List<EmployeeAccount>();
        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }

    public class Permission
    {
        public int Id { get; set; }
        public string PermissionKey { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<RolePermission> RolePermissions { get; set; } = new List<RolePermission>();
    }

    public class RolePermission
    {
        public int RoleId { get; set; }
        public int PermissionId { get; set; }

        public Role? Role { get; set; }
        public Permission? Permission { get; set; }
    }

    public class SalesOrderStatus
    {
        public int Id { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<SalesOrder> SalesOrders { get; set; } = new List<SalesOrder>();
    }

    public class PurchaseOrderStatus
    {
        public int Id { get; set; }
        public string StatusName { get; set; } = string.Empty;
        public string? Description { get; set; }

        public ICollection<PurchaseOrder> PurchaseOrders { get; set; } = new List<PurchaseOrder>();
    }

    public class Invoice
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; } = string.Empty;
        public int OrderId { get; set; }
        public DateTime InvoiceDate { get; set; } = DateTime.Now;
        public DateOnly? DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public bool IsPaid { get; set; }

        public SalesOrder? SalesOrder { get; set; }
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }

    public class Payment
    {
        public int Id { get; set; }
        public int InvoiceId { get; set; }
        public DateTime PaymentDate { get; set; } = DateTime.Now;
        public decimal Amount { get; set; }
        public string? PaymentMethod { get; set; }
        public string? Note { get; set; }

        public Invoice? Invoice { get; set; }
    }
}
