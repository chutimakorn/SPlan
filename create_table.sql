USE [test]
GO
/****** Object:  Table [dbo].[Bom]    Script Date: 8/1/2567 19:47:39 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Bom](
	[sku_code] [varchar](50) NOT NULL,
	[min_batch_hub] [int] NULL,
	[min_batch_non_hub] [int] NULL,
	[batch_uom] [varchar](50) NULL,
	[ingredient_sku] [varchar](50) NOT NULL,
	[ingredient_name] [varchar](2000) NULL,
	[weight_hub] [float] NULL,
	[weight_uom] [varchar](50) NULL,
	[create_date] [varchar](50) NOT NULL,
	[update_date] [varchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[sku_code] ASC,
	[ingredient_sku] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[ImportLog]    Script Date: 8/1/2567 19:47:39 ******/
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
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]
GO
