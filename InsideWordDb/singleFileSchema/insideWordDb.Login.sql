USE [master]
GO
/****** Object:  Login [insideWordDb]    Script Date: 06/04/2011 10:41:40 ******/
DROP LOGIN [insideWordDb]
GO
/* For security reasons the login is created disabled and with a random password. */
CREATE LOGIN [insideWordDb] WITH PASSWORD=N'¼í%jm} çt3¶äåFWi65DG¡
$
', DEFAULT_DATABASE=[insideWordDb], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
ALTER LOGIN [insideWordDb] DISABLE
GO
