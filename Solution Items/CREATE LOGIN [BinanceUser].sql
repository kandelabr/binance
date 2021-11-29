USE [master]
GO

/* For security reasons the login is created disabled and with a random password. */
/****** Object:  Login [BinanceUser]    Script Date: 11/29/2021 12:49:42 PM ******/
CREATE LOGIN [BinanceUser] WITH PASSWORD=N'MQ3EtpAuw8EquZ5fXsH63HotOZeN4ayxwsnGLahTPvk=', DEFAULT_DATABASE=[Binance], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=ON
GO

ALTER LOGIN [BinanceUser] DISABLE
GO

ALTER SERVER ROLE [sysadmin] ADD MEMBER [BinanceUser]
GO


USE [Binance]
GO

/****** Object:  User [BinanceUser]    Script Date: 11/29/2021 12:49:30 PM ******/
CREATE USER [BinanceUser] FOR LOGIN [BinanceUser] WITH DEFAULT_SCHEMA=[dbo]
GO


