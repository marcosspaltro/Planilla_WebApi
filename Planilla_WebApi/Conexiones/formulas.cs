using System.Data;
using System.Data.SqlClient;

namespace Planilla_WebApi.Conexiones
{
    public class formulas
    {

        public int Max_ID(string tabla, string Campo_ID = "Id")
        {
            SqlConnection sql = new SqlConnection(Conexion.Cadena);
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
