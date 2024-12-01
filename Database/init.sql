
CREATE OR ALTER PROCEDURE usp_GetAllAddresses
AS
BEGIN
    SELECT AddressID,
           AddressLine1,
           AddressLine2,
           City,
           StateProvince,
           CountryRegion,
           PostalCode,
           rowguid,
           ModifiedDate
    FROM [AdventureWorks].[SalesLT].[Address]
END
GO


CREATE OR ALTER PROCEDURE usp_GetAddressById @AddressID INT
AS
BEGIN
    SELECT AddressID,
           AddressLine1,
           AddressLine2,
           City,
           StateProvince,
           CountryRegion,
           PostalCode,
           rowguid,
           ModifiedDate
    FROM [AdventureWorks].[SalesLT].[Address]
    WHERE AddressID = @AddressID
END
GO


CREATE OR ALTER PROCEDURE usp_InsertAddress @AddressLine1 NVARCHAR(60),
                                            @AddressLine2 NVARCHAR(60) = NULL,
                                            @City NVARCHAR(30),
                                            @StateProvince NVARCHAR(50),
                                            @CountryRegion NVARCHAR(50),
                                            @PostalCode NVARCHAR(15),
                                            @RowGuid UNIQUEIDENTIFIER OUTPUT,
                                            @ModifiedDate DATETIME,
                                            @NewAddressID INT OUTPUT
AS
BEGIN
    SET NOCOUNT ON;
    
    SET @ModifiedDate = GETDATE();
    SET @RowGuid = NEWID();
    
    INSERT INTO [AdventureWorks].[SalesLT].[Address]
    (AddressLine1, AddressLine2, City, StateProvince, CountryRegion, PostalCode, rowguid, ModifiedDate)
    VALUES (@AddressLine1, @AddressLine2, @City, @StateProvince, @CountryRegion, @PostalCode, @RowGuid, @ModifiedDate)

    SET @NewAddressID = SCOPE_IDENTITY()
END
GO


CREATE OR ALTER PROCEDURE usp_UpdateAddress @AddressID INT,
                                            @AddressLine1 NVARCHAR(60),
                                            @AddressLine2 NVARCHAR(60) = NULL,
                                            @City NVARCHAR(30),
                                            @StateProvince NVARCHAR(50),
                                            @CountryRegion NVARCHAR(50),
                                            @PostalCode NVARCHAR(15),
                                            @ModifiedDate DATETIME
AS
BEGIN
    SET @ModifiedDate = GETDATE();

    UPDATE [AdventureWorks].[SalesLT].[Address]
    SET AddressLine1  = @AddressLine1,
        AddressLine2  = @AddressLine2,
        City          = @City,
        StateProvince = @StateProvince,
        CountryRegion = @CountryRegion,
        PostalCode    = @PostalCode,
        ModifiedDate  = @ModifiedDate
    WHERE AddressID = @AddressID
END
GO


CREATE OR ALTER PROCEDURE usp_DeleteAddress @AddressID INT
AS
BEGIN
    DELETE
    FROM [AdventureWorks].[SalesLT].[Address]
    WHERE AddressID = @AddressID
END
GO
