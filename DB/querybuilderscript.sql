USE [master]
GO
/****** Object:  Database [QueryBuilder]    Script Date: 11/3/2014 2:37:28 PM ******/
CREATE DATABASE [QueryBuilder]
 CONTAINMENT = NONE
 ON  PRIMARY 
( NAME = N'QueryBuilder', FILENAME = N'c:\Program Files (x86)\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\QueryBuilder.mdf' , SIZE = 3072KB , MAXSIZE = UNLIMITED, FILEGROWTH = 1024KB )
 LOG ON 
( NAME = N'QueryBuilder_log', FILENAME = N'c:\Program Files (x86)\Microsoft SQL Server\MSSQL11.SQLEXPRESS\MSSQL\DATA\QueryBuilder_log.ldf' , SIZE = 1024KB , MAXSIZE = 2048GB , FILEGROWTH = 10%)
GO
ALTER DATABASE [QueryBuilder] SET COMPATIBILITY_LEVEL = 110
GO
IF (1 = FULLTEXTSERVICEPROPERTY('IsFullTextInstalled'))
begin
EXEC [QueryBuilder].[dbo].[sp_fulltext_database] @action = 'enable'
end
GO
ALTER DATABASE [QueryBuilder] SET ANSI_NULL_DEFAULT OFF 
GO
ALTER DATABASE [QueryBuilder] SET ANSI_NULLS OFF 
GO
ALTER DATABASE [QueryBuilder] SET ANSI_PADDING OFF 
GO
ALTER DATABASE [QueryBuilder] SET ANSI_WARNINGS OFF 
GO
ALTER DATABASE [QueryBuilder] SET ARITHABORT OFF 
GO
ALTER DATABASE [QueryBuilder] SET AUTO_CLOSE ON 
GO
ALTER DATABASE [QueryBuilder] SET AUTO_CREATE_STATISTICS ON 
GO
ALTER DATABASE [QueryBuilder] SET AUTO_SHRINK OFF 
GO
ALTER DATABASE [QueryBuilder] SET AUTO_UPDATE_STATISTICS ON 
GO
ALTER DATABASE [QueryBuilder] SET CURSOR_CLOSE_ON_COMMIT OFF 
GO
ALTER DATABASE [QueryBuilder] SET CURSOR_DEFAULT  GLOBAL 
GO
ALTER DATABASE [QueryBuilder] SET CONCAT_NULL_YIELDS_NULL OFF 
GO
ALTER DATABASE [QueryBuilder] SET NUMERIC_ROUNDABORT OFF 
GO
ALTER DATABASE [QueryBuilder] SET QUOTED_IDENTIFIER OFF 
GO
ALTER DATABASE [QueryBuilder] SET RECURSIVE_TRIGGERS OFF 
GO
ALTER DATABASE [QueryBuilder] SET  DISABLE_BROKER 
GO
ALTER DATABASE [QueryBuilder] SET AUTO_UPDATE_STATISTICS_ASYNC OFF 
GO
ALTER DATABASE [QueryBuilder] SET DATE_CORRELATION_OPTIMIZATION OFF 
GO
ALTER DATABASE [QueryBuilder] SET TRUSTWORTHY OFF 
GO
ALTER DATABASE [QueryBuilder] SET ALLOW_SNAPSHOT_ISOLATION OFF 
GO
ALTER DATABASE [QueryBuilder] SET PARAMETERIZATION SIMPLE 
GO
ALTER DATABASE [QueryBuilder] SET READ_COMMITTED_SNAPSHOT OFF 
GO
ALTER DATABASE [QueryBuilder] SET HONOR_BROKER_PRIORITY OFF 
GO
ALTER DATABASE [QueryBuilder] SET RECOVERY SIMPLE 
GO
ALTER DATABASE [QueryBuilder] SET  MULTI_USER 
GO
ALTER DATABASE [QueryBuilder] SET PAGE_VERIFY CHECKSUM  
GO
ALTER DATABASE [QueryBuilder] SET DB_CHAINING OFF 
GO
ALTER DATABASE [QueryBuilder] SET FILESTREAM( NON_TRANSACTED_ACCESS = OFF ) 
GO
ALTER DATABASE [QueryBuilder] SET TARGET_RECOVERY_TIME = 0 SECONDS 
GO
USE [QueryBuilder]
GO
/****** Object:  Table [dbo].[FieldQuery]    Script Date: 11/3/2014 2:37:28 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[FieldQuery](
	[FieldQueryId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[TerrainId] [int] NULL,
	[ZoneId] [int] NULL,
	[CompanyId] [int] NULL,
	[FieldQueryName] [nvarchar](max) NOT NULL,
	[BlockId] [int] NULL,
 CONSTRAINT [PK_FieldQuery] PRIMARY KEY CLUSTERED 
(
	[FieldQueryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[IncidentQuery]    Script Date: 11/3/2014 2:37:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[IncidentQuery](
	[IncidentQueryId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[IncidentTypeId] [int] NULL,
	[CompanyId] [bigint] NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[IncidentQueryName] [nvarchar](max) NOT NULL,
 CONSTRAINT [PK_IncidentQuery] PRIMARY KEY CLUSTERED 
(
	[IncidentQueryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ProductionQuery]    Script Date: 11/3/2014 2:37:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ProductionQuery](
	[ProductionQueryId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CompanyId] [int] NULL,
	[ProductId] [int] NULL,
	[TerrainId] [int] NULL,
	[ZoneId] [int] NULL,
	[FieldId] [int] NULL,
	[BlockId] [int] NULL,
	[ProductionQueryName] [nvarchar](max) NOT NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
 CONSTRAINT [PK_ProductionQuery] PRIMARY KEY CLUSTERED 
(
	[ProductionQueryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WellCompletionQuery]    Script Date: 11/3/2014 2:37:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WellCompletionQuery](
	[WellCompletionQueryId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CompanyId] [int] NULL,
	[WellId] [int] NULL,
	[WellTypeId] [int] NULL,
	[TerrainId] [int] NULL,
	[ZoneId] [int] NULL,
	[CompletionTypeId] [int] NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[EquipmentId] [int] NULL,
	[WellCompletionQueryName] [nvarchar](max) NOT NULL,
	[WellClassId] [int] NULL,
 CONSTRAINT [PK_WellCompletionQuery] PRIMARY KEY CLUSTERED 
(
	[WellCompletionQueryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WellQuery]    Script Date: 11/3/2014 2:37:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WellQuery](
	[WellQueryId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[WellTypeId] [int] NULL,
	[CompanyId] [int] NULL,
	[FieldId] [int] NULL,
	[WellClassId] [int] NOT NULL,
	[TerrainId] [int] NULL,
	[ZoneId] [int] NULL,
	[WellQueryName] [nvarchar](max) NOT NULL,
	[BlockId] [int] NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
 CONSTRAINT [PK_WellQuery] PRIMARY KEY CLUSTERED 
(
	[WellQueryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[WellWorkoverQuery]    Script Date: 11/3/2014 2:37:29 PM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[WellWorkoverQuery](
	[WellWorkoverQueryId] [bigint] IDENTITY(1,1) NOT FOR REPLICATION NOT NULL,
	[CompanyId] [bigint] NULL,
	[TerrainId] [int] NULL,
	[ZoneId] [int] NULL,
	[WorkoverReasonId] [int] NULL,
	[EquipmentId] [int] NULL,
	[StartDate] [date] NULL,
	[EndDate] [date] NULL,
	[WellId] [int] NULL,
	[WellWorkoverQueryName] [nvarchar](max) NOT NULL,
	[WellClassId] [int] NULL,
	[WellTypeId] [int] NULL,
 CONSTRAINT [PK_WellWorkoverQuery] PRIMARY KEY CLUSTERED 
(
	[WellWorkoverQueryId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
SET IDENTITY_INSERT [dbo].[WellQuery] ON 

INSERT [dbo].[WellQuery] ([WellQueryId], [WellTypeId], [CompanyId], [FieldId], [WellClassId], [TerrainId], [ZoneId], [WellQueryName], [BlockId], [StartDate], [EndDate]) VALUES (3, 0, 0, 0, 0, 0, 0, N'test3', 0, CAST(0xAAF00A00 AS Date), CAST(0x0D100B00 AS Date))
SET IDENTITY_INSERT [dbo].[WellQuery] OFF
SET IDENTITY_INSERT [dbo].[WellWorkoverQuery] ON 

INSERT [dbo].[WellWorkoverQuery] ([WellWorkoverQueryId], [CompanyId], [TerrainId], [ZoneId], [WorkoverReasonId], [EquipmentId], [StartDate], [EndDate], [WellId], [WellWorkoverQueryName], [WellClassId], [WellTypeId]) VALUES (1, 0, 0, 0, 0, 0, CAST(0x4C320B00 AS Date), CAST(0x94360B00 AS Date), 0, N'test', 0, 0)
SET IDENTITY_INSERT [dbo].[WellWorkoverQuery] OFF
USE [master]
GO
ALTER DATABASE [QueryBuilder] SET  READ_WRITE 
GO
