USE [master]
GO
/****** Object:  Login [mssqluser]    Script Date: 06/04/2011 10:41:40 ******/
DROP LOGIN [mssqluser]
GO
/* For security reasons the login is created disabled and with a random password. */
CREATE LOGIN [mssqluser] WITH PASSWORD=N'@v4Ú<\4G
Ò¬^_É÷j=bx»g8»Ù!#+', DEFAULT_DATABASE=[master], DEFAULT_LANGUAGE=[us_english], CHECK_EXPIRATION=OFF, CHECK_POLICY=OFF
GO
EXEC sys.sp_addsrvrolemember @loginame = N'mssqluser', @rolename = N'sysadmin'
GO
ALTER LOGIN [mssqluser] DISABLE
GO
