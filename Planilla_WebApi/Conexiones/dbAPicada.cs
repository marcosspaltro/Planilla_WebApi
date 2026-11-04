using Planilla_WebApi.Modelos;
using System.Data.SqlClient;

namespace Planilla_WebApi.Conexiones
{
    public class dbAPicada
    {
        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");

        public IList<APicada> A_PicadaSemana(DateTime fecha, int suc)
        {
            IList<APicada> lista = new List<APicada>();
            try
            {
                sql.Open();
                SqlCommand cmd = new SqlCommand("SELECT * FROM vw_APicada WHERE Fecha BETWEEN @Fecha AND DATEADD(DAY, 6, @Fecha) AND Id_Sucursales = @Suc ORDER BY Fecha", sql);
                cmd.Parameters.AddWithValue("@Fecha", fecha);
                cmd.Parameters.AddWithValue("@Suc", suc);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    APicada aPicada = new APicada
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        Fecha = Convert.ToDateTime(reader["Fecha"]),
                        Id_Sucursales = Convert.ToInt32(reader["Id_Sucursales"]),
                        Id_Productos_A = Convert.ToInt32(reader["Id_Productos_A"]),
                        Desc_A = reader["Desc_A"].ToString(),
                        Kilos_A = Convert.ToSingle(reader["Kilos_A"]),
                        Costo_A = Convert.ToSingle(reader["Costo_A"]),
                        Id_Productos_S = Convert.ToInt32(reader["Id_Productos_S"]),
                        Desc_S = reader["Desc_S"].ToString(),
                        Costo_S = Convert.ToSingle(reader["Costo_S"]),
                        Kilos_S = Convert.ToSingle(reader["Kilos_S"])
                    };
                    lista.Add(aPicada);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                // Manejo de errores
            }
            finally
            {
                sql.Close();
            }
            return lista;
        }

        internal bool Agregar(APicada aPicada)
        {
            try
            {
                // Buscar el precio franquicia
                float prA = 0, prS = 0;
                dbPrecios dbp = new dbPrecios();
                prA = dbp.Precio_Franquicia(aPicada.Id_Sucursales, aPicada.Id_Productos_A, aPicada.Fecha);
                prS = dbp.Precio_Franquicia(aPicada.Id_Sucursales, aPicada.Id_Productos_S, aPicada.Fecha);

                sql.Open();
                SqlCommand cmd = new SqlCommand("INSERT INTO APicada (Fecha, Id_Sucursales, Id_Productos_A, Kilos_A, Costo_A, Costo_AFr, Id_Productos_S, Kilos_S, Costo_S, Costo_SFr) " +
                    "VALUES (@Fecha, @Id_Sucursales, @Id_Productos_A, @Kilos_A, @Costo_A, @Costo_AFr, @Id_Productos_S, @Kilos_S, @Costo_S, @Costo_SFr)", sql);
                cmd.Parameters.AddWithValue("@Fecha", aPicada.Fecha);
                cmd.Parameters.AddWithValue("@Id_Sucursales", aPicada.Id_Sucursales);
                cmd.Parameters.AddWithValue("@Id_Productos_A", aPicada.Id_Productos_A);
                cmd.Parameters.AddWithValue("@Kilos_A", aPicada.Kilos_A);
                cmd.Parameters.AddWithValue("@Costo_A", aPicada.Costo_A);
                cmd.Parameters.AddWithValue("@Costo_AFr", prA);
                cmd.Parameters.AddWithValue("@Id_Productos_S", aPicada.Id_Productos_S);
                cmd.Parameters.AddWithValue("@Kilos_S", aPicada.Kilos_S);
                cmd.Parameters.AddWithValue("@Costo_S", aPicada.Costo_S);
                cmd.Parameters.AddWithValue("@Costo_SFr", prS);
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;

            }
            catch (Exception ex)
            {
                // Manejo de errores
                return false;
            }
            finally
            {
                sql.Close();
            }
        }

        internal IList<APicada> Tabla()
        {
            IList<APicada> lista = new List<APicada>();
            try
            {
                sql.Open();
                SqlCommand cmd = new SqlCommand("SELECT Id, Nombre FROM Productos WHERE Ver=1 AND Id_Tipo=1 ORDER BY Id", sql);
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    APicada aPicada = new APicada
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        Desc_A = reader["Nombre"].ToString()                        
                    };
                    lista.Add(aPicada);
                }
                reader.Close();
            }
            catch (Exception ex)
            {
                // Manejo de errores
            }
            finally
            {
                sql.Close();
            } 
            return lista;
        }

        
    }
}
