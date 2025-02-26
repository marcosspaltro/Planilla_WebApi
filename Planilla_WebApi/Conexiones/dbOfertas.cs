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
        public IList<Ofertas>? Ofertas(int f_suc, DateTime dia)
        {
            
            sql.Open();
  
            string nFecha = $"'{dia:MM/dd/yyy}'";
            if (dia == DateTime.MinValue)
            {
                // No se proporcionó fecha
                nFecha = "(SELECT MAX(Semana) FROM Semanas)";
            }

            string cadena = $"SELECT '{dia:dd/MM/yyy}' Fecha, PO.Id_Productos, (SELECT pr.Nombre FROM Productos pr WHERE pr.Id=PO.Id_Productos) Nombre" +
                $", CASE PO.Descripcion " +
                $" WHEN '' THEN ' x Kg' " +
                $" WHEN NULL THEN ' x Kg'" +
                $" ELSE PO.Descripcion" +
                $" END " +
                $" AS Oferta" +
                $", PO.Costo Precio " +
                $", ISNULL((SELECT SUM(ISNULL(o.Kilos, 0)) FROM Ofertas o WHERE o.Fecha={nFecha} AND o.ID_Productos=PO.Id_Productos AND o.Descripcion LIKE '%' + PO.Descripcion AND o.ID_Sucursales={f_suc}), 0) Kilos " +
                $" FROM Precios_Ofertas PO" +
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

        public void Agregar_registro(int suc, int Id_prod, string oferta, double Kilos, DateTime fecha)
        {

            try
            {
                int viejoID = Max_ID("Ofertas");
                                
                Productos pr = new Productos();


                string cmdText = $"INSERT INTO Ofertas (Fecha, ID_Sucursales, ID_Productos, Descripcion, Costo_Original, Costo_Oferta, Kilos) " +
                                        $"VALUES('{fecha:MM/dd/yyy}', {suc}, {Id_prod}, CONCAT((SELECT TOP 1 Nombre FROM Productos WHERE Id = {Id_prod}), ' :: {oferta}') , " +
                                        $"(SELECT TOP 1 Precio FROM Precios_Sucursales WHERE Id_Sucursales = {suc} AND Id_Productos = {Id_prod} AND Fecha <= '{fecha:MM/dd/yyy}' ORDER BY Fecha DESC)" +
                                        $", isnull((SELECT TOP 1 ISNULL(po.Costo, 0) FROM Precios_Ofertas po WHERE po.Id_Productos={Id_prod} AND po.Descripcion LIKE '%{oferta}'), 0)" +
                                        $", {Math.Round(Kilos, 3).ToString().Replace(",", ".")})";

                // Para agregar el precio de la oferta
                //isnull((SELECT TOP 1 ISNULL(po.Costo, 0) FROM Precios_Ofertas po WHERE po.Id_Productos={Id_prod} AND po.Descripcion='{oferta}'), 0)                

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

                SqlCommand command = new SqlCommand($"DELETE FROM Ofertas WHERE Id_Sucursales = {suc} AND Descripcion like '%{oferta}%' AND Fecha = '{fecha:MM/dd/yyy}' AND Id_Productos = {Id_prod} AND id <> " +
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

