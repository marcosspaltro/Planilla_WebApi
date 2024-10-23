using Planilla_WebApi.Modelos;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;

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
        public IList<Stock>? Stocks(int f_suc, int tipo = 0, string semana = "1/1/2000")
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
                $", ISNULL((SELECT Kilos FROM Stock WHERE Id_Sucursales={f_suc} AND Fecha='{f:MM/dd/yyyy}' AND Id_Productos=Productos.Id), 0) " +
                $"AS Kilos" +
                $", {f_suc} AS ID_Sucursales, CONVERT(DATETIME, '{f:MM/dd/yyyy}') AS Fecha" +
                $", ISNULL((SELECT TOP 1 ID FROM Stock WHERE Id_Sucursales={f_suc} AND Fecha='{f:MM/dd/yyyy}' AND Id_Productos=Productos.Id), 0) " +
                $"AS ID_Stock" +
                $", Id_Tipo" +
                $" FROM Productos WHERE Ver = 1 {sTipo} ORDER BY Id_Tipo, Id", sql);
            cmd.CommandType = CommandType.Text;

            List<Stock> _Stocks = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    _Stocks.Add(new()
                    {
                        Fecha = Convert.ToDateTime(dr["Fecha"].ToString()),
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

        #endregion

        //#region " MERCADERIA "
        //internal IList<Mercaderia> Mercaderia(int suc, int tipo, string fecha1, string fecha2)
        //{
        //    DateTime f = DateTime.Now;
        //    int rd = (int)f.DayOfWeek;

        //    if (fecha1 == "1/1/2000")
        //    {
        //        if (rd < 3) { f = f.AddDays(-rd); }
        //        else { f = f.AddDays(7 - rd); }
        //    }
        //    else
        //    {
        //        f = DateTime.Parse(fecha1);
        //    }

        //    string sTipo = "";

        //    if (tipo != 0)
        //    {
        //        sTipo = " AND Id_Tipo=" + tipo.ToString();
        //    }
        //    sql.Open();
        //    SqlCommand cmd = new SqlCommand($"SELECT ID, Nombre AS Descripcion" +
        //        $", ISNULL((SELECT Kilos FROM Stock WHERE Id_Sucursales={suc} AND Fecha='{f:MM/dd/yyyy}' AND Id_Productos=Productos.Id), 0) " +
        //        $"AS Kilos" +
        //        $", {suc} AS ID_Sucursales, CONVERT(DATETIME, '{f:MM/dd/yyyy}') AS Fecha" +
        //        $", ISNULL((SELECT TOP 1 ID FROM Stock WHERE Id_Sucursales={suc} AND Fecha='{f:MM/dd/yyyy}' AND Id_Productos=Productos.Id), 0) " +
        //        $"AS ID_Stock" +
        //        $", Id_Tipo" +
        //        $" FROM Productos WHERE Ver = 1 {sTipo} ORDER BY Id_Tipo, Id", sql);
        //    cmd.CommandType = CommandType.Text;

        //    List<Mercaderia> _Mercaderia = new();
        //    try
        //    {
        //        SqlDataReader dr = cmd.ExecuteReader();

        //        while (dr.Read())
        //        {
        //            _Mercaderia.Add(new()
        //            {
        //                //ID = Convert.ToInt32(dr["ID_Stock"]),
        //                Fecha = Convert.ToDateTime(dr["Fecha"].ToString()),
        //                Sucursal = Convert.ToInt32(dr["ID_Sucursales"]),
        //                Producto = Convert.ToInt32(dr["ID"]),
        //                //Tipo = Convert.ToInt32(dr["Id_Tipo"]),
        //                Descripcion = dr["Descripcion"].ToString(),
        //                Kilos = Convert.ToSingle(dr["Kilos"])
        //            });
        //        }

        //        dr.Close();
        //        sql.Close();

        //        return _Mercaderia;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        _Mercaderia = null;
        //        return _Mercaderia;
        //    }
        //}

        //#endregion
        public int cant_prods()
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand($"SELECT COUNT(ID) FROM Productos WHERE ver=1 ", sql);
            cmd.CommandType = CommandType.Text;

            int d = 0;

            try
            {
                SqlDataAdapter daAdapt = new SqlDataAdapter(cmd);
                d = (int)cmd.ExecuteScalar();
                sql.Close();
            }
            catch (Exception ex)
            {
            }

            return d;
        }


        public IList<User> Usuarios()
        {
            List<User> nlist = new();

            sql.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM UsuariosWeb", sql);
            cmd.CommandType = CommandType.Text;

            try
            {
                SqlDataReader dr = cmd.ExecuteReader();

                DateTime semana = DateTime.Today;

                int daysToSubtract = (int)semana.DayOfWeek;

                daysToSubtract -= 1;

                // Si es antes del lunes (e.g. domingo o martes), restar los días necesarios
                semana = semana.AddDays(daysToSubtract * -1);
                //semana.AddDays(-1);


                while (dr.Read())
                {
                    User user = new User();
                    user.Nivel = (int)dr["Nivel"];
                    user.Username = dr["Usuario"].ToString();
                    user.Sucursal = (int)dr["Sucursal"];
                    user.NombreSucursal = dr["NombreSucursal"].ToString();
                    CreatePasswordHash(dr["Contraseña"].ToString(), out byte[] passwordHash, out byte[] passwordSalt);
                    user.suc = Convert.ToInt32(dr["Sucursal"]);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;
                    user.Semana = semana;
                    nlist.Add(user);
                }

                dr.Close();
                sql.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                nlist = null;
            }

            return nlist;
        }


        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        public void Agregar_registro(string fechaStock, int suc, int Id_prod, double Kilos)
        {

            // Chequear que la fecha no sea de la semana anterior
            DateTime nfecha = DateTime.Parse(fechaStock);

            nfecha = AjustarFechaSiEsDeSemanaAnterior(nfecha);
            try
            {
                int viejoID = Max_ID("Stock");

                string cmdText = $"INSERT INTO Stock (Fecha, Id_Sucursales, Id_Productos, Descripcion, Costo, Kilos) " +
                                        $"VALUES('{nfecha:MM/dd/yyy}', {suc}, {Id_prod}, (SELECT TOP 1 Nombre FROM Productos WHERE Id = {Id_prod}), " +
                                        $"(SELECT TOP 1 Precio FROM Precios_Sucursales WHERE Id_Sucursales = {suc} AND Id_Productos = {Id_prod} AND Fecha <= '{nfecha:MM/dd/yyy}' ORDER BY Fecha DESC)" +
                                        $", {Math.Round(Kilos, 3).ToString().Replace(",", ".")})";

                SqlCommand command = new SqlCommand(cmdText, sql);
                command.CommandType = CommandType.Text;
                command.Connection = sql;
                sql.Open();

                var d = command.ExecuteNonQuery();

                sql.Close();

                Id = Max_ID("Stock");

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

                SqlCommand command = new SqlCommand($"DELETE FROM Stock WHERE Id_Sucursales = {suc} AND Fecha = '{nfecha:MM/dd/yyy}' AND Id_Productos = {Id_prod} AND id <> " +
                    $" (SELECT TOP 1 id FROM Stock WHERE Id_Sucursales = {suc} AND Fecha = '{nfecha:MM/dd/yyy}' AND Id_Productos = {Id_prod} ORDER BY Id DESC) ", sql);
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

        public DateTime AjustarFechaSiEsDeSemanaAnterior(DateTime fecha)
        {
            // Obtener el día actual de la semana (0 = domingo, 1 = lunes, ..., 6 = sábado)
            DayOfWeek diaActualSemana = DateTime.Now.DayOfWeek;

            // Obtener el lunes de la semana actual
            DateTime lunesDeEstaSemana = DateTime.Now.AddDays(-(int)diaActualSemana + (diaActualSemana == DayOfWeek.Sunday ? -6 : 1));

            // Obtener el lunes de la semana anterior
            DateTime lunesDeSemanaAnterior = lunesDeEstaSemana.AddDays(-7);

            // Verificar si la fecha dada está entre el lunes y el domingo de la semana anterior
            if (fecha >= lunesDeSemanaAnterior && fecha < lunesDeEstaSemana)
            {
                // Si la fecha pertenece a la semana anterior, devolver el lunes de la semana actual
                return lunesDeEstaSemana;
            }

            // Si la fecha no es de la semana anterior, devolver la fecha original
            return fecha;
        }

        public void escribirLog(string e)
        {
            SqlCommand command = new SqlCommand($"INSERT INTO logApi (fecha, texto) VALUES(null, '{e}')", sql);
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

