USE [test]
GO
/****** Object:  Table [dbo].[Bom]    Script Date: 1/8/2024 11:11:13 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ImportLog]    Script Date: 1/8/2024 11:11:13 PM ******/
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
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON, OPTIMIZE_FOR_SEQUENTIAL_KEY = OFF) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Item]    Script Date: 1/8/2024 11:11:13 PM ******/
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
/****** Object:  Table [dbo].[ItemFeature]    Script Date: 1/8/2024 11:11:13 PM ******/
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
/****** Object:  Table [dbo].[Store]    Script Date: 1/8/2024 11:11:13 PM ******/
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
/****** Object:  Table [dbo].[StoreType]    Script Date: 1/8/2024 11:11:13 PM ******/
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
