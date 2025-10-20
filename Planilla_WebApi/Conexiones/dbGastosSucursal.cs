using Planilla_WebApi.Modelos;
using System.Data;
using System.Data.SqlClient;

namespace Planilla_WebApi.Conexiones
{
    public class dbGastosSucursal
    {

        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");

        public int Id;

        public dbGastosSucursal()
        {

        }
        #region " Gastos "
        public IList<gastosSucursales>? Gastos(int f_suc)
        {
            DateTime f = DateTime.Today;
            
            sql.Open();
            SqlCommand cmd = new SqlCommand($"SELECT Id, Id_Tipo Tipo, Descripcion, Importe FROM Gastos_Sucursales WHERE Fecha='{f:MM/dd/yyy}' AND ID_Sucursales={f_suc} ORDER BY Id", sql);
            
            cmd.CommandType = CommandType.Text;

            List<gastosSucursales> _Gastos = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    _Gastos.Add(new()
                    {
                        Id = Convert.ToInt32(dr["ID"]),
                        Tipo = Convert.ToInt32(dr["Tipo"]),
                        Descripcion = dr["Descripcion"].ToString(),
                        Monto = Convert.ToSingle(dr["Importe"])
                    });
                }

                dr.Close();
                sql.Close();

                return _Gastos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Gastos = null;
                return _Gastos;
            }
        }

        public int AgregarGasto(gastosSucursales gasto)
        {
            try
            {
                string cmdText = $"INSERT INTO Gastos_Sucursales (ID_Sucursales, Fecha, Id_Tipo, Descripcion, Importe) " +
                                 $"VALUES (@Id_Sucursales, @Fecha, @Tipo, @Descripcion, @Monto); " +
                                 "SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(cmdText, sql);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@Id_Sucursales", gasto.Id_Sucursales);
                command.Parameters.AddWithValue("@Fecha", gasto.Fecha);
                command.Parameters.AddWithValue("@Tipo", gasto.Tipo);
                command.Parameters.AddWithValue("@Descripcion", gasto.Descripcion);
                command.Parameters.AddWithValue("@Monto", gasto.Monto);

                command.Connection = sql;
                sql.Open();
                var id = command.ExecuteScalar();
                sql.Close();

                return Convert.ToInt32(id);
            }
            catch (Exception e)
            {
                escribirLog(e.Message);
                if (sql.State == System.Data.ConnectionState.Open)
                    sql.Close();
                return 0;
            }
        }

        public bool BorrarGasto(int id)
        {
            try
            {
                string cmdText = "DELETE FROM Gastos_Sucursales WHERE Id = @Id";
                SqlCommand command = new SqlCommand(cmdText, sql);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@Id", id);

                command.Connection = sql;
                sql.Open();
                int rowsAffected = command.ExecuteNonQuery();
                sql.Close();

                return rowsAffected > 0;
            }
            catch (Exception e)
            {
                escribirLog(e.Message);
                if (sql.State == System.Data.ConnectionState.Open)
                    sql.Close();
                return false;
            }
        }

        public void Agregar_registro(int suc, int Id_prod, double Kilos)
        {
            string nfecha = "DATEADD(DAY, -1, (SELECT MAX(Semana) FROM Semanas))";


            try
            {
                formulas f = new formulas();
                int viejoID = f.Max_ID("Stock");

                string cmdText = $"INSERT INTO Stock (Fecha, Id_Sucursales, Id_Productos, Descripcion, Costo, Kilos) " +
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

                SqlCommand command = new SqlCommand($"DELETE FROM Stock WHERE Id_Sucursales = {suc} AND Fecha = {nfecha} AND Id_Productos = {Id_prod} AND id <> " +
                    $" (SELECT TOP 1 id FROM Stock WHERE Id_Sucursales = {suc} AND Fecha = {nfecha} AND Id_Productos = {Id_prod} ORDER BY Id DESC) ", sql);
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
        internal IList<tipoGastosSucursales> TiposGastos()
        {
            
            SqlCommand cmd = new SqlCommand("SELECT Id, Nombre FROM GastosSucursales_Tipos WHERE LTRIM(Nombre)<>'' ORDER BY Nombre", sql);
            cmd.CommandType = CommandType.Text;
            cmd.Connection = sql;
            sql.Open();
            
            List<tipoGastosSucursales> _Tipos = new List<tipoGastosSucursales>();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    _Tipos.Add(new tipoGastosSucursales()
                    {
                        Id = Convert.ToInt32(dr["Id"]),
                        Descripcion = dr["Nombre"].ToString(),
                        
                    });
                }
                dr.Close();
                sql.Close();
                return _Tipos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Tipos = null;
                return _Tipos;
                
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

    }
}
