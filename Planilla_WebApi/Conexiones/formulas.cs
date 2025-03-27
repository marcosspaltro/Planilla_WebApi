using System.Data;
using System.Data.SqlClient;

namespace Planilla_WebApi.Conexiones
{
    public class formulas
    {

        public int Max_ID(string tabla, string Campo_ID = "Id")
        {
            SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");
            int d = 0;

            try
            {
                string Cadena = $"SELECT MAX({Campo_ID}) FROM {tabla}";

                SqlCommand cmd = new SqlCommand(Cadena, sql);
                cmd.CommandType = CommandType.Text;

                sql.Open();
                SqlDataAdapter daAdapt = new SqlDataAdapter(cmd);
                d = (int)cmd.ExecuteScalar();

                sql.Close();
            }
            catch (Exception)
            {
            }

            return d;
        }
    }
}
