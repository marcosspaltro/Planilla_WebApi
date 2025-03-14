using Planilla_WebApi.Modelos;
using System.Data;
using System.Data.SqlClient;

namespace Planilla_WebApi.Conexiones
{
    public class dbAnalisis
    {
        
        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");

        public IEnumerable<Ventas>? VentaAnuales()
        {
            string cadena = $"SELECT YEAR(Fecha) AS Año, " +
                $"FORMAT(SUM(Kilos), 'N0', 'es-ES') AS TotalKilos " +
                $"FROM vw_VentaProductos " +
                $"WHERE YEAR(Fecha) >= 2002 " +
                $"GROUP BY YEAR(Fecha) " +
                $"ORDER BY Año";
            sql.Open();
            SqlCommand cmd = new SqlCommand(cadena, sql);
            cmd.CommandType = CommandType.Text;
            List<Ventas> _Ventas = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    _Ventas.Add(new()
                    {
                        Tipo = Convert.ToInt32(dr["Año"]),
                        Kilos = Convert.ToSingle(dr["TotalKilos"])
                    });
                }
                dr.Close();
                sql.Close();
                return _Ventas;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Ventas = null;
                return _Ventas;
            }
        }
    }
}
