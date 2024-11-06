using Planilla_WebApi.Modelos;
using System.Data;
using System.Data.SqlClient;

namespace Planilla_WebApi.Conexiones
{
    public class dbOfertas
    {
        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");

        public int Id;

        public dbOfertas()
        {

        }
        #region " Ofertas "
        public IList<Ofertas>? Ofertas(int f_suc, int tipo = 0, string semana = "1/1/2000")
        {
            DateTime f = DateTime.Now;
            int rd = (int)f.DayOfWeek;

            if (semana == "1/1/2000")
            {
                if (rd < 3) { f = f.AddDays(-rd); }
                else { f = f.AddDays(7 - rd); }
            }
            else
            {
                f = DateTime.Parse(semana);
            }

            string sTipo = "";

            if (tipo != 0)
            {
                sTipo = " AND Id_Tipo=" + tipo.ToString();
            }
            sql.Open();
            SqlCommand cmd = new SqlCommand($"SELECT ID, Nombre AS Descripcion" +
                $", ISNULL((SELECT Kilos FROM Ofertas WHERE Id_Sucursales={f_suc} AND Fecha='{f:MM/dd/yyyy}' AND Id_Productos=Productos.Id), 0) " +
                $"AS Kilos" +
                $", {f_suc} AS ID_Sucursales, CONVERT(DATETIME, '{f:MM/dd/yyyy}') AS Fecha" +
                $", ISNULL((SELECT TOP 1 ID FROM Ofertas WHERE Id_Sucursales={f_suc} AND Fecha='{f:MM/dd/yyyy}' AND Id_Productos=Productos.Id), 0) " +
                $"AS ID_Ofertas" +
                $", Id_Tipo" +
                $" FROM Productos WHERE Ver = 1 {sTipo} ORDER BY Id_Tipo, Id", sql);
            cmd.CommandType = CommandType.Text;

            List<Ofertas> _Ofertas = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    _Ofertas.Add(new()
                    {
                        fecha = Convert.ToDateTime(dr["Fecha"].ToString()),
                        id_sucursal = Convert.ToInt32(dr["ID_Sucursales"]),
                        id_productos = Convert.ToInt32(dr["ID"]),
                        descripcion = dr["Descripcion"].ToString(),
                        kilos = Convert.ToSingle(dr["Kilos"])
                    });
                }

                dr.Close();
                sql.Close();

                return _Ofertas;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Ofertas = null;
                return _Ofertas;
            }
        }

        public void Agregar_registro(string fecha, int suc, int Id_prod, double Kilos)
        {


            // Obtener el día actual de la semana (0 = domingo, 1 = lunes, ..., 6 = sábado)
            DayOfWeek diaActualSemana = DateTime.Now.DayOfWeek;

            // No tomamos la fecha que manden porque puede venir una fecha anterior por no cerrar sesion
            DateTime nfecha = DateTime.Today;

            // Si es lunes o martes se va a agregar con la fecha del domingo
            nfecha.AddDays(((int)diaActualSemana) * -1);

            if (diaActualSemana < DayOfWeek.Wednesday)
            {
                try
                {
                    int viejoID = Max_ID("Ofertas");

                    string cmdText = $"INSERT INTO Ofertas (Fecha, Id_Sucursales, Id_Productos, Descripcion, Costo, Kilos) " +
                                            $"VALUES('{nfecha:MM/dd/yyy}', {suc}, {Id_prod}, (SELECT TOP 1 Nombre FROM Productos WHERE Id = {Id_prod}), " +
                                            $"(SELECT TOP 1 Precio FROM Precios_Sucursales WHERE Id_Sucursales = {suc} AND Id_Productos = {Id_prod} AND Fecha <= '{nfecha:MM/dd/yyy}' ORDER BY Fecha DESC)" +
                                            $", {Math.Round(Kilos, 3).ToString().Replace(",", ".")})";

                    SqlCommand command = new SqlCommand(cmdText, sql);
                    command.CommandType = CommandType.Text;
                    command.Connection = sql;
                    sql.Open();

                    var d = command.ExecuteNonQuery();

                    sql.Close();

                    Id = Max_ID("Ofertas");

                    // Si es igual es que no se agrego
                    if (viejoID == Id)
                    {
                        escribirLog("no se agrego");
                        Id = 0;
                    }
                }
                catch (Exception e)
                {
                    escribirLog(e.Message);
                }

                try
                {

                    SqlCommand command = new SqlCommand($"DELETE FROM Ofertas WHERE Id_Sucursales = {suc} AND Fecha = '{nfecha:MM/dd/yyy}' AND Id_Productos = {Id_prod} AND id <> " +
                        $" (SELECT TOP 1 id FROM Ofertas WHERE Id_Sucursales = {suc} AND Fecha = '{nfecha:MM/dd/yyy}' AND Id_Productos = {Id_prod} ORDER BY Id DESC) ", sql);
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
            else
            {
                string s = DateTime.Today.ToString("dddd");
                escribirLog($"Estan ingresando un dato hoy {s}. Suc: {suc}, Prod: {Id_prod}, Kilos: {Kilos:N2}");
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

