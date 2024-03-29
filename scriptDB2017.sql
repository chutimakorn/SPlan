USE [test]
GO
/****** Object:  Table [dbo].[Bom]    Script Date: 1/10/2024 10:35:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bom](
	[sku_id] [int] NOT NULL,
	[min_batch_hub] [int] NULL,
	[min_batch_non_hub] [int] NULL,
	[batch_uom] [varchar](50) NULL,
	[ingredient_sku] [varchar](50) NOT NULL,
	[ingredient_name] [varchar](2000) NULL,
	[weight_hub] [float] NULL,
	[weight_uom] [varchar](50) NULL,
	[create_date] [varchar](50) NOT NULL,
	[update_date] [varchar](50) NULL,
 CONSTRAINT [PK__Bom__F98D5DABA861954F] PRIMARY KEY CLUSTERED 
(
	[sku_id] ASC,
	[ingredient_sku] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ImportLog]    Script Date: 1/10/2024 10:35:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImportLog](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[menu] [varchar](50) NULL,
	[message] [nvarchar](max) NULL,
	[create_date] [varchar](50) NULL,
	[status] [varchar](50) NULL,
	[current_name] [nvarchar](500) NULL,
	[old_name] [nvarchar](500) NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Item]    Script Date: 1/10/2024 10:35:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Item](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[sku_code] [varchar](50) NULL,
	[sku_name] [varchar](50) NULL,
	[create_date] [varchar](50) NULL,
	[update_date] [varchar](50) NULL,
	[effective_date] [varchar](50) NULL,
 CONSTRAINT [PK_Item] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ItemFeature]    Script Date: 1/10/2024 10:35:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ItemFeature](
	[store_id] [int] NOT NULL,
	[item_id] [int] NOT NULL,
	[minimum_feature] [int] NULL,
	[maximum_feature] [int] NULL,
	[default_feature] [int] NULL,
	[create_date] [varchar](50) NULL,
	[update_date] [varchar](50) NULL,
 CONSTRAINT [PK_ItemFeature] PRIMARY KEY CLUSTERED 
(
	[store_id] ASC,
	[item_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Store]    Script Date: 1/10/2024 10:35:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Store](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[type_id] [int] NULL,
	[store_code] [varchar](50) NULL,
	[store_name] [varchar](50) NULL,
	[create_date] [varchar](50) NULL,
	[update_date] [varchar](50) NULL,
 CONSTRAINT [PK_Store] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StoreType]    Script Date: 1/10/2024 10:35:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[store_type_code] [varchar](50) NULL,
	[store_type_name] [varchar](50) NULL,
	[create_date] [varchar](50) NULL,
	[update_date] [varchar](50) NULL,
 CONSTRAINT [PK_StoreType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 1/10/2024 10:35:40 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[username] [varchar](50) NULL,
	[password] [varchar](50) NULL,
	[role] [int] NULL,
	[name] [varchar](50) NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
INSERT [dbo].[Bom] ([sku_id], [min_batch_hub], [min_batch_non_hub], [batch_uom], [ingredient_sku], [ingredient_name], [weight_hub], [weight_uom], [create_date], [update_date]) VALUES (3, 1, 1, N'pack', N'3001', N'magarine', 4, N'Piece', N'20240108', N'20240108')
INSERT [dbo].[Bom] ([sku_id], [min_batch_hub], [min_batch_non_hub], [batch_uom], [ingredient_sku], [ingredient_name], [weight_hub], [weight_uom], [create_date], [update_date]) VALUES (4, 1, 3, N'pack', N'3003', N'ครืม', 0.002, N'Roll', N'20240108', NULL)
GO
SET IDENTITY_INSERT [dbo].[ImportLog] ON 

INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (1, N'Bom', NULL, N'20240108', N'SUCCESS', N'tae', N'taetae')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (2, N'Item', N'invalid file', N'20240108', N'unsuccessful', NULL, N'bom_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (3, N'Bom', N'[{"id":3,"sku_code":"0001","sku_name":"cake","create_date":"20240103","update_date":"20240103","effective_date":"20240103"},{"id":4,"sku_code":"0002","sku_name":"cookie","create_date":"20240103","update_date":"20240103","effective_date":"20240103"}]', N'20240108', N'success', NULL, NULL)
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (4, N'Item', NULL, N'20240110', N'success', N'item_import_20240110204917659.xlsx', N'item_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (5, N'storeType', N'The entity type ''List<StoreType>'' was not found. Ensure that the entity type has been added to the model.', N'20240110', N'unsuccessful', N'storeType_import_20240110204925118.xlsx', N'storeType_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (6, N'storeType', N'The entity type ''List<StoreType>'' was not found. Ensure that the entity type has been added to the model.', N'20240110', N'unsuccessful', N'storeType_import_20240110205410343.xlsx', N'storeType_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (7, N'storeType', NULL, N'20240110', N'success', N'storeType_import_20240110210433294.xlsx', N'storeType_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (8, N'store', N'A second operation was started on this context instance before a previous operation completed. This is usually caused by different threads concurrently using the same instance of DbContext. For more information on how to avoid threading issues with DbContext, see https://go.microsoft.com/fwlink/?linkid=2097913.', N'20240110', N'unsuccessful', N'store_import_20240110210441302.xlsx', N'store_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (9, N'store', N'A second operation was started on this context instance before a previous operation completed. This is usually caused by different threads concurrently using the same instance of DbContext. For more information on how to avoid threading issues with DbContext, see https://go.microsoft.com/fwlink/?linkid=2097913.', N'20240110', N'unsuccessful', N'store_import_20240110210923319.xlsx', N'store_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (10, N'store', N'A second operation was started on this context instance before a previous operation completed. This is usually caused by different threads concurrently using the same instance of DbContext. For more information on how to avoid threading issues with DbContext, see https://go.microsoft.com/fwlink/?linkid=2097913.', N'20240110', N'unsuccessful', N'store_import_20240110211014215.xlsx', N'store_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (11, N'store', N'A second operation was started on this context instance before a previous operation completed. This is usually caused by different threads concurrently using the same instance of DbContext. For more information on how to avoid threading issues with DbContext, see https://go.microsoft.com/fwlink/?linkid=2097913.', N'20240110', N'unsuccessful', N'store_import_20240110211150744.xlsx', N'store_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (12, N'store', NULL, N'20240110', N'success', N'store_import_20240110211214146.xlsx', N'store_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (13, N'store', NULL, N'20240110', N'success', N'store_import_20240110211456239.xlsx', N'store_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (14, N'store', NULL, N'20240110', N'success', N'store_import_20240110211718274.xlsx', N'store_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (15, N'store', NULL, N'20240110', N'success', N'store_import_20240110212002064.xlsx', N'store_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (16, N'store', NULL, N'20240110', N'success', N'store_import_20240110212827381.xlsx', N'store_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (17, N'storeType', N'invalid file', N'20240110', N'unsuccessful', NULL, N'store_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (18, N'store', NULL, N'20240110', N'success', N'store_import_20240110214044499.xlsx', N'store_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (19, N'store', NULL, N'20240110', N'success', N'store_import_20240110214743028.xlsx', N'store_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (20, N'ItemFeature', NULL, N'20240110', N'success', N'itemFeature_import_20240110215345054.xlsx', N'itemFeature_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (21, N'Item', NULL, N'20240110', N'success', N'item_import_20240110221008029.xlsx', N'item_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (22, N'storeType', NULL, N'20240110', N'success', N'storeType_import_20240110221013737.xlsx', N'storeType_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (23, N'store', NULL, N'20240110', N'success', N'store_import_20240110221018743.xlsx', N'store_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (24, N'ItemFeature', NULL, N'20240110', N'success', N'itemFeature_import_20240110221031050.xlsx', N'itemFeature_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (25, N'store', N'sku id is null', N'20240110', N'unsuccessful', N'store_import_20240110221914225.xlsx', N'store_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (26, N'store', NULL, N'20240110', N'success', N'store_import_20240110221956000.xlsx', N'store_import.xlsx')
INSERT [dbo].[ImportLog] ([id], [menu], [message], [create_date], [status], [current_name], [old_name]) VALUES (27, N'store', N'invalid file', N'20240110', N'unsuccessful', NULL, N'store_import.xlsx')
SET IDENTITY_INSERT [dbo].[ImportLog] OFF
GO
SET IDENTITY_INSERT [dbo].[Item] ON 

INSERT [dbo].[Item] ([id], [sku_code], [sku_name], [create_date], [update_date], [effective_date]) VALUES (7, N'1001', N'cake', N'20240110', NULL, N'20240101')
INSERT [dbo].[Item] ([id], [sku_code], [sku_name], [create_date], [update_date], [effective_date]) VALUES (8, N'1002', N'cookie', N'20240110', NULL, N'20240101')
SET IDENTITY_INSERT [dbo].[Item] OFF
GO
INSERT [dbo].[ItemFeature] ([store_id], [item_id], [minimum_feature], [maximum_feature], [default_feature], [create_date], [update_date]) VALUES (15, 7, 1, 4, 3, NULL, NULL)
INSERT [dbo].[ItemFeature] ([store_id], [item_id], [minimum_feature], [maximum_feature], [default_feature], [create_date], [update_date]) VALUES (15, 8, 1, 4, 3, NULL, NULL)
INSERT [dbo].[ItemFeature] ([store_id], [item_id], [minimum_feature], [maximum_feature], [default_feature], [create_date], [update_date]) VALUES (16, 7, 1, 6, 4, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[StoreType] ON 

INSERT [dbo].[StoreType] ([id], [store_type_code], [store_type_name], [create_date], [update_date]) VALUES (6, N'3001', N'Hub', N'20240110', N'20240110')
INSERT [dbo].[StoreType] ([id], [store_type_code], [store_type_name], [create_date], [update_date]) VALUES (7, N'3002', N'Spoke', N'20240110', N'20240110')
SET IDENTITY_INSERT [dbo].[StoreType] OFF
GO
SET IDENTITY_INSERT [dbo].[User] ON 

INSERT [dbo].[User] ([id], [username], [password], [role], [name]) VALUES (1, N'admin', N'1234', 1, N'Admin')
INSERT [dbo].[User] ([id], [username], [password], [role], [name]) VALUES (2, N'planner', N'0000', 2, N'User Planner')
INSERT [dbo].[User] ([id], [username], [password], [role], [name]) VALUES (3, N'storehub', N'1111', 3, N'User Store HUB')
INSERT [dbo].[User] ([id], [username], [password], [role], [name]) VALUES (4, N'storespoke', N'2222', 4, N'User Store Spoke')
SET IDENTITY_INSERT [dbo].[User] OFF
GO
