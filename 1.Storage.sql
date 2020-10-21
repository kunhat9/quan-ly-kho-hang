USE master
GO
DROP DATABASE DB_STORAGES
GO
CREATE DATABASE DB_STORAGES
GO
USE DB_STORAGES
GO
CREATE TABLE TB_Users		-- Người dùng
(
	UserId int IDENTITY PRIMARY KEY
	, Username nvarchar(30)
	, UserFullName nvarchar(255) -- họ tên
	, UserAddress nvarchar(255) -- địa chỉ 
	, UserPhone nvarchar(20) -- số điện thoại 
	, UserDateCreated Datetime -- ngày tạo user
	, UserPassword nvarchar(255) -- mật khẩu đăng nhập 
	, UserType int		-- Phân loại: ADMIN / STAFF
	, UserStatus int	-- Trạng thái
	, UserNote nvarchar(100)	-- Ghi chú
	, Avatar nvarchar(max)
)
GO
ALTER TABLE TB_Users ALTER COLUMN UserType INT
GO
CREATE TABLE TB_Providers	-- Nhà cung cấp
(
	ProviderId int IDENTITY PRIMARY KEY
	, ProviderName nvarchar(100)	-- Tên ncc
	, ProviderAddress nvarchar(200)	-- Địa chỉ
	, ProviderPhone nvarchar(20)	-- Sdt
	, ProviderEmail nvarchar(50)	-- Email
	, ProviderNote nvarchar(100)	-- Ghi chú
	, ProviderStatus int	-- Trạng thái: A/D
	, Logo nvarchar(max)			-- logo nhà cung cấp
)
GO
CREATE TABLE TB_Categories  -- loại sản phẩm
(
	CategoriesId int identity PRIMARY KEY
	, CategoriesName nvarchar(255)   -- tên loại
	, CategoriesNote nvarchar(255)	-- mô tả
	, CategoriesStatus int			-- trạng thái
)
GO

CREATE TABLE TB_Products	-- Sản phẩm
(
	ProductId int IDENTITY PRIMARY KEY
	, ProductCode nvarchar(20)		-- Mã sp
	, ProductName nvarchar(50)		-- Tên sp
	, ProductImage nvarchar(MAX)	-- Hình ảnh
	, ProductNote nvarchar(100)		-- Mô tả thêm
	, ProductPrice decimal(18,2)	-- giá
	, ProductStatus INT     -- Trạng thái :
	, ProductProviderId int			-- Mã ncc
	--, CONSTRAINT FK_ProductProviderId FOREIGN KEY (ProductProviderId) REFERENCES TB_PROVIDERS(ProviderId)
	, ProductCategoriesId int			-- Mã file
	--, CONSTRAINT FK_ProductCategoriesId FOREIGN KEY (ProductCategoriesId) REFERENCES TB_Categories(CategoriesId)

)

GO
CREATE TABLE TB_Orders	-- Đơn hàng
(
	OrderId int IDENTITY PRIMARY KEY
	, OrderCode nvarchar(20)		-- Mã đơn hàng
	, OrderType int		-- Loại đơn: NHAP/XUAT
	, OrderDate datetime				-- Ngày nhập/xuất hàng
	, OrderProviderId int			-- Mã ncc
	--, CONSTRAINT FK_OrderProviderId FOREIGN KEY (OrderProviderId) REFERENCES TB_PROVIDERS(ProviderId)
	, OrderUserId int				-- Mã người nhập/xuất
	--, CONSTRAINT FK_OrderUserId FOREIGN KEY (OrderUserId) REFERENCES TB_USERS(UserId)
)
GO
CREATE TABLE TB_OrderDetails	-- Chi tiết đơn hàng
(
	DetailId int IDENTITY PRIMARY KEY
	, DetailNumber int				-- Số lượng
	, DetailPrice decimal(18,2)		-- Đơn giá
	, DetailValueDate date			-- Ngày sản xuất
	, DetailExpiredDate date		-- Ngày hết hạn
	, DetailOrderId int				-- Mã đơn hàng
	--, CONSTRAINT FK_DetailOrderId FOREIGN KEY (DetailOrderId) REFERENCES TB_ORDERS(OrderId)
	, DetailProductId int			-- Mã sp
	--, CONSTRAINT FK_DetailProductId FOREIGN KEY (DetailProductId) REFERENCES TB_PRODUCTS(ProductId)
)
GO
CREATE TABLE TB_Inventory -- thông tin kiểm kê hàng hóa
(
	Id int identity Primary key
	, CreatedDate datetime -- ngày thực hiện
	, UserId int -- người thực hiện
	, Note nvarchar(max) -- mô tả , lý do kiểm kê
)
GO
CREATE TABLE TB_InventoryDetails -- chi tiết bản kiểm kê
(
	Id int identity Primary key
	, ProductId int -- sản phẩm
	, Unit int -- đơn vị tính
	, OrderId int -- mã hóa đơn ( nếu kiểm kê theo lô hàng thì cần còn không thì thôi )
	, Total	int	-- số lượng ban đầu tính theo tổng hóa đơn
	, TotalRemaining int -- số lượng còn lại
	, TotalUsed int		-- số lượng đã sử dụng
	, Note nvarchar(max) -- lý do thừa , thiếu , đủ
	, StatusID int		-- trạng thái hàng hóa ( hết hạn , còn hạn , còn hàng )
)	
GO
CREATE TABLE AppConfig
(
	Id int identity Primary key
	, ImageLogin nvarchar(max)
	, ImagePanelLogin varchar(max)
)

INSERT INTO [dbo].[TB_USERS]([Username],[UserPassword],[UserType],[UserStatus],[UserNote], UserFullName , UserAddress , UserPhone,UserDateCreated)
     VALUES ('ADMIN', N'/f9e7bsNi+c=', N'ADMIN', 1,N'',N'Admin Hệ thống',N'',N'',GETDATE())

/*
	1, Đăng nhập
	2, Trang chủ
	3, Quản lý User (UserType: ADMIN)
		Danh sách nhân viên
		Thêm/sửa nhân viên
	4, Quản lý NCC
		Danh sách ncc - Tạo phiếu yêu cầu
		Thêm/sửa ncc
	5, Quản lý Sản phẩm
		Danh sách sp
		Thêm/sửa sp
	6, Quản lý Kho (Nhập / xuất)
		Danh sách phiếu nhập
		Thêm phiếu nhập
		Danh sách phiếu xuất
		Thêm phiếu xuất
	7, Báo cáo - Thống kê
*/