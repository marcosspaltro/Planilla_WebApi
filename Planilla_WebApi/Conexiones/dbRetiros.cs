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
            SqlCommand cmd = new SqlCommand("SELECT * FROM dbEmpleados.dbo.vw_Retiros WHERE Suc=@sucursal AND Semana BETWEEN @semana AND DATEADD(D, 6, @semana) " +
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
            SqlCommand cmd = new SqlCommand(
                "SELECT Id, Nombre, ISNULL(Alta, '1/1/1900') Alta, Suc FROM dbEmpleados.dbo.Empleados " +
                "WHERE Suc IN(@sucursal, 100) AND (Nombre LIKE @nombre) AND Baja IS NULL ORDER BY Suc, Id", sql);
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
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return lista;

        }
    }
}
