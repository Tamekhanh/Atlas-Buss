-- =============================================
-- Migration: Bill Templates - increased customizability
-- Mở rộng cột HeaderNote/FooterNote và cập nhật seed mặc định
-- cho các tùy chọn in mới (lưu trong OptionsJson).
-- Idempotent — an toàn khi chạy nhiều lần.
-- =============================================

-- 1) Tạo bảng BillTemplates nếu chưa có (cho DB mới chưa chạy migration trước đó).
IF NOT EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME='BillTemplates')
BEGIN
    CREATE TABLE dbo.BillTemplates(
        id int identity(1,1) primary key,
        TemplateName nvarchar(100) not null,
        Description nvarchar(255) null,
        PageSize nvarchar(20) not null default 'A4',
        Orientation nvarchar(10) not null default 'Portrait',
        OptionsJson nvarchar(max) null,
        HeaderNote nvarchar(4000) null,
        FooterNote nvarchar(4000) null,
        IsDefault bit not null default 0,
        IsDeleted bit not null default 0,
        CreatedAt datetime not null default GETDATE(),
        UpdatedAt datetime not null default GETDATE()
    );
END
GO

-- 2) Mở rộng HeaderNote/FooterNote lên 4000 (cho DB đã có bảng từ migration cũ
--    với nvarchar(500)). Sponsor một lần, không phạt khi cột đã đủ rộng.
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
           WHERE TABLE_NAME='BillTemplates' AND COLUMN_NAME='HeaderNote'
             AND CHARACTER_MAXIMUM_LENGTH < 4000)
BEGIN
    ALTER TABLE dbo.BillTemplates ALTER COLUMN HeaderNote nvarchar(4000) null;
END
GO

IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
           WHERE TABLE_NAME='BillTemplates' AND COLUMN_NAME='FooterNote'
             AND CHARACTER_MAXIMUM_LENGTH < 4000)
BEGIN
    ALTER TABLE dbo.BillTemplates ALTER COLUMN FooterNote nvarchar(4000) null;
END
GO

-- 3) Seed mẫu mặc định nếu chưa có dòng nào IsDefault=1.
--    OptionsJson chứa các tùy chọn in mới (mặc định an toàn, giữ hành vi cũ).
IF NOT EXISTS (SELECT 1 FROM dbo.BillTemplates WHERE IsDefault=1 AND IsDeleted=0)
BEGIN
    INSERT INTO dbo.BillTemplates
        (TemplateName, Description, PageSize, Orientation, OptionsJson, HeaderNote, FooterNote, IsDefault, IsDeleted)
    VALUES
    ('Default Invoice', N'Mau in bill mac dinh cho Sales Order.', 'A4','Portrait',
     N'{"showLogo":true,"showTaxBreakdown":true,"showSignatureLine":true,"showGrandTotalBox":true,"showCustomerInfo":true,"showWarehouseColumn":true,"billTitle":null,"billSubtitle":null,"showSkuColumn":true,"showDescriptionColumn":false,"showAmountInWords":false,"showCurrencyCode":true,"showExchangeRate":true,"showPageNumbers":true,"accentColorHex":null,"logoMaxHeight":50,"pageMargin":40,"grandTotalBoxStyle":"Box"}',
     N'Thank you for your business.', N'This is a computer generated document and does not require a signature.', 1, 0);
END
GO
