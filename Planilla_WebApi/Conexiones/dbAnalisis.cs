using Planilla_WebApi.Modelos;
using System.Data;
using System.Data.SqlClient;

namespace Planilla_WebApi.Conexiones
{
    public class dbAnalisis
    {

        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");

        public IEnumerable<Ventas>? VentaAnuales(int tipo, bool mostrarSuc)
        {
            string sSuc = mostrarSuc ? ", Suc" : "";

            string cadena = $"SELECT YEAR(Fecha) AS Año{sSuc}, " +
                $"FORMAT(SUM(Kilos), 'N0', 'es-ES') AS TotalKilos " +
                $"FROM vw_VentaProductos " +
                $"WHERE YEAR(Fecha) >= 2002" +
                $"GROUP BY YEAR(Fecha){sSuc} " +
                $"ORDER BY Año{sSuc}";
            if (tipo > 0)
            {
                cadena = $"SELECT YEAR(Fecha) AS Año{sSuc}, " +
                $"FORMAT(SUM(Kilos), 'N0', 'es-ES') AS TotalKilos " +
                $"FROM vw_VentaProductos " +
                $"WHERE YEAR(Fecha) >= 2002 AND Tipo={tipo}" +
                $"GROUP BY YEAR(Fecha){sSuc} " +
                $"ORDER BY Año{sSuc}";
            }
            sql.Open();
            SqlCommand cmd = new SqlCommand(cadena, sql);
            cmd.CommandType = CommandType.Text;
            List<Ventas> _Ventas = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {

                    if (!mostrarSuc)
                    {
                        _Ventas.Add(new()
                        {
                            Tipo = Convert.ToInt32(dr["Año"]),
                            Kilos = Convert.ToSingle(dr["TotalKilos"])
                        });
                    }
                    else
                    {
                        _Ventas.Add(new()
                        {
                            Id_Sucursales = Convert.ToInt32(dr["Suc"]),
                            Tipo = Convert.ToInt32(dr["Año"]),
                            Kilos = Convert.ToSingle(dr["TotalKilos"])
                        });
                    }
                        ;
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

