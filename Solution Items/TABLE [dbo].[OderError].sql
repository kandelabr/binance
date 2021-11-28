USE [Binance]
GO

/****** Object:  Table [dbo].[OderError]    Script Date: 11/28/2021 2:58:53 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[OderError](
	[Symbol] [varchar](100) NULL,
	[ErrorMessage] [varchar](300) NULL,
	[OrderType] [varchar](10) NULL,
	[CreatedAt] [datetime] NULL
) ON [PRIMARY]
GO


