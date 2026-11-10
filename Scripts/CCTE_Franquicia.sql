-- =============================================================================
-- Cuenta corriente de franquicias: saldo anterior arrastrado y stock
-- Ejecutar completo en dbGastos.
--
-- Regla: Saldo = Saldo anterior + Stock inicial + Entradas - Entregas - Stock final
-- (positivo = la franquicia debe; negativo = saldo a favor)
--
-- Cambios respecto de la versión anterior:
--  1. fn_SaldoFranquicia sumaba solo la semana pedida (@FInicio = @F), así que el
--     saldo no arrastraba lo de las semanas anteriores. Ahora acumula desde el
--     inicio (26/10/2026), incluida la franquicia de cada semana.
--  2. Se resta el stock al cierre de la semana (domingo) y se suma el del cierre
--     de la semana anterior, para no restar el mismo stock dos veces.
--  3. El "Saldo" que se muestra es la suma de las filas de la pantalla, con lo
--     que siempre cierra. Coincide con fn_SaldoFranquicia porque las dos usan
--     las mismas fuentes:
--       - Ventas: vw_Ventas en las dos (la función usaba la tabla Ventas).
--       - Salida de mercadería: Costo_SalidaFR en las dos (el SP mostraba Costo_EntradaFR).
--       - Entregas y entregas digitales: sin INNER JOIN a Tipo_Cuentas (el SP
--         dejaba afuera las que tenían un tipo inexistente; la función no).
--  4. Las filas van a una tabla variable propia de cada consulta en vez de
--     ccte_temp con un número al azar (dos consultas a la vez se podían mezclar).
--  5. Importes nulos pasan a 0 (un NULL hacía fallar la API y la pantalla quedaba vacía).
--
-- fn_SaldoFranquicia mantiene su signo (negativo = debe) por si se usa en otro lado;
-- el SP lo invierte para mostrar positivo = debe.
-- =============================================================================

ALTER FUNCTION [dbo].[fn_SaldoFranquicia]
(
 @S INT, @F DATE
)
RETURNS float
AS
BEGIN
 -- Saldo al cierre de la semana que empieza el lunes @F.
 -- Negativo = la franquicia debe.
 -- = entregas acumuladas - entradas acumuladas + stock al cierre de la semana
 DECLARE @Inicio date = '2026-10-26'
 DECLARE @Fin date = DATEADD(DAY, 6, @F)
 DECLARE @T float = 0

 -- STOCK AL CIERRE DE LA SEMANA (domingo)
 SET @T = ISNULL((SELECT SUM(V.Kilos * V.Costo_Franquicia) FROM dbDatos.dbo.Stock V WHERE V.Fecha = @Fin AND V.Id_Sucursales=@S), 0)

 -- Antes del inicio solo cuenta el stock (el saldo de la semana anterior al
 -- 26/10 es el stock del domingo 25/10)
 IF (@Fin < @Inicio)
  RETURN @T

 -- FRANQUICIA (una por semana)
 DECLARE @Sem date = @Inicio
 WHILE (@Sem <= @F)
 BEGIN
  SET @T -= ISNULL(dbDatos.dbo.fr_Franquicia(@S, @Sem), 0)
  SET @Sem = DATEADD(DAY, 7, @Sem)
 END

 -- CARGAS SOCIALES
 SET @T -= ISNULL((SELECT SUM(v.Importe) FROM dbDatos.dbo.valor_cargassociales v WHERE v.Fecha BETWEEN @Inicio AND @Fin AND v.id_sucursal=@S), 0)
 -- GASTOS OFICINA
 SET @T -= ISNULL((SELECT SUM(v.Importe) FROM dbDatos.dbo.valor_gasto v WHERE v.Fecha BETWEEN @Inicio AND @Fin AND v.id_sucursal=@S), 0)
 -- IMPUESTOS
 SET @T -= ISNULL((SELECT SUM(v.Importe) FROM dbDatos.dbo.valor_impuestos v WHERE v.Fecha BETWEEN @Inicio AND @Fin AND v.id_sucursal=@S), 0)
 -- VENTAS
 SET @T -= ISNULL((SELECT SUM(V.Kilos * V.Costo_Franquicia) FROM dbDatos.dbo.vw_Ventas V WHERE V.Fecha BETWEEN @Inicio AND @Fin AND V.Id_Sucursales=@S), 0)
 -- CARNE
 SET @T -= ISNULL((SELECT SUM(H.Media * H.Costo_Franquicia) FROM dbDatos.dbo.Hacienda_Salidas H WHERE H.Fecha BETWEEN @Inicio AND @Fin AND H.Id_Sucursales=@S), 0)
 -- TRASLADOS ENTRADA
 SET @T -= ISNULL((SELECT SUM(T.Kilos * T.Costo_EntradaFR) FROM dbDatos.dbo.Traslados T WHERE T.Fecha BETWEEN @Inicio AND @Fin AND T.Suc_Entrada=@S), 0)
 -- GASTOS EMPRESA
 SET @T -= ISNULL((SELECT SUM(G.Importe) FROM dbDatos.dbo.CD_Gastos G WHERE G.Fecha BETWEEN @Inicio AND @Fin AND G.ID_TipoGastos IN(1, 10) AND G.ID_SubTipoGastos=@S), 0)

 -- ENTREGAS
 SET @T += ISNULL((SELECT SUM(E.Importe) FROM Entregas_Franquicia E WHERE E.Fecha BETWEEN @Inicio AND @Fin AND E.Suc=@S), 0)
 -- EFECTIVOS
 SET @T += ISNULL((SELECT SUM(E.Importe) FROM dbDatos.dbo.vw_Entregas_Suc E WHERE E.Fecha BETWEEN @Inicio AND @Fin AND E.Suc=@S), 0)
 -- ENTREGAS DIGITAL
 SET @T += ISNULL((SELECT SUM(E.Importe) FROM Entradas_Tarjeta E WHERE E.Fecha BETWEEN @Inicio AND @Fin AND E.Suc=@S), 0)
 -- TRASLADOS SALIDA
 SET @T += ISNULL((SELECT SUM(T.Kilos * T.Costo_SalidaFR) FROM dbDatos.dbo.Traslados T WHERE T.Fecha BETWEEN @Inicio AND @Fin AND T.Suc_Salida=@S), 0)

 RETURN @T
END
GO

ALTER PROCEDURE [dbo].[sp_CCTE_Franquicia]
 @S INT, @F DATE
AS
BEGIN
 SET NOCOUNT ON;

 DECLARE @Fin date = DATEADD(DAY, 6, @F)

 -- Orden: 0 saldo anterior, 1 stock inicial, 2 movimientos, 3 stock final, 4 saldo
 DECLARE @C TABLE (Orden int, ID int, Fecha date, Descripcion varchar(100), Importe float, Entrada bit, Tipo int)

 -- 0. SALDO ANTERIOR (positivo = debe)
 INSERT INTO @C VALUES
  (0, 0, @F, 'Saldo Anterior', -ISNULL(dbo.fn_SaldoFranquicia(@S, DATEADD(DAY, -7, @F)), 0), 1, 0)

 -- STOCK INICIAL (cierre de la semana anterior): se suma porque el saldo anterior ya lo descontó
 INSERT INTO @C
  SELECT 1, 9, @F, 'Stock inicial', SUM(V.Kilos * V.Costo_Franquicia), 1, 0
   FROM dbDatos.dbo.Stock V
   WHERE V.Fecha = DATEADD(DAY, -1, @F) AND V.Id_Sucursales=@S
   HAVING COUNT(*) > 0

 -- 1. ENTRADAS
 -- VENTAS
 INSERT INTO @C
  SELECT 2, 1, V.Fecha, V.Desc_Tipo, SUM(V.Kilos * V.Costo_Franquicia), 1, V.Id_Tipo
   FROM dbDatos.dbo.vw_Ventas V
   WHERE V.Fecha BETWEEN @F AND @Fin AND V.Id_Sucursales=@S
   GROUP BY V.Fecha, V.Desc_Tipo, V.Id_Tipo

 -- CARNE
 INSERT INTO @C
  SELECT 2, 2, H.Fecha, 'Carne ( ' + CONVERT(varchar, COUNT(*)) + ' medias)', SUM(H.Media * H.Costo_Franquicia), 1, 0
   FROM dbDatos.dbo.Hacienda_Salidas H
   WHERE H.Fecha BETWEEN @F AND @Fin AND H.Id_Sucursales=@S
   GROUP BY H.Fecha

 -- TRASLADOS ENTRADA
 INSERT INTO @C
  SELECT 2, 3, T.Fecha, 'Entrada de mercaderia', SUM(T.Kilos * T.Costo_EntradaFR), 1, 0
   FROM dbDatos.dbo.Traslados T
   WHERE T.Fecha BETWEEN @F AND @Fin AND T.Suc_Entrada=@S
   GROUP BY T.Fecha

 -- GASTOS EMPRESA
 INSERT INTO @C
  SELECT 2, 4, G.Fecha, LEFT(G.Descripcion, 20), G.Importe, 1, 0
   FROM dbDatos.dbo.CD_Gastos G
   WHERE G.Fecha BETWEEN @F AND @Fin AND G.ID_TipoGastos IN(1, 10) AND G.ID_SubTipoGastos=@S

 -- CARGAS SOCIALES
 INSERT INTO @C
  SELECT 2, 4, @Fin, 'Cargas Sociales', v.Importe, 1, 0
   FROM dbDatos.dbo.valor_cargassociales v
   WHERE v.Fecha BETWEEN @F AND @Fin AND v.id_sucursal=@S

 -- GASTOS OFICINA
 INSERT INTO @C
  SELECT 2, 4, @Fin, 'Gastos Oficina', v.Importe, 1, 0
   FROM dbDatos.dbo.valor_gasto v
   WHERE v.Fecha BETWEEN @F AND @Fin AND v.id_sucursal=@S

 -- IMPUESTOS
 INSERT INTO @C
  SELECT 2, 4, @Fin, 'Impuestos', v.Importe, 1, 0
   FROM dbDatos.dbo.valor_impuestos v
   WHERE v.Fecha BETWEEN @F AND @Fin AND v.id_sucursal=@S

 -- FRANQUICIA
 INSERT INTO @C VALUES
  (2, 6, @Fin, 'Franquicia', dbDatos.dbo.fr_Franquicia(@S, @F), 1, 0)

 -- 2. ENTREGAS
 INSERT INTO @C
  SELECT 2, 5, E.Fecha, 'Entregas', SUM(E.Importe), 0, 0
   FROM Entregas_Franquicia E
   WHERE E.Fecha BETWEEN @F AND @Fin AND E.Suc=@S
   GROUP BY E.Fecha
 INSERT INTO @C
  SELECT 2, 5, E.Fecha, 'Efectivos', SUM(E.Importe), 0, 0
   FROM dbDatos.dbo.vw_Entregas_Suc E
   WHERE E.Fecha BETWEEN @F AND @Fin AND E.Suc=@S
   GROUP BY E.Fecha
 INSERT INTO @C
  SELECT 2, 5, E.Fecha, 'Entregas (digital)', SUM(E.Importe), 0, 0
   FROM Entradas_Tarjeta E
   WHERE E.Fecha BETWEEN @F AND @Fin AND E.Suc=@S
   GROUP BY E.Fecha

 -- TRASLADOS SALIDA
 INSERT INTO @C
  SELECT 2, 8, T.Fecha, 'Salida de mercaderia', SUM(T.Kilos * T.Costo_SalidaFR), 0, 0
   FROM dbDatos.dbo.Traslados T
   WHERE T.Fecha BETWEEN @F AND @Fin AND T.Suc_Salida=@S
   GROUP BY T.Fecha

 -- 3. STOCK FINAL (domingo de la semana)
 INSERT INTO @C
  SELECT 3, 9, @Fin, 'Stock final', SUM(V.Kilos * V.Costo_Franquicia), 0, 0
   FROM dbDatos.dbo.Stock V
   WHERE V.Fecha = @Fin AND V.Id_Sucursales=@S
   HAVING COUNT(*) > 0

 UPDATE @C SET Importe = 0 WHERE Importe IS NULL

 -- 4. SALDO = suma de lo anterior (entradas suman, entregas y stock final restan)
 INSERT INTO @C
  SELECT 4, 7, @Fin, 'Saldo', SUM(CASE WHEN Entrada = 1 THEN Importe ELSE -Importe END), 0, 0
   FROM @C

 SELECT ID, Fecha, Descripcion, Importe, Entrada, Tipo
  FROM @C
  ORDER BY Orden, Fecha, ID
END
GO

-- Control (opcional): el saldo de la pantalla tiene que ser igual a -fn_SaldoFranquicia.
-- Cambiar sucursal y lunes:
-- EXEC dbo.sp_CCTE_Franquicia @S = 5, @F = '2026-11-02';
-- SELECT -dbo.fn_SaldoFranquicia(5, '2026-11-02') AS SaldoEsperado;
