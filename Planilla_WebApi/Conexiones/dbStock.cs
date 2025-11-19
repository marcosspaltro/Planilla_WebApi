using Planilla_WebApi.Modelos;
using System.Data;
using System.Data.SqlClient;

namespace Planilla_WebApi.Conexiones
{
    public class dbStock
    {
        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");

        public int Id;

        public dbStock()
        {

        }
        #region " STOCK "
        public IList<Stock>? Stocks(int f_suc, DateTime semana, bool anterior = false)
        {
            DateTime f = semana.AddDays(6);           

            if (anterior) { f = f.AddDays(-7); }

            sql.Open();
            SqlCommand cmd = new SqlCommand($"SELECT ID, Nombre AS Descripcion" +
                $", ISNULL((SELECT TOP 1 Kilos FROM Stock WHERE Id_Sucursales={f_suc} AND Fecha='{f:MM/dd/yyyy}' AND Id_Productos=Productos.Id), 0) " +
                $"AS Kilos" +
                $", {f_suc} AS ID_Sucursales, CONVERT(DATETIME, '{f:MM/dd/yyyy}') AS Fecha" +
                $", Id_Tipo" +
                $" FROM Productos WHERE Ver = 1 ORDER BY Id_Tipo, Id", sql);

            cmd.CommandType = CommandType.Text;

            List<Stock> _Stocks = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    _Stocks.Add(new()
                    {
                        Fecha = (DateTime)dr["Fecha"],
                        Sucursal = Convert.ToInt32(dr["ID_Sucursales"]),
                        Producto = Convert.ToInt32(dr["ID"]),
                        Tipo = Convert.ToInt32(dr["Id_Tipo"]),
                        Descripcion = dr["Descripcion"].ToString(),
                        Kilos = Convert.ToSingle(dr["Kilos"])
                    });
                }

                dr.Close();
                sql.Close();

                return _Stocks;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Stocks = null;
                return _Stocks;
            }
        }

        public void Agregar_registro(int suc, DateTime semana, int Id_prod, double Kilos)
        {
            try
            {
                formulas f = new formulas();
                int viejoID = f.Max_ID("Stock");

                string cmdText = $"INSERT INTO Stock (Fecha, Id_Sucursales, Id_Productos, Descripcion, Costo, Costo_Franquicia, Kilos) " +
                                        $" VALUES('{semana.AddDays(6):MM/dd/yy}', {suc}, {Id_prod}, (SELECT TOP 1 Nombre FROM Productos WHERE Id = {Id_prod}), " +
                                        $" (SELECT TOP 1 Precio FROM Precios_Sucursales WHERE Id_Sucursales = {suc} AND Id_Productos = {Id_prod}" +
                                        $" AND Fecha <= '{semana.AddDays(6):MM/dd/yy}' ORDER BY Fecha DESC)," +
                                        $" ISNULL((SELECT TOP 1 Precio FROM Precios_Franquicia WHERE Id_Sucursales = {suc} AND Id_Productos = {Id_prod}" +
                                        $" AND Fecha <= '{semana.AddDays(6):MM/dd/yy}' ORDER BY Fecha DESC), 0)" +
                                        $", {Math.Round(Kilos, 3).ToString().Replace(",", ".")})";

                SqlCommand command = new SqlCommand(cmdText, sql);
                command.CommandType = CommandType.Text;
                command.Connection = sql;
                sql.Open();

                var d = command.ExecuteNonQuery();

                sql.Close();
                                
                Id = f.Max_ID("Stock");

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

                SqlCommand command = new SqlCommand($"DELETE FROM Stock WHERE Id_Sucursales = {suc} AND Fecha = '{semana.AddDays(6):MM/dd/yy}' AND Id_Productos = {Id_prod} AND id <> " +
                    $" (SELECT TOP 1 id FROM Stock WHERE Id_Sucursales = {suc} AND Fecha = ' {semana.AddDays(6):MM/dd/yy} ' AND Id_Productos = {Id_prod} ORDER BY Id DESC) ", sql);
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
            
            if (sql.State == ConnectionState.Closed) sql.Open();


            //var d = command.ExecuteNonQuery();

            sql.Close();

        }

        


    }
}

