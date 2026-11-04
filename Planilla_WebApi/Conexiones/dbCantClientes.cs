using Planilla_WebApi.Modelos;
using System.Data.SqlClient;
namespace Planilla_WebApi.Conexiones
{
    public class dbCantClientes
    {
        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");

        internal void Agregar(Cant_Clientes c)
        {
            // Agregar un nuevo registro de cantidad de clientes
            sql.Open();
            SqlCommand cmd = sql.CreateCommand();
            
            cmd.CommandText = "INSERT INTO Cantidad_Clientes (Fecha, Id_Sucursales, Cantidad) VALUES (@f, @s, @c)";
            cmd.Parameters.AddWithValue("@f", c.fecha);
            cmd.Parameters.AddWithValue("@s", c.sucursal);
            cmd.Parameters.AddWithValue("@c", c.cantidad);

            cmd.ExecuteNonQuery();

            sql.Close();
        }

        internal IList<Cant_Clientes> CantClientes(int sucursal, DateTime fecha, bool sumado = true)
        {
            // Devolver la cantidad de clientes de esa sucursal en esa fecha
            sql.Open();
            SqlCommand cmd = sql.CreateCommand();

            IList<Cant_Clientes> lista = new List<Cant_Clientes>();
            if (sumado)
            {
                cmd.CommandText = "SELECT Fecha, SUM(Cantidad) AS Cant FROM Cantidad_Clientes WHERE Fecha=@f AND Id_Sucursales=@s GROUP BY Fecha";
            }
            else
            {
                cmd.CommandText = "SELECT Fecha, Cantidad AS Cant FROM Cantidad_Clientes WHERE Fecha=@f AND Id_Sucursales=@s ORDER BY Id";
            }
            cmd.Parameters.AddWithValue("@f", fecha);
            cmd.Parameters.AddWithValue("@s", sucursal);
            SqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                lista.Add(new Cant_Clientes
                {
                    fecha = reader.GetDateTime(0),
                    sucursal = sucursal,
                    cantidad = reader.GetInt32(1)
                });

            }

            sql.Close();

            return lista;
        }
    }
}
