using Planilla_WebApi.Modelos;
using System.Data;
using System.Data.SqlClient;

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
            SqlCommand cmd = new SqlCommand($"SELECT * FROM (SELECT Prod, Nombre, CONVERT(varchar(10), FECHA, 103) AS Semana, ISNULL( Kilos, 0) AS tKilos  FROM vw_VentaProductos WHERE Suc={f_suc}) AS Venta  " +
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
                        //Tipo = Convert.ToInt32(dr["Id_Tipo"]),
                        Descripcion = dr["Nombre"].ToString(),
                        Kilos = Convert.ToSingle(dr[f.AddDays(-14).ToString("dd/MM/yyyy")]),
                        Kilos1 = Convert.ToSingle(dr[f.AddDays(-21).ToString("dd/MM/yyyy")]),
                        Kilos2 = Convert.ToSingle(dr[f.AddDays(-28).ToString("dd/MM/yyyy")]),
                        Kilos3 = Convert.ToSingle(dr[f.AddDays(-35).ToString("dd/MM/yyyy")])
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
            string nfecha = "DATEADD(DAY, -1, (SELECT MAX(Semana) FROM Semanas))";


            try
            {
                int viejoID = Max_ID("Pedidos");

                string cmdText = $"INSERT INTO Pedidos (Fecha, Id_Sucursales, Id_Productos, Descripcion, Costo, Kilos) " +
                                        $"VALUES({nfecha}, {suc}, {Id_prod}, (SELECT TOP 1 Nombre FROM Productos WHERE Id = {Id_prod}), " +
                                        $"(SELECT TOP 1 Precio FROM Precios_Sucursales WHERE Id_Sucursales = {suc} AND Id_Productos = {Id_prod}" +
                                        $" AND Fecha <= {nfecha} ORDER BY Fecha DESC)" +
                                        $", {Math.Round(Kilos, 3).ToString().Replace(",", ".")})";

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

                SqlCommand command = new SqlCommand($"DELETE FROM Pedidos WHERE Id_Sucursales = {suc} AND Fecha = {nfecha} AND Id_Productos = {Id_prod} AND id <> " +
                    $" (SELECT TOP 1 id FROM Pedidos WHERE Id_Sucursales = {suc} AND Fecha = {nfecha} AND Id_Productos = {Id_prod} ORDER BY Id DESC) ", sql);
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

