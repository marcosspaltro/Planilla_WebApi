using Planilla_WebApi.Modelos;
using System.Data;
using System.Data.SqlClient;
using System.Drawing.Imaging;

namespace Planilla_WebApi.Conexiones
{
    public class dbPedidos
    {
        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");

        public int Id;

        public dbPedidos()
        {

        }
        #region " Pedidos "
        public IList<Pedidos>? Pedidoss(int f_suc)
        {
            DateTime f = DateTime.Today;
            int rd = (int)f.DayOfWeek;

            f = f.AddDays(-rd + 1);

            sql.Open();
            SqlCommand cmd = new SqlCommand($"SELECT *, ISNULL((SELECT TOP 1 Kilos FROM Stock WHERE Id_Sucursales = {f_suc} AND Fecha = '{f.AddDays(-1).ToString("MM/dd/yyyy")}' AND Id_Productos = Prod),0) AS Stock," +
                $" ISNULL((SELECT SUM(Kilos) FROM Pedidos WHERE fecha BETWEEN '{f.ToString("MM/dd/yyyy")}' AND '{f.AddDays(6).ToString("MM/dd/yyyy")}' AND Suc = {f_suc} AND id_Producto = Prod), 0) As Pedido" +
                $" FROM (SELECT Prod, Tipo, Nombre, CONVERT(varchar(10), FECHA, 103) AS Semana, ISNULL( Kilos, 0) AS tKilos  FROM vw_VentaProductos WHERE Suc={f_suc}) AS Venta  " +
                $"PIVOT (SUM (tKilos) FOR Semana IN([{f.AddDays(-35).ToString("dd/MM/yyyy")}] ,  [{f.AddDays(-28).ToString("dd/MM/yyyy")}] ,  [{f.AddDays(-21).ToString("dd/MM/yyyy")}] ,  [{f.AddDays(-14).ToString("dd/MM/yyyy")}])) AS VENTAS   ORDER BY Prod", sql);

            cmd.CommandType = CommandType.Text;
            List<Pedidos> _Pedidoss = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    _Pedidoss.Add(new()
                    {
                        //Fecha = Convert.ToDateTime(dr["Fecha"].ToString()),
                        //Sucursal = Convert.ToInt32(dr["ID_Sucursales"]),
                        Producto = Convert.ToInt32(dr["Prod"]),
                        Tipo = Convert.ToInt32(dr["Tipo"]),
                        Descripcion = dr["Nombre"].ToString(),
                        Stock = Convert.ToSingle(dr["Stock"]),
                        Pedido = Convert.ToSingle(dr["Pedido"]),
                        Kilos = dr.IsDBNull(6) ? 0 : Convert.ToSingle(dr[f.AddDays(-14).ToString("dd/MM/yyyy")]),
                        Kilos1 = dr.IsDBNull(5) ? 0 : Convert.ToSingle(dr[f.AddDays(-21).ToString("dd/MM/yyyy")]),
                        Kilos2 = dr.IsDBNull(4) ? 0 : Convert.ToSingle(dr[f.AddDays(-28).ToString("dd/MM/yyyy")]),
                        Kilos3 = dr.IsDBNull(3) ? 0 : Convert.ToSingle(dr[f.AddDays(-35).ToString("dd/MM/yyyy")])
                    });
                }

                dr.Close();
                sql.Close();

                return _Pedidoss;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Pedidoss = null;
                return _Pedidoss;
            }
        }

        public IList<Pedidos>? Detalle_Pedidos(int f_suc, int Prod)
        {
            DateTime f = DateTime.Today;
            int rd = (int)f.DayOfWeek;

            f = f.AddDays(-rd + 1);

            sql.Open();
            SqlCommand cmd = new SqlCommand($"SELECT Id, Fecha, Kilos FROM Pedidos WHERE Fecha BETWEEN '{f.ToString("MM/dd/yyyy")}' AND '{f.AddDays(6).ToString("MM/dd/yyyy")}' AND Suc = {f_suc} AND Id_Producto = {Prod} ORDER BY Fecha, Id", sql);

            cmd.CommandType = CommandType.Text;

            List<Pedidos> _Pedidoss = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    _Pedidoss.Add(new()
                    {
                        ID = Convert.ToInt32(dr["ID"]),
                        Fecha = Convert.ToDateTime(dr["Fecha"].ToString()),
                        Kilos = Convert.ToSingle(dr["Kilos"])
                    });
                }

                dr.Close();
                sql.Close();

                return _Pedidoss;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Pedidoss = null;
                return _Pedidoss;
            }
        }

        public void Agregar_registro(int suc, int Id_prod, double Kilos)
        {
            try
            {
                int viejoID = Max_ID("Pedidos");

                string cmdText = $"INSERT INTO Pedidos (Fecha, Suc, Id_Producto, Kilos) VALUES ((SELECT CAST(GETDATE() AS DATE)), {suc}, {Id_prod}, {Kilos})";

                SqlCommand command = new SqlCommand(cmdText, sql);
                command.CommandType = CommandType.Text;
                command.Connection = sql;
                sql.Open();

                var d = command.ExecuteNonQuery();

                sql.Close();

                Id = Max_ID("Pedidos");

                // Si es igual es que no se agrego
                if (viejoID == Id)
                {
                    escribirLog("no se agrego");
                    Id = 0;
                }
                else
                {
                    escribirLog($"Suc: {suc}, Prod: {Id_prod}, Kilos: {Kilos:N2}");
                }
            }
            catch (Exception e)
            {
                escribirLog(e.Message);
            }

            try
            {

                SqlCommand command = new SqlCommand($"DELETE FROM Pedidos WHERE Suc = {suc} AND Fecha = (SELECT CAST(GETDATE() AS DATE)) AND Id_Producto = {Id_prod} AND id <> " +
                    $" (SELECT TOP 1 id FROM Pedidos WHERE Suc = {suc} AND Fecha = (SELECT CAST(GETDATE() AS DATE)) AND Id_Producto = {Id_prod} ORDER BY Id DESC) ", sql);
                command.CommandType = CommandType.Text;
                command.Connection = sql;
                sql.Open();

                var d = command.ExecuteNonQuery();

                sql.Close();
            }
            catch (Exception e)
            {
                escribirLog(e.Message);
            }

        }

        #endregion

        public void escribirLog(string e)
        {
            string s = $"{DateTime.Today.ToString("MM/dd/yyy")} {DateTime.Now:HH:mm}";

            SqlCommand command = new SqlCommand($"INSERT INTO logApi (fecha, texto) VALUES('{s}', '{e}')", sql);
            command.CommandType = CommandType.Text;
            command.Connection = sql;
            sql.Open();

            var d = command.ExecuteNonQuery();

            sql.Close();

        }

        public int Max_ID(string tabla, string Campo_ID = "Id")
        {
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

