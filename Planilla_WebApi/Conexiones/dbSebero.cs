using Planilla_WebApi.Modelos;
using System.Data;
using System.Data.SqlClient;

namespace Planilla_WebApi.Conexiones
{
    public class dbSebero
    {
        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");

        public int Id;
        public IList<Sebero> Sebo(int suc, DateTime fecha)
        {
            //string cadena = $"SELECT S.Id_Productos, S.Descripcion, S.Kilos FROM vw_CargaSebo S WHERE S.Id_Sucursales={suc} AND S.Fecha='{fecha:MM/dd/yy}'";
            string cadena = $"SELECT S.Id Id_Productos, S.Nombre Descripcion, ISNULL((" +
                $"SELECT SUM(SB.Kilos) FROM CargaSebo SB WHERE SB.Fecha='{fecha:MM/dd/yy}' AND SB.Id_Sucursales={suc} AND SB.Id_Productos=S.Id" +
                $"), 0) Kilos FROM Productos S WHERE S.Id IN (401, 402)";
            sql.Open();
            SqlCommand cmd = new SqlCommand(cadena, sql);

            cmd.CommandType = System.Data.CommandType.Text;
            
            List<Sebero> _Sebero = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    _Sebero.Add(new()
                    {
                        Id_Productos = Convert.ToInt32(dr["Id_Productos"]),
                        Descripcion = dr["Descripcion"].ToString(),
                        Kilos = Convert.ToSingle(dr["Kilos"])
                    });
                }

                dr.Close();
                sql.Close();
                return _Sebero;
            }
            catch (Exception er)
            {
                Console.WriteLine(er.Message);
                return null;
            }            
        }

        public void Agregar(Sebero sebero)
        {
            sql.Open();
            // primero borrar 
            string cadena = $"DELETE FROM CargaSebo WHERE Fecha='{sebero.Fecha:MM/dd/yy}' AND Id_Sucursales={sebero.Id_Sucursales} AND Id_Productos={sebero.Id_Productos}";
            SqlCommand cmd = new SqlCommand(cadena, sql);
            cmd.CommandType = CommandType.Text;
            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            if (sebero.Kilos > 0)
            {
                cadena = $"INSERT INTO CargaSebo (Fecha, Id_Seberos, Id_Sucursales, Id_Productos, Kilos, Costo) " +
                        $"VALUES ('{sebero.Fecha:MM/dd/yy}', 103, {sebero.Id_Sucursales}, {sebero.Id_Productos}" +
                        $",  {Math.Round(sebero.Kilos, 3).ToString().Replace(",", ".")}, 0)";
                cmd = new SqlCommand(cadena, sql);
                try
                {
                    cmd.ExecuteNonQuery();
                    sql.Close();
                    formulas formulas = new();
                    Id = formulas.Max_ID("CargaSebo");
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
            }
        }
    }
}