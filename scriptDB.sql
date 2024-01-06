USE [test]
GO
/****** Object:  Table [dbo].[Item]    Script Date: 1/6/2024 8:30:07 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ItemFeature]    Script Date: 1/6/2024 8:30:07 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Store]    Script Date: 1/6/2024 8:30:07 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StoreType]    Script Date: 1/6/2024 8:30:07 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
SET IDENTITY_INSERT [dbo].[Item] ON 

INSERT [dbo].[Item] ([id], [sku_code], [sku_name], [create_date], [update_date], [effective_date]) VALUES (3, N'0001', N'cake', N'20240103', N'20240103', N'20240103')
INSERT [dbo].[Item] ([id], [sku_code], [sku_name], [create_date], [update_date], [effective_date]) VALUES (4, N'0002', N'cookie', N'20240103', N'20240103', N'20240103')
SET IDENTITY_INSERT [dbo].[Item] OFF
GO
INSERT [dbo].[ItemFeature] ([store_id], [item_id], [minimum_feature], [maximum_feature], [default_feature], [create_date], [update_date]) VALUES (1, 3, 1, 6, 3, N'20240103', N'20240103')
INSERT [dbo].[ItemFeature] ([store_id], [item_id], [minimum_feature], [maximum_feature], [default_feature], [create_date], [update_date]) VALUES (1, 4, 1, 5, 2, N'20240103', N'20240103')
GO
SET IDENTITY_INSERT [dbo].[Store] ON 

INSERT [dbo].[Store] ([id], [type_id], [store_code], [store_name], [create_date], [update_date]) VALUES (1, 1, N'1001', N'BIGC', N'20240103', N'20240103')
INSERT [dbo].[Store] ([id], [type_id], [store_code], [store_name], [create_date], [update_date]) VALUES (2, 2, N'1002', N'LOTUS', N'20240103', N'20240103')
SET IDENTITY_INSERT [dbo].[Store] OFF
GO
SET IDENTITY_INSERT [dbo].[StoreType] ON 

INSERT [dbo].[StoreType] ([id], [store_type_code], [store_type_name], [create_date], [update_date]) VALUES (1, N'2001', N'Hub', N'20240103', N'20240103')
INSERT [dbo].[StoreType] ([id], [store_type_code], [store_type_name], [create_date], [update_date]) VALUES (2, N'2002', N'Non-Hub', N'20240103', N'20240103')
INSERT [dbo].[StoreType] ([id], [store_type_code], [store_type_name], [create_date], [update_date]) VALUES (3, N'2003', N'Spoke', N'20240103', N'20240103')
SET IDENTITY_INSERT [dbo].[StoreType] OFF
GO
