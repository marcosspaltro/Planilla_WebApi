using System.Data;
using System.Data.SqlClient;

namespace Planilla_WebApi.Conexiones
{
    public class Productos
    {
        public int Id { get; set; }
        public string Descripcion { get; set; }


        public string buscar_Descripcion(int id = 0)
        {
            if (id == 0) id = Id;

            SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");
            
            sql.Open();


            // Buscar el nombre según el id pasado y asignarlo

            try
            {
                string Cadena = $"SELECT TOP 1 ISNULL(Nombre, '') FROM Productos WHERE Id={id}";

                SqlCommand cmd = new SqlCommand(Cadena, sql);
                cmd.CommandType = CommandType.Text;
                                
                SqlDataAdapter daAdapt = new SqlDataAdapter(cmd);
                Descripcion = cmd.ExecuteScalar().ToString();

                sql.Close();

                return Descripcion;
            }
            catch (Exception)
            {
                return "";
            }
        }
    }
}
