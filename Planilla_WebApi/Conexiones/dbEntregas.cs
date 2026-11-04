using Planilla_WebApi.Modelos;
using System.Data;
using System.Data.SqlClient;

namespace Planilla_WebApi.Conexiones
{
    public class dbEntregas
    {
        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");

        //Listado de Tipo_Entrega
        internal IList<Tipo_Entrega> TiposEntregas()
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM [dbGastos].[dbo].[Tipo_Cuentas] WHERE Ver=1", sql);
            cmd.CommandType = CommandType.Text;
            List<Tipo_Entrega> _Datos = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    _Datos.Add(new()
                    {
                        ID = Convert.ToInt32(dr["Id"]),
                        Nombre = dr["Nombre"].ToString()
                    });
                }
                dr.Close();
                sql.Close();
                return _Datos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Datos = null;
                return _Datos;
            }
        }

        // Listado de Entregas por fecha y sucursal
        internal IList<Entrega> Entregas(DateTime fecha, int sucursal, int tipo)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("dbGastos.dbo.sp_Entregas", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@F", fecha);
            cmd.Parameters.AddWithValue("@S", sucursal);
            cmd.Parameters.AddWithValue("@T", tipo);

            List<Entrega> _Datos = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    _Datos.Add(new()
                    {
                        ID = Convert.ToInt32(dr["ID"]),
                        Fecha = DateTime.Parse(dr["Fecha"].ToString()),
                        Sucursal = Convert.ToInt32(dr["Suc"]),
                        Tipo = Convert.ToInt16(dr["Tipo"]),
                        Descripcion = dr["Nombre"].ToString(),
                        Importe = Convert.ToSingle(dr["Importe"]),
                        Aprobado = Convert.ToBoolean(dr["Aprobado"])
                    });
                }
                dr.Close();
                sql.Close();
                return _Datos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Datos = null;
                return _Datos;
            }
        }

        internal IList<Entrega> Entregas_semana(DateTime fecha, int sucursal)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("dbGastos.dbo.sp_Entregas_Semana", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@F", fecha);
            cmd.Parameters.AddWithValue("@S", sucursal);
            List<Entrega> _Datos = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    _Datos.Add(new()
                    {
                        ID = 0,
                        Fecha = Convert.ToDateTime(dr["Fecha"]),
                        Cantidad = Convert.ToInt32(dr["Cantidad"]),
                        Sucursal = sucursal,
                        Tipo = Convert.ToInt16(dr["Tipo"]),
                        Descripcion = dr["Nombre"].ToString(),
                        Importe = Convert.ToSingle(dr["Importe"])
                    });
                }
                dr.Close();
                sql.Close();
                return _Datos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Datos = null;
                return _Datos;
            }
        }

        internal int Nueva_Entrega(Entrega entrega)
        {
            // Agrega una nueva entrega y devuelve el ID generado
            try
            {
                string cmdText = $"INSERT INTO [dbGastos].[dbo].[Entregas_Sucursales] (Fecha, Suc, Tipo, Importe) " +
                                 $"VALUES (@Fecha, @Suc, @Tipo, @Importe); " +
                                 "SELECT SCOPE_IDENTITY();";
                SqlCommand command = new SqlCommand(cmdText, sql);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@Fecha", entrega.Fecha);
                command.Parameters.AddWithValue("@Suc", entrega.Sucursal);
                command.Parameters.AddWithValue("@Tipo", entrega.Tipo);
                command.Parameters.AddWithValue("@Importe", entrega.Importe);
                command.Connection = sql;
                sql.Open();
                var id = command.ExecuteScalar();
                sql.Close();
                return Convert.ToInt32(id);
            }
            catch (Exception e)
            {
                if (sql.State == System.Data.ConnectionState.Open)
                    sql.Close();
                return 0;
            }
        }

        internal bool Eliminar_Entrega(int id)
        {
            // Elimina una entrega por ID
            try
            {
                string cmdText = $"DELETE FROM [dbGastos].[dbo].[Entregas_Sucursales] WHERE ID = @ID";
                SqlCommand command = new SqlCommand(cmdText, sql);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ID", id);
                command.Connection = sql;
                sql.Open();
                int rowsAffected = command.ExecuteNonQuery();
                sql.Close();
                return rowsAffected > 0;
            }
            catch (Exception e)
            {
                if (sql.State == System.Data.ConnectionState.Open)
                    sql.Close();
                return false;
            }
        }

        internal bool Actualizar_Entrega(int id, float importe)
        {
            // Actualiza una entrega por ID
            try
            {
                string cmdText = $"UPDATE [dbGastos].[dbo].[Entregas_Sucursales] SET Importe = @Importe WHERE ID = @ID";
                SqlCommand command = new SqlCommand(cmdText, sql);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ID", id);
                command.Parameters.AddWithValue("@Importe", importe);
                command.Connection = sql;
                sql.Open();
                int rowsAffected = command.ExecuteNonQuery();
                sql.Close();
                return rowsAffected > 0;
            }
            catch (Exception e)
            {
                if (sql.State == System.Data.ConnectionState.Open)
                    sql.Close();
                return false;
            }
        }

    }
}
