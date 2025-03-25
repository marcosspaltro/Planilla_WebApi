using Planilla_WebApi.Modelos;
using System.Data;
using System.Data.SqlClient;
using System.Reflection.Metadata;

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
                $"WHERE YEAR(Fecha) >= 2002 " +
                $"GROUP BY YEAR(Fecha){sSuc} " +
                $"ORDER BY Año{sSuc}";
            if (tipo > 0)
            {
                cadena = $"SELECT YEAR(Fecha) AS Año{sSuc}, " +
                $"FORMAT(SUM(Kilos), 'N0', 'es-ES') AS TotalKilos " +
                $"FROM vw_VentaProductos " +
                $"WHERE YEAR(Fecha) >= 2002 AND Tipo={tipo} " +
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

        public IEnumerable<Analisis>? Analisis(DateTime fecha, int suc = 0)
        {
            var dt = new DataTable("Datos");

            try
            {
                SqlParameter p1 = new SqlParameter();
                p1.ParameterName = "Suc";
                if (suc <= 0)
                {
                    p1.Value = 0;
                }
                else
                {
                    p1.Value = suc;
                }

                SqlParameter p2 = new SqlParameter();
                p2.ParameterName = "F1";
                p2.Value = fecha;

                SqlParameter p3 = new SqlParameter();
                p3.ParameterName = "Cant";
                p3.Value = 9;

                SqlParameter[] parameter = new SqlParameter[] { p1, p2, p3 };

                sql.Open();
                SqlCommand cmd = new SqlCommand("dbo.sp_Analisis", sql);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddRange(parameter);

                SqlDataAdapter daAdapt = new SqlDataAdapter(cmd);
                daAdapt.Fill(dt);

                List<Analisis> _Analisis = new();
                foreach (DataRow dr in dt.Rows)
                {
                    _Analisis.Add(new()
                    {
                        Suc = Convert.ToInt32(dr["Suc"]),
                        Balance = Convert.ToSingle(dr["Balance"]),
                        Empleados = Convert.ToSingle(dr["Empleados"]),
                        Gastos = Convert.ToSingle(dr["Gastos"]),
                        Insumos = Convert.ToSingle(dr["Insumos"]),
                        Descartes = Convert.ToSingle(dr["Descartes"]),
                        Ofertas = Convert.ToSingle(dr["Ofertas"]),
                        Traslados = Convert.ToSingle(dr["Traslados"]),
                        Reintegros = Convert.ToSingle(dr["Reintegros"]),
                        Gasto_Tes = Convert.ToSingle(dr["Gasto_Tesoreria"]),
                        Mantenimiento = Convert.ToSingle(dr["Mantenimiento"]),
                        Gasto_Emp = Convert.ToSingle(dr["Gasto_Emp"]),
                        Empleados_Emp = Convert.ToSingle(dr["Empleados_Emp"]),
                        Cant = Convert.ToInt32(dr["Cant"]),
                        Ganancia = Convert.ToSingle(dr["Ganancia"]),
                        Resultado = Convert.ToSingle(dr["Resultado"])
                    });
                }
                return _Analisis;
            }
            catch (Exception er)
            {
                Console.WriteLine("Error en la consulta:" + er.Message);
                dt = null;
            }
            return null;
        }
    }
}

