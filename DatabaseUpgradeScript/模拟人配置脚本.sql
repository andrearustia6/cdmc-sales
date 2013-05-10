USE [processtest]
GO

/****** Object:  Table [dbo].[SimulatorConfig]    Script Date: 05/09/2013 21:41:18 ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE TABLE [dbo].[SimulatorConfig](
	[ID] [int] IDENTITY(8,1) NOT NULL,
	[AdminName] [nvarchar](50) NOT NULL,
	[SimulatorName] [nvarchar](50) NOT NULL,
	[Description] [nvarchar](4000) NULL,
	[Sequence] [int] NULL,
	[ModifiedUser] [nvarchar](4000) NULL,
	[ModifiedDate] [datetime] NULL,
	[Creator] [nvarchar](4000) NULL,
	[CreatedDate] [datetime] NULL,
 CONSTRAINT [SimulatorConfig_PK__SimulatorConfig] PRIMARY KEY CLUSTERED 
(
	[ID] ASC
)WITH (PAD_INDEX  = OFF, STATISTICS_NORECOMPUTE  = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS  = ON, ALLOW_PAGE_LOCKS  = ON) ON [PRIMARY]
) ON [PRIMARY]

GO


