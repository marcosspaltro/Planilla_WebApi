-- Tabla para la carga diaria de Stock Carne y Stock Pollo (kilos) por sucursal.
-- Ejecutar una vez en dbDatos antes de publicar la API.
IF OBJECT_ID('dbo.Stock_Diario', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Stock_Diario (
        Id            INT IDENTITY(1,1) PRIMARY KEY,
        Fecha         DATE          NOT NULL,
        Id_Sucursales INT           NOT NULL,
        Carne         DECIMAL(10,2) NOT NULL DEFAULT 0,
        Pollo         DECIMAL(10,2) NOT NULL DEFAULT 0,
        Creado        DATETIME      NOT NULL DEFAULT GETDATE(),
        Modificado    DATETIME      NULL,
        CONSTRAINT UQ_Stock_Diario_Fecha_Suc UNIQUE (Fecha, Id_Sucursales)
    );
END
