﻿CREATE PROCEDURE [clients_get_sp]
AS
BEGIN
	SET NOCOUNT ON;  
	SELECT *
	FROM Clients
END
GO

CREATE PROCEDURE [entities_get_sp]
AS
BEGIN
	SET NOCOUNT ON;  
	SELECT *
	FROM Entities
END
GO