using Planilla_WebApi.Modelos;
using System.Data.SqlClient;

namespace Planilla_WebApi.Conexiones
{
    public class dbSemanas
    {

        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");

        public IList<Semanas>? Semanas(int cantidad = 10)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand($"SELECT TOP {cantidad} * FROM Semanas ORDER BY Semana DESC", sql);
            cmd.CommandType = System.Data.CommandType.Text;
            List<Semanas> _Semanas = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    _Semanas.Add (new Modelos.Semanas
                    {
                        Semana = Convert.ToDateTime(dr["Semana"]),
                        Guardada = Convert.ToBoolean(dr["Guardada"]),
                        Cerrada = Convert.ToBoolean(dr["Cerrada"])
                    });
                }
                dr.Close();
                sql.Close();
                return _Semanas;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Semanas = null;
                return _Semanas;
            }
        }
    }
}
