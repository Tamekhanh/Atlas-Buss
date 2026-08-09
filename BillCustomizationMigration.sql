-- Incremental migration: increase bill customizability.
-- Idempotent — safe to run multiple times.
--
-- Changes:
--   1. Widen BillTemplates.HeaderNote / FooterNote from nvarchar(500) -> nvarchar(4000)
--      so long header/footer notes fit (entity now uses [StringLength(4000)]).
-- No other schema change is required: all new bill customization options
-- (custom title/subtitle, line columns, amount-in-words, accent color, page
-- margin, logo height, grand-total box style, page numbers, currency/exchange-
-- rate display, etc.) are stored inside the existing OptionsJson nvarchar(max)
-- column as JSON, so they need no new columns.

-- 1. HeaderNote -> nvarchar(4000)
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
           WHERE TABLE_NAME='BillTemplates' AND COLUMN_NAME='HeaderNote'
             AND DATA_TYPE='nvarchar' AND CHARACTER_MAXIMUM_LENGTH < 4000)
BEGIN
    ALTER TABLE dbo.BillTemplates ALTER COLUMN HeaderNote nvarchar(4000) null;
END
GO

-- 2. FooterNote -> nvarchar(4000)
IF EXISTS (SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS
           WHERE TABLE_NAME='BillTemplates' AND COLUMN_NAME='FooterNote'
             AND DATA_TYPE='nvarchar' AND CHARACTER_MAXIMUM_LENGTH < 4000)
BEGIN
    ALTER TABLE dbo.BillTemplates ALTER COLUMN FooterNote nvarchar(4000) null;
END
GO
