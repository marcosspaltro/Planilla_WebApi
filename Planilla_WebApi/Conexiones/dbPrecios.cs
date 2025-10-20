using Planilla_WebApi.Modelos;
using System.Data;
using System.Data.SqlClient;

namespace Planilla_WebApi.Conexiones
{
    public class dbPrecios
    {
        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");

        public int Id;

        #region " Precios "
        public IList<Precios>? Precios(int f_suc)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand($"SELECT P.Id, P.Nombre, P.Id_Tipo, (SELECT dbo.f_Precio(GETDATE(), {f_suc}, P.Id)) Precio " +
                $"FROM Productos P WHERE P.Ver=1 AND P.Id >17 AND P.Id_Tipo NOT IN(5, 7, 8, 9, 10, 13) ORDER BY P.Id_Tipo, P.Id", sql);

            cmd.CommandType = CommandType.Text;

            List<Precios> _Precioss = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    _Precioss.Add(new()
                    {
                        Producto = Convert.ToInt32(dr["ID"]),
                        Descripcion = dr["Nombre"].ToString(),
                        Tipo = Convert.ToInt32(dr["Id_Tipo"]),
                        Precio = Convert.ToSingle(dr["Precio"])
                    });
                }

                dr.Close();
                sql.Close();

                return _Precioss;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Precioss = null;
                return _Precioss;
            }
        }

        

        #endregion
    }
}
