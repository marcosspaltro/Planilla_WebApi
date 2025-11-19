using Planilla_WebApi.Modelos;
using System.Data;
using System.Data.SqlClient;

namespace Planilla_WebApi.Conexiones
{
    public class dbVentas
    {
        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");

        public int Id;

        #region " Ventas "
        public IList<Ventas>? Ventas(int f_suc, DateTime fecha, int tipo = 0)
        {
            string cadena = $"SELECT P.Id, P.Nombre Descripcion" +
                $", ISNULL((SELECT V.Kilos FROM vw_Ventas V WHERE V.Fecha='{fecha:MM/dd/yy}' AND v.Id_Sucursales={f_suc} AND V.Id_Productos=P.Id), 0) Kilos " +
                $", ISNULL((SELECT V.Cantidad FROM vw_Ventas V WHERE V.Fecha='{fecha:MM/dd/yy}' AND v.Id_Sucursales={f_suc} AND V.Id_Productos=P.Id), 0) cantidad " +
                $"FROM Productos P WHERE P.Id_Tipo={tipo} AND P.Ver=1 " +
                $" AND P.Id NOT IN(347, 347, 349, 350, 353, 401, 402) ORDER BY P.Id";                
            if (tipo == 0)
            {
                cadena = $"SELECT P.Id, P.Nombre Descripcion" +
                $", ISNULL((SELECT V.Kilos FROM vw_Ventas V WHERE V.Fecha='{fecha:MM/dd/yy}' AND v.Id_Sucursales={f_suc} AND V.Id_Productos=P.Id), 0) Kilos " +
                $", ISNULL((SELECT V.Cantidad FROM vw_Ventas V WHERE V.Fecha='{fecha:MM/dd/yy}' AND v.Id_Sucursales={f_suc} AND V.Id_Productos=P.Id), 0) cantidad " +
                $"FROM Productos P WHERE P.Ver=1 " +
                $" ORDER BY P.Id";
            }
                
            sql.Open();
            SqlCommand cmd = new SqlCommand(cadena, sql);

            cmd.CommandType = CommandType.Text;

            List<Ventas> _Ventas = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    _Ventas.Add(new()
                    {
                        Id_Productos = Convert.ToInt32(dr["Id"]),
                        Descripcion = dr["Descripcion"].ToString(),
                        Tipo = tipo,
                        Cantidad = Convert.ToSingle(dr["cantidad"]),
                        Kilos = Convert.ToSingle(dr["Kilos"])
                    });
                }

                dr.Close();
                sql.Close();

                return _Ventas;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Ventas = null;
                return _Ventas;
            }
        }
        
        public void Agregar(Ventas venta)
        {
            sql.Open();
            // primero borrar la venta
            string cadena = $"DELETE FROM Ventas WHERE Fecha='{venta.Fecha:MM/dd/yy}' AND Id_Sucursales={venta.Id_Sucursales} AND Id_Productos={venta.Id_Productos}";
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

            if (venta.Kilos > 0)
            {
                // Buscar el precio franquicia
                float pr = 0; 
                dbPrecios dbp = new dbPrecios();
                pr = dbp.Precio_Franquicia(venta.Id_Productos, venta.Id_Sucursales, venta.Fecha);
              
                cadena = "INSERT INTO Ventas " +
                    "(Fecha, Id_Sucursales, Id_Proveedores, Id_Productos, Descripcion, Kilos, cantidad, Costo_Venta, Costo_Compra, Costo_Franquicia) " +
         "VALUES (@Fecha, @Id_Sucursales, @Id_Proveedores, @Id_Productos, @Descripcion, @Kilos, @Cantidad, @Costo_Venta, @Costo_Compra, @Costo_Franquicia)";

                cmd = new SqlCommand(cadena, sql);

                // Agregar parámetros
                cmd.Parameters.AddWithValue("@Fecha", venta.Fecha);
                cmd.Parameters.AddWithValue("@Id_Sucursales", venta.Id_Sucursales);
                cmd.Parameters.AddWithValue("@Id_Proveedores", 69);
                cmd.Parameters.AddWithValue("@Id_Productos", venta.Id_Productos);
                cmd.Parameters.AddWithValue("@Descripcion", venta.Descripcion);
                cmd.Parameters.AddWithValue("@Kilos", venta.Kilos);
                cmd.Parameters.AddWithValue("@Cantidad", venta.Cantidad);
                cmd.Parameters.AddWithValue("@Costo_Venta", 0);
                cmd.Parameters.AddWithValue("@Costo_Compra", 0);
                cmd.Parameters.AddWithValue("@Costo_Franquicia", pr);

                try
                {
                    cmd.ExecuteNonQuery();                    

                    sql.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                } 
            }
        }
        #endregion
    }
}
