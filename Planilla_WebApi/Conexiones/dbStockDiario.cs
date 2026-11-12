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
    }
}
