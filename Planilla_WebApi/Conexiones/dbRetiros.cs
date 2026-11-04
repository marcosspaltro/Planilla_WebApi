using Planilla_WebApi.Modelos;
using System.Data.SqlClient;

namespace Planilla_WebApi.Conexiones
{
    internal class dbRetiros
    {
        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");

        internal IList<Retiro> Retiros_Semana(int sucursal, DateTime semana)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM dbEmpleados.dbo.vw_Retiros WHERE Suc=@sucursal " +
                "AND Semana BETWEEN @semana AND DATEADD(HOUR, 20, DATEADD(D, 6, @semana)) " +
                "ORDER BY Semana, Empleado, Tipo", sql);
            cmd.Parameters.AddWithValue("@sucursal", sucursal);
            cmd.Parameters.AddWithValue("@semana", semana);

            List<Retiro> lista = new List<Retiro>();
            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new Retiro
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        Fecha = Convert.ToDateTime(reader["Semana"]),
                        Empleado = Convert.ToInt32(reader["Empleado"]),
                        Nombre = reader["Nombre"].ToString(),
                        Tipo = Convert.ToInt32(reader["Tipo"]),
                        Descripcion_Tipo = reader["Descripcion"].ToString(),
                        Tipo_Descuento = Convert.ToInt32(reader["Tipo_Descuento"]),
                        Nota = reader["Nota"].ToString(),
                        Dias = Convert.ToInt32(reader["Dias"]),
                        Importe = Convert.ToSingle(reader["Importe"])
                    });
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                sql.Close();
            }
            return lista;
        }

        internal IList<TipoRetiro> GetTipoRetiros()
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM dbEmpleados.dbo.Tipos_retiro WHERE VerEnSuc=1 ORDER BY Id", sql);
            cmd.CommandType = System.Data.CommandType.Text;

            List<TipoRetiro> lista = new List<TipoRetiro>();

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new TipoRetiro
                    {
                        ID = Convert.ToInt32(reader["ID"]),
                        Descripcion = reader["Descripcion"].ToString()
                    });
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return lista;
        }

        internal void Agregar(Retiro s)
        {
            // Agregar el nuevo retiro
            sql.Open();
            SqlCommand cmd = new SqlCommand("INSERT INTO dbEmpleados.dbo.Retiros (Semana, Empleado, Suc, Tipo, Fecha_Imputacion, Importe) " +
                "VALUES (@semana, @empleado, @sucursal, @tipo, @semana, @importe)", sql);
            cmd.Parameters.AddWithValue("@semana", s.Fecha);
            cmd.Parameters.AddWithValue("@empleado", s.Empleado);
            cmd.Parameters.AddWithValue("@sucursal", s.Sucursal);
            cmd.Parameters.AddWithValue("@tipo", s.Tipo);
            cmd.Parameters.AddWithValue("@importe", s.Importe);

            try
            {
                cmd.ExecuteNonQuery();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        internal IList<Empleado> Empleados(int sucursal, string? nombre)
        {
            // Devolver la lista de empleados para la sucursal y si se especifica un nombre, filtrar por ese nombre o similar
            string t = "SELECT Id, Nombre, ISNULL(Alta, '1/1/1900') Alta, Suc FROM dbEmpleados.dbo.Empleados " +
                "WHERE Suc IN(@sucursal, 100) AND (Nombre LIKE @nombre) AND Baja IS NULL ORDER BY Suc, Id";
            if (sucursal == 0) {
                t = "SELECT Id, Nombre, ISNULL(Alta, '1/1/1900') Alta, Suc FROM dbEmpleados.dbo.Empleados " +
                "WHERE Suc<1000 AND (Nombre LIKE @nombre) AND Baja IS NULL ORDER BY Suc, Id";
                }
            ;
            SqlCommand cmd = new SqlCommand(                t, sql);
            cmd.Parameters.AddWithValue("@sucursal", sucursal);
            cmd.Parameters.AddWithValue("@nombre", $"%{nombre}%");

            List<Empleado> lista = new List<Empleado>();

            sql.Open();

            try
            {
                SqlDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    lista.Add(new Empleado
                    {
                        Id = Convert.ToInt32(reader["Id"]),
                        Nombre = reader["Nombre"].ToString(),
                        Sucursal = Convert.ToInt32(reader["Suc"]),
                        Alta = Convert.ToDateTime(reader["Alta"])
                    });
                }
                reader.Close();

                if (sucursal == 0)
                {
                    // Si no se especifica sucursal, devolvemos todos los empleados encontrados
                    return lista;
                }
                cmd.Parameters.Clear();
                // Ahora generamos otra consulta y agregamos los empleados que no esten en la consulta anterior a lista
                cmd.CommandText = "SELECT Empleado Id, Nombre_Empleado Nombre FROM dbEmpleados2.dbo.Registros_Asistencias " +
                    "WHERE Dispositivo= CONVERT(varchar, @sucursal) AND Fecha = @F GROUP BY Empleado, Nombre_Empleado, Dispositivo ORDER BY Id";
                cmd.Parameters.AddWithValue("@sucursal", sucursal);
                cmd.Parameters.AddWithValue("@F", DateTime.Today.Date); 

                reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    int id = Convert.ToInt32(reader["Id"]);
                    if (!lista.Any(e => e.Id == id))
                    {
                        lista.Add(new Empleado
                        {
                            Id = id,
                            Nombre = reader["Nombre"].ToString(),
                            Sucursal = sucursal,
                            Alta = DateTime.Today
                        });
                    }
                }

                reader.Close();
                sql.Close();

            }            


            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return lista;

        }
    }
}
