USE [test]
GO
/****** Object:  Table [dbo].[StoreRelation]    Script Date: 2/9/2024 8:13:10 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[StoreRelation](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[store_hub_id] [int] NOT NULL,
	[store_spoke_id] [int] NOT NULL,
	[start_date] [varchar](50) NULL,
	[end_date] [varchar](50) NULL,
	[create_date] [varchar](50) NULL,
	[update_date] [varchar](50) NULL,
 CONSTRAINT [PK_StoreRelations] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]
GO
