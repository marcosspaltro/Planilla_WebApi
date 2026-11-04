using Microsoft.Identity.Client;
using Planilla_WebApi.Modelos;
using System.Data;
using System.Data.SqlClient;

namespace Planilla_WebApi.Conexiones
{
    public class dbEfectivos
    {
        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");

        public IList<Efectivo> Efectivos(DateTime fecha, int suc)
        {
            List<Efectivo> ef = new List<Efectivo>();
            sql.Open();
            using (SqlCommand cmd = new SqlCommand($"SELECT * FROM dbGastos.dbo.Efectivos", sql))
            {
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Efectivo en = new Efectivo()
                        {
                            Id = dr.GetInt32("Id"),
                            Fecha = fecha,
                            Sucursal = suc,
                            Importe = (Single)dr.GetDouble("Importe"),
                            Cargado = dr.GetBoolean("Cargado")
                        };
                        ef.Add(en);
                    }
                }
            }
            sql.Close();
            return ef;
        }

        internal IList<Efectivo> Efectivos_semana(DateTime fecha, int suc)
        {
            // Fecha tiene que ser un lunes, si no es lunes, restarle los días necesarios para que lo sea
            int diasParaLunes = ((int)fecha.DayOfWeek + 6) % 7; // Calcula los días para retroceder al lunes
            DateTime lunes = fecha.AddDays(-diasParaLunes);
            List<Efectivo> ef = new List<Efectivo>();
            sql.Open();
            using (SqlCommand cmd = new SqlCommand($"SELECT * FROM dbGastos.dbo.Efectivos WHERE Fecha >= @lunes AND Fecha < @lunes + 7 AND Id_Sucursal = @suc ORDER BY Fecha, Id", sql))
            {
                cmd.Parameters.AddWithValue("@lunes", lunes);
                cmd.Parameters.AddWithValue("@suc", suc);
                using (SqlDataReader dr = cmd.ExecuteReader())
                {
                    while (dr.Read())
                    {
                        Efectivo en = new Efectivo()
                        {
                            Id = dr.GetInt32("Id"),
                            Fecha = dr.GetDateTime("Fecha"),
                            Sucursal = dr.GetInt32("Id_Sucursal"),
                            Importe = (Single)dr.GetDouble("Importe"),
                            Cargado = dr.GetBoolean("Cargado")
                        };
                        ef.Add(en);
                    }
                }
            }
            sql.Close();
            return ef;
        }

        internal int Nuevo_Efectivo(Efectivo efectivo)
        {
            // Insertar un nuevo registro en la tabla Efectivos
            sql.Open();
            using (SqlCommand cmd = new SqlCommand($"INSERT INTO dbGastos.dbo.Efectivos (Fecha, Id_Sucursal, Importe, Cargado) VALUES (@fecha, @sucursal, @importe, @cargado)", sql))
            {
                cmd.Parameters.AddWithValue("@fecha", efectivo.Fecha);
                cmd.Parameters.AddWithValue("@sucursal", efectivo.Sucursal);
                cmd.Parameters.AddWithValue("@importe", efectivo.Importe);
                cmd.Parameters.AddWithValue("@cargado", efectivo.Cargado);
                int rowsAffected = cmd.ExecuteNonQuery();
                sql.Close();
                return rowsAffected;
            }
        }

        internal int Modificar_Efectivo(int id, Efectivo efectivo)
        {
            // Modificar el importe del efectivo, solo si todavía no fue cargado
            sql.Open();
            using (SqlCommand cmd = new SqlCommand($"UPDATE dbGastos.dbo.Efectivos SET Importe = @importe WHERE Id = @id AND Cargado = 0", sql))
            {
                cmd.Parameters.AddWithValue("@id", id);
                cmd.Parameters.AddWithValue("@importe", efectivo.Importe);
                int rowsAffected = cmd.ExecuteNonQuery();
                sql.Close();
                return rowsAffected;
            }
        }

        // Fecha guardada del registro, o null si no existe. Se usa para que una sucursal
        // no pueda modificar ni borrar registros de días anteriores.
        public DateTime? FechaEfectivo(int id)
        {
            try
            {
                using (SqlCommand cmd = new SqlCommand("SELECT Fecha FROM dbGastos.dbo.Efectivos WHERE Id = @id", sql))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    sql.Open();
                    var valor = cmd.ExecuteScalar();
                    sql.Close();
                    return valor == null || valor == DBNull.Value ? null : Convert.ToDateTime(valor);
                }
            }
            finally
            {
                if (sql.State == ConnectionState.Open)
                    sql.Close();
            }
        }
    }
}
