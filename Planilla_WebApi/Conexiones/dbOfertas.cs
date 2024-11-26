using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
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
        public IList<Ofertas>? Ofertas(int f_suc, DateTime fecha)
        {

            sql.Open();
            string cadena = $"SELECT p.ID_Productos" +
                            $", LEFT(Descripcion, CHARINDEX('::', Descripcion) - 1) AS Nombre" +
                            $", ISNULL(SUBSTRING(Descripcion, CHARINDEX('::', Descripcion) + 2, LEN(Descripcion)), 'x kg') AS Oferta" +
                            $", Kilos" +
                            $" FROM vw_Ofertas p " +
                            $" WHERE P.Fecha='{fecha:MM/dd/yyy}' AND p.id_Sucursales={f_suc}";

            string nFecha = $"'{fecha:MM/dd/yyy}'";
            if (fecha == DateTime.MinValue)
            {
                // No se proporcionó fecha
                nFecha = "(SELECT MAX(Semana) FROM Semanas)";
            }
            
            cadena = $"SELECT PO.Fecha, PO.Id_Productos, (SELECT pr.Nombre FROM Productos pr WHERE pr.Id=PO.Id_Productos) Nombre" +
                $", PO.Oferta, PO.Precio" +
                $", ISNULL((SELECT SUM(ISNULL(o.Kilos, 0)) FROM Ofertas o WHERE o.Fecha={nFecha} AND o.ID_Productos=PO.Id_Productos AND o.ID_Sucursales=PO.Id_Sucursales), 0) Kilos " +
                $" FROM Precios_OfertasSucursales PO" +
                $" WHERE PO.Fecha=(SELECT MAX(p.Fecha) FROM Precios_OfertasSucursales p WHERE p.Id_Sucursales={f_suc} AND p.Fecha<={nFecha}) AND PO.Id_Sucursales={f_suc}" +
                $" ORDER BY PO.Id_Productos";

            SqlCommand cmd = new SqlCommand(cadena, sql);
            cmd.CommandType = CommandType.Text;

            List<Ofertas> _Ofertas = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    _Ofertas.Add(new()
                    {
                        id_sucursal = f_suc,
                        id_productos = Convert.ToInt32(dr["Id_Productos"]),
                        descripcion = dr["Nombre"].ToString(),
                        oferta = dr["Oferta"].ToString(),
                        precio = Convert.ToSingle(dr["Precio"]),
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

        public void Agregar_registro(int suc, int Id_prod, double Kilos, DateTime fecha)
        {

            try
            {
                int viejoID = Max_ID("Ofertas");

                // Variable para generar la descripcion con la oferta
                string descripcion = "";

                Productos pr = new Productos();


                string cmdText = $"INSERT INTO Ofertas (Fecha, ID_Sucursales, ID_Productos, Descripcion, Costo_Original, Costo_Oferta, Kilos) " +
                                        $"VALUES('{fecha:MM/dd/yyy}', {suc}, {Id_prod}, (SELECT TOP 1 Nombre FROM Productos WHERE Id = {Id_prod}), " +
                                        $"(SELECT TOP 1 Precio FROM Precios_Sucursales WHERE Id_Sucursales = {suc} AND Id_Productos = {Id_prod} AND Fecha <= '{fecha:MM/dd/yyy}' ORDER BY Fecha DESC)" +
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

                SqlCommand command = new SqlCommand($"DELETE FROM Ofertas WHERE Id_Sucursales = {suc} AND Fecha = '{fecha:MM/dd/yyy}' AND Id_Productos = {Id_prod} AND id <> " +
                    $" (SELECT TOP 1 id FROM Ofertas WHERE Id_Sucursales = {suc} AND Fecha = '{fecha:MM/dd/yyy}' AND Id_Productos = {Id_prod} ORDER BY Id DESC) ", sql);
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

