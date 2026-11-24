-- Stock Carne/Pollo por producto.
-- La sucursal carga los kilos de cada producto marcado con Productos.PedirStock = 1.
-- Ejecutar una vez en dbDatos antes de publicar la API. Se puede correr más de una vez.

-- 1) Columna PedirStock en Productos (ya agregada a mano; esto solo la crea si falta)
IF COL_LENGTH('dbo.Productos', 'PedirStock') IS NULL
    ALTER TABLE dbo.Productos ADD PedirStock BIT NOT NULL CONSTRAINT DF_Productos_PedirStock DEFAULT 0;
GO

-- 2) Kilos por producto, sucursal y día. Un solo registro por fecha + sucursal + producto.
--    Los totales Carne/Pollo de Stock_Diario se siguen llenando al guardar
--    (Pollo = productos 300 a 399, Carne = el resto), así Programa1 y el inicio no cambian.
IF OBJECT_ID('dbo.Stock_Diario_Productos', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.Stock_Diario_Productos (
        Id            INT IDENTITY(1,1) PRIMARY KEY,
        Fecha         DATE          NOT NULL,
        Id_Sucursales INT           NOT NULL,
        Id_Productos  INT           NOT NULL,
        Kilos         DECIMAL(10,2) NOT NULL DEFAULT 0,
        Creado        DATETIME      NOT NULL DEFAULT GETDATE(),
        Modificado    DATETIME      NULL,
        CONSTRAINT UQ_Stock_Diario_Productos UNIQUE (Fecha, Id_Sucursales, Id_Productos)
    );
END
GO

-- 3) Control: qué productos van a aparecer en el listado
SELECT Id, Nombre, Id_Tipo FROM dbo.Productos WHERE PedirStock = 1 ORDER BY Id_Tipo, Id;
