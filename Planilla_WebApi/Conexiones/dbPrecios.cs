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
        
        //public Precios Precio_Franquicia(int f_suc, int prod, DateTime? fecha = null)
        //{
        //    fecha = fecha ?? DateTime.Today;

        //    sql.Open();
        //    SqlCommand cmd = new SqlCommand("SELECT TOP 1 Precio FROM Precios_Franquicia WHERE Fecha<=@fecha" +
        //        $"  AND Id_Productos = @prod AND Id_Sucursales=@suc ORDER BY Fecha DESC", sql);

        //    cmd.CommandType = CommandType.Text;
        //    cmd.Parameters.AddWithValue("@fecha", fecha);
        //    cmd.Parameters.AddWithValue("@prod", prod);
        //    cmd.Parameters.AddWithValue("@suc", f_suc);

        //    Precios _precio = new();
        //    try
        //    {
        //        SqlDataReader dr = cmd.ExecuteReader();

        //        while (dr.Read())
        //        {
        //            _precio = new()
        //            {
        //                Producto = prod,
        //                Precio = Convert.ToSingle(dr["Precio"])
        //            };                    
        //        }

        //        dr.Close();
        //        sql.Close();

        //        return _precio;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        _precio.Producto = prod;
        //        _precio.Precio = 0;
        //        return _precio;
        //    }
        //}

        public float Precio_Franquicia(int f_suc, int prod, DateTime? fecha = null)
        {
            fecha = fecha ?? DateTime.Today;
            float precio = 0;

            sql.Open();
            SqlCommand cmd = new SqlCommand("SELECT TOP 1 ISNULL(Precio, 0) FROM Precios_Franquicia WHERE Fecha<=@fecha" +
                $"  AND Id_Productos = @prod AND Id_Sucursales=@suc ORDER BY Fecha DESC", sql);

            cmd.CommandType = CommandType.Text;
            cmd.Parameters.AddWithValue("@fecha", fecha);
            cmd.Parameters.AddWithValue("@prod", prod);
            cmd.Parameters.AddWithValue("@suc", f_suc);


            try
            {
                precio = cmd.ExecuteNonQuery();
                if(precio < 0)
                {
                    precio = 0;
                }
            }
            catch (Exception)
            {
                precio = 0;                
            }

            sql.Close();
            return precio;
        }
        #endregion
    }
}
