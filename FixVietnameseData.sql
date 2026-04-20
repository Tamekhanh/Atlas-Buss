USE AtlasDB
GO

UPDATE dbo.Addresses SET AddressType=N'Văn phòng', Street=N'123 Đường Lê Lợi', City=N'Quận 1', State=N'TP. Hồ Chí Minh', Country=N'Việt Nam' WHERE Id=1;
UPDATE dbo.Addresses SET AddressType=N'Nhà riêng', Street=N'456 Đường Nguyễn Huệ', City=N'Hoàn Kiếm', State=N'Hà Nội', Country=N'Việt Nam' WHERE Id=2;
UPDATE dbo.Addresses SET AddressType=N'Kho bãi', Street=N'789 Khu Công Nghiệp Tân Bình', City=N'Tân Phú', State=N'TP. Hồ Chí Minh', Country=N'Việt Nam' WHERE Id=3;
UPDATE dbo.Addresses SET AddressType=N'Văn phòng' WHERE Id=4;

UPDATE dbo.Persons SET FirstName=N'Văn A', LastName=N'Nguyễn' WHERE Id=1;
UPDATE dbo.Persons SET FirstName=N'Thị B', LastName=N'Trần' WHERE Id=2;
UPDATE dbo.Persons SET FirstName=N'Văn C', LastName=N'Lê' WHERE Id=3;

UPDATE dbo.Companies SET CompanyName=N'Công ty Công nghệ Atlas' WHERE Id=1;
UPDATE dbo.Companies SET CompanyName=N'Phân phối Linh kiện X' WHERE Id=3;
GO
