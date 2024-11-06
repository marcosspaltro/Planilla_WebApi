using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace Planilla_WebApi.Conexiones
{
    public class dbUsuarios
    {
        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");

        public dbUsuarios() { }

        public IList<User> Usuarios()
        {
            List<User> nlist = new();

            sql.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM UsuariosWeb", sql);
            cmd.CommandType = CommandType.Text;

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();

                DateTime semana = DateTime.Today;

                int daysToSubtract = (int)semana.DayOfWeek;

                daysToSubtract--;

                // Si es antes del lunes (e.g. domingo o martes), restar los días necesarios
                semana = semana.AddDays(daysToSubtract * -1);
                


                while (dr.Read())
                {
                    User user = new User();
                    user.Nivel = (int)dr["Nivel"];
                    user.Username = dr["Usuario"].ToString();
                    user.Sucursal = (int)dr["Sucursal"];
                    user.NombreSucursal = dr["NombreSucursal"].ToString();
                    CreatePasswordHash(dr["Contraseña"].ToString(), out byte[] passwordHash, out byte[] passwordSalt);
                    user.suc = Convert.ToInt32(dr["Sucursal"]);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    user.Semana = semana;
                    nlist.Add(user);
                }

                dr.Close();
                sql.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                nlist = null;
            }

            return nlist;
        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }
    }
}
