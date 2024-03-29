USE [test]
GO
/****** Object:  Table [dbo].[PlanDetail]    Script Date: 2/19/2024 11:31:39 PM ******/
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
	[plan_wen] [int] NULL,
	[plan_thu] [int] NULL,
	[plan_fri] [int] NULL,
	[plan_sat] [int] NULL,
	[plan_sun] [int] NULL,
	[week_no] [varchar](50) NULL,
 CONSTRAINT [PK_PlanDetail] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[SaleHistory]    Script Date: 2/19/2024 11:31:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SaleHistory](
	[store_code] [varchar](50) NULL,
	[sku_code] [varchar](50) NULL,
	[day_of_week] [varchar](50) NULL,
	[qty_base] [int] NULL,
	[qty_bad] [int] NULL,
	[qty_rtc] [int] NULL,
	[qty_kl] [int] NULL,
	[bad_percent] [decimal](18, 0) NULL,
	[rtc_percent] [decimal](18, 0) NULL,
	[kl_percent] [decimal](18, 0) NULL,
	[week_no] [varchar](50) NULL,
	[id] [int] IDENTITY(1,1) NOT NULL
) ON [PRIMARY]
GO
/****** Object:  Table [dbo].[Week]    Script Date: 2/19/2024 11:31:39 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Week](
	[week_no] [varchar](50) NULL,
	[start_date] [varchar](50) NULL,
	[end_date] [varchar](50) NULL,
	[status] [varchar](50) NULL
) ON [PRIMARY]
GO
