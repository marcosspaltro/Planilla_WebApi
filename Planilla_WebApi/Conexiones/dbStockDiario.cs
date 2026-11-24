using Planilla_WebApi.Modelos;
using System.Data.SqlClient;
namespace Planilla_WebApi.Conexiones
{
    public class dbStockDiario
    {
        readonly SqlConnection sql;

        // Usa la cadena "DefaultConnection" de appsettings.json
        public dbStockDiario(string cadenaConexion)
        {
            sql = new SqlConnection(cadenaConexion);
        }

        // Guarda el stock del día. Hay un solo registro por sucursal y fecha:
        // si ya existe se actualiza (permite corregir lo cargado).
        internal void Guardar(Stock_Diario s)
        {
            sql.Open();
            SqlCommand cmd = sql.CreateCommand();

            cmd.CommandText = @"
                UPDATE Stock_Diario SET Carne=@c, Pollo=@p, Modificado=GETDATE()
                WHERE Fecha=@f AND Id_Sucursales=@s;
                IF @@ROWCOUNT = 0
                    INSERT INTO Stock_Diario (Fecha, Id_Sucursales, Carne, Pollo) VALUES (@f, @s, @c, @p);";
            cmd.Parameters.AddWithValue("@f", s.fecha.Date);
            cmd.Parameters.AddWithValue("@s", s.sucursal);
            cmd.Parameters.AddWithValue("@c", s.carne);
            cmd.Parameters.AddWithValue("@p", s.pollo);

            cmd.ExecuteNonQuery();

            sql.Close();
        }

        // Devuelve el stock cargado de esa sucursal en esa fecha (lista vacía si no hay)
        internal IList<Stock_Diario> StockDiario(int sucursal, DateTime fecha)
        {
            sql.Open();
            SqlCommand cmd = sql.CreateCommand();

            IList<Stock_Diario> lista = new List<Stock_Diario>();
            cmd.CommandText = "SELECT Fecha, Carne, Pollo FROM Stock_Diario WHERE Fecha=@f AND Id_Sucursales=@s";
            cmd.Parameters.AddWithValue("@f", fecha.Date);
            cmd.Parameters.AddWithValue("@s", sucursal);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Stock_Diario
                {
                    fecha = reader.GetDateTime(0),
                    sucursal = sucursal,
                    carne = reader.GetDecimal(1),
                    pollo = reader.GetDecimal(2)
                });
            }

            sql.Close();

            return lista;
        }

        // Listado de Stock Carne/Pollo: productos con PedirStock = 1 y los kilos que
        // cargó la sucursal ese día (null si el producto no se cargó todavía)
        internal IList<Stock_Diario_Producto> Productos(int sucursal, DateTime fecha)
        {
            sql.Open();
            SqlCommand cmd = sql.CreateCommand();

            IList<Stock_Diario_Producto> lista = new List<Stock_Diario_Producto>();
            cmd.CommandText = @"
                SELECT p.Id, ISNULL(p.Nombre, ''), sp.Kilos
                FROM Productos p
                LEFT JOIN Stock_Diario_Productos sp
                    ON sp.Id_Productos = p.Id AND sp.Fecha = @f AND sp.Id_Sucursales = @s
                WHERE p.PedirStock = 1
                ORDER BY p.Id_Tipo, p.Id";
            cmd.Parameters.AddWithValue("@f", fecha.Date);
            cmd.Parameters.AddWithValue("@s", sucursal);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Stock_Diario_Producto
                {
                    id = Convert.ToInt32(reader.GetValue(0)),
                    nombre = reader.GetString(1),
                    kilos = reader.IsDBNull(2) ? null : Convert.ToDecimal(reader.GetValue(2))
                });
            }

            sql.Close();

            return lista;
        }

        // Guarda los kilos de cada producto del día (reemplaza lo cargado antes) y
        // recalcula los totales de Stock_Diario: Pollo = productos 300 a 399, Carne = el resto.
        // Todo en una transacción para que no quede la mitad grabada.
        internal void GuardarProductos(Stock_Diario_Carga carga)
        {
            sql.Open();
            SqlTransaction tr = sql.BeginTransaction();
            try
            {
                foreach (var p in carga.productos)
                {
                    SqlCommand cmd = sql.CreateCommand();
                    cmd.Transaction = tr;
                    cmd.CommandText = @"
                        UPDATE Stock_Diario_Productos SET Kilos=@k, Modificado=GETDATE()
                        WHERE Fecha=@f AND Id_Sucursales=@s AND Id_Productos=@p;
                        IF @@ROWCOUNT = 0
                            INSERT INTO Stock_Diario_Productos (Fecha, Id_Sucursales, Id_Productos, Kilos)
                            VALUES (@f, @s, @p, @k);";
                    cmd.Parameters.AddWithValue("@f", carga.fecha.Date);
                    cmd.Parameters.AddWithValue("@s", carga.sucursal);
                    cmd.Parameters.AddWithValue("@p", p.id);
                    cmd.Parameters.AddWithValue("@k", p.kilos ?? 0);
                    cmd.ExecuteNonQuery();
                }

                SqlCommand tot = sql.CreateCommand();
                tot.Transaction = tr;
                tot.CommandText = @"
                    DECLARE @c DECIMAL(10,2), @po DECIMAL(10,2);
                    SELECT @c  = ISNULL(SUM(CASE WHEN Id_Productos BETWEEN 300 AND 399 THEN 0 ELSE Kilos END), 0),
                           @po = ISNULL(SUM(CASE WHEN Id_Productos BETWEEN 300 AND 399 THEN Kilos ELSE 0 END), 0)
                    FROM Stock_Diario_Productos WHERE Fecha=@f AND Id_Sucursales=@s;
                    UPDATE Stock_Diario SET Carne=@c, Pollo=@po, Modificado=GETDATE()
                    WHERE Fecha=@f AND Id_Sucursales=@s;
                    IF @@ROWCOUNT = 0
                        INSERT INTO Stock_Diario (Fecha, Id_Sucursales, Carne, Pollo) VALUES (@f, @s, @c, @po);";
                tot.Parameters.AddWithValue("@f", carga.fecha.Date);
                tot.Parameters.AddWithValue("@s", carga.sucursal);
                tot.ExecuteNonQuery();

                tr.Commit();
            }
            catch
            {
                tr.Rollback();
                throw;
            }
            finally
            {
                sql.Close();
            }
        }
    }
}
