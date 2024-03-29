USE [test]
GO
/****** Object:  Table [dbo].[Week]    Script Date: 3/4/2024 9:17:53 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Week]') AND type in (N'U'))
DROP TABLE [dbo].[Week]
GO
/****** Object:  Table [dbo].[User]    Script Date: 3/4/2024 9:17:53 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[User]') AND type in (N'U'))
DROP TABLE [dbo].[User]
GO
/****** Object:  Table [dbo].[StoreType]    Script Date: 3/4/2024 9:17:53 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreType]') AND type in (N'U'))
DROP TABLE [dbo].[StoreType]
GO
/****** Object:  Table [dbo].[StoreRelation]    Script Date: 3/4/2024 9:17:53 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[StoreRelation]') AND type in (N'U'))
DROP TABLE [dbo].[StoreRelation]
GO
/****** Object:  Table [dbo].[Store]    Script Date: 3/4/2024 9:17:53 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Store]') AND type in (N'U'))
DROP TABLE [dbo].[Store]
GO
/****** Object:  Table [dbo].[SaleHistory]    Script Date: 3/4/2024 9:17:53 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[SaleHistory]') AND type in (N'U'))
DROP TABLE [dbo].[SaleHistory]
GO
/****** Object:  Table [dbo].[PlanDetail]    Script Date: 3/4/2024 9:17:53 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[PlanDetail]') AND type in (N'U'))
DROP TABLE [dbo].[PlanDetail]
GO
/****** Object:  Table [dbo].[ItemFeature]    Script Date: 3/4/2024 9:17:53 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ItemFeature]') AND type in (N'U'))
DROP TABLE [dbo].[ItemFeature]
GO
/****** Object:  Table [dbo].[Item]    Script Date: 3/4/2024 9:17:53 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Item]') AND type in (N'U'))
DROP TABLE [dbo].[Item]
GO
/****** Object:  Table [dbo].[ImportLog]    Script Date: 3/4/2024 9:17:53 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ImportLog]') AND type in (N'U'))
DROP TABLE [dbo].[ImportLog]
GO
/****** Object:  Table [dbo].[Bom]    Script Date: 3/4/2024 9:17:53 PM ******/
IF  EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Bom]') AND type in (N'U'))
DROP TABLE [dbo].[Bom]
GO
/****** Object:  Table [dbo].[Bom]    Script Date: 3/4/2024 9:17:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bom](
	[sku_id] [int] NOT NULL,
	[min_batch_hub] [int] NULL,
	[min_batch_non_hub] [int] NULL,
	[batch_uom] [varchar](50) COLLATE Thai_CI_AS NULL,
	[ingredient_sku] [varchar](50) COLLATE Thai_CI_AS NOT NULL,
	[ingredient_name] [varchar](2000) COLLATE Thai_CI_AS NULL,
	[weight_hub] [float] NULL,
	[weight_uom] [varchar](50) COLLATE Thai_CI_AS NULL,
	[create_date] [varchar](50) COLLATE Thai_CI_AS NOT NULL,
	[update_date] [varchar](50) COLLATE Thai_CI_AS NULL,
 CONSTRAINT [PK__Bom__F98D5DABA861954F] PRIMARY KEY CLUSTERED 
(
	[sku_id] ASC,
	[ingredient_sku] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ImportLog]    Script Date: 3/4/2024 9:17:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ImportLog](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[menu] [varchar](50) COLLATE Thai_CI_AS NULL,
	[message] [nvarchar](max) COLLATE Thai_CI_AS NULL,
	[create_date] [varchar](50) COLLATE Thai_CI_AS NULL,
	[status] [varchar](50) COLLATE Thai_CI_AS NULL,
	[current_name] [nvarchar](500) COLLATE Thai_CI_AS NULL,
	[old_name] [nvarchar](500) COLLATE Thai_CI_AS NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Item]    Script Date: 3/4/2024 9:17:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Item](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[sku_code] [varchar](50) COLLATE Thai_CI_AS NULL,
	[sku_name] [varchar](50) COLLATE Thai_CI_AS NULL,
	[create_date] [varchar](50) COLLATE Thai_CI_AS NULL,
	[update_date] [varchar](50) COLLATE Thai_CI_AS NULL,
	[effective_date] [varchar](50) COLLATE Thai_CI_AS NULL,
 CONSTRAINT [PK_Item] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ItemFeature]    Script Date: 3/4/2024 9:17:53 PM ******/
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
	[create_date] [varchar](50) COLLATE Thai_CI_AS NULL,
	[update_date] [varchar](50) COLLATE Thai_CI_AS NULL,
 CONSTRAINT [PK_ItemFeature] PRIMARY KEY CLUSTERED 
(
	[store_id] ASC,
	[item_id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[PlanDetail]    Script Date: 3/4/2024 9:17:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PlanDetail](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[sku_id] [int] NULL,
	[store_id] [int] NULL,
	[plan_mon] [int] NULL,
	[plan_tues] [int] NULL,
	[plan_wed] [int] NULL,
	[plan_thu] [int] NULL,
	[plan_fri] [int] NULL,
	[plan_sat] [int] NULL,
	[plan_sun] [int] NULL,
	[week_no] [int] NULL,
 CONSTRAINT [PK_PlanDetail] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SaleHistory]    Script Date: 3/4/2024 9:17:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SaleHistory](
	[store_code] [varchar](50) COLLATE Thai_CI_AS NULL,
	[sku_code] [varchar](50) COLLATE Thai_CI_AS NULL,
	[day_of_week] [int] NULL,
	[qty_base] [int] NULL,
	[qty_bad] [int] NULL,
	[qty_rtc] [int] NULL,
	[qty_kl] [int] NULL,
	[bad_percent] [float] NULL,
	[rtc_percent] [float] NULL,
	[kl_percent] [float] NULL,
	[week_no] [int] NULL,
	[id] [int] IDENTITY(1,1) NOT NULL,
 CONSTRAINT [PK_SaleHistory] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Store]    Script Date: 3/4/2024 9:17:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Store](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[type_id] [int] NULL,
	[store_code] [varchar](50) COLLATE Thai_CI_AS NULL,
	[store_name] [varchar](50) COLLATE Thai_CI_AS NULL,
	[start_date] [varchar](50) COLLATE Thai_CI_AS NULL,
	[end_date] [varchar](50) COLLATE Thai_CI_AS NULL,
	[create_date] [varchar](50) COLLATE Thai_CI_AS NULL,
	[update_date] [varchar](50) COLLATE Thai_CI_AS NULL,
 CONSTRAINT [PK_Store] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StoreRelation]    Script Date: 3/4/2024 9:17:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreRelation](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[store_hub_id] [int] NOT NULL,
	[store_spoke_id] [int] NOT NULL,
	[start_date] [varchar](50) COLLATE Thai_CI_AS NULL,
	[end_date] [varchar](50) COLLATE Thai_CI_AS NULL,
	[create_date] [varchar](50) COLLATE Thai_CI_AS NULL,
	[update_date] [varchar](50) COLLATE Thai_CI_AS NULL,
 CONSTRAINT [PK_StoreRelations] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[StoreType]    Script Date: 3/4/2024 9:17:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreType](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[store_type_code] [varchar](50) COLLATE Thai_CI_AS NULL,
	[store_type_name] [varchar](50) COLLATE Thai_CI_AS NULL,
	[create_date] [varchar](50) COLLATE Thai_CI_AS NULL,
	[update_date] [varchar](50) COLLATE Thai_CI_AS NULL,
 CONSTRAINT [PK_StoreType] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[User]    Script Date: 3/4/2024 9:17:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[User](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[username] [varchar](50) COLLATE Thai_CI_AS NULL,
	[password] [varchar](50) COLLATE Thai_CI_AS NULL,
	[role] [int] NULL,
	[name] [varchar](50) COLLATE Thai_CI_AS NULL,
 CONSTRAINT [PK_User] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Week]    Script Date: 3/4/2024 9:17:53 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Week](
	[week_no] [int] NULL,
	[start_date] [varchar](50) COLLATE Thai_CI_AS NULL,
	[end_date] [varchar](50) COLLATE Thai_CI_AS NULL,
	[status] [int] NULL
) ON [PRIMARY]
GO
