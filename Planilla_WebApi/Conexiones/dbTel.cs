using Planilla_WebApi.Modelos;
using System.Data;
using System.Data.SqlClient;

namespace Planilla_WebApi.Conexiones
{
    public class dbTel
    {
        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");

        public int Id;

        public dbTel()
        {

        }
        #region " Telefonos "
        public IList<Telefonos>? Telefonos(int f_suc)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand($"SELECT Top 1 Cuit, Nombre, Telefono, Enlace, N_cliente FROM vw_servicio_internet WHERE id = {f_suc}", sql);

            cmd.CommandType = CommandType.Text;

            List<Telefonos> _Telefonoss = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    _Telefonoss.Add(new()
                    {
                        Cuit = dr["Cuit"].ToString(),
                        Nombre = dr["Nombre"].ToString(),
                        Telefono = dr["Telefono"].ToString(),
                        Enlace = dr["Enlace"].ToString(),
                        N_cliente = dr["N_cliente"].ToString(),
                    });
                }

                dr.Close();
                sql.Close();

                return _Telefonoss;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Telefonoss = null;
                return _Telefonoss;
            }
        }

        #endregion
    }
}

