using Planilla_WebApi.Modelos;
using System.Data;
using System.Data.SqlClient;

namespace Planilla_WebApi.Conexiones
{
    public class dbDatos
    {
        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos2;User Id=Nikorasu;Password=Oficina02");

        public int Sucursal { get; set; }
        public string? Nombre { get; set; }
        public DateTime Fecha { get; set; }

        public dbDatos()
        {
            Sucursal = 1;
            Nombre = "LOS ALAMOS";
            Fecha = new DateTime(2022, 3, 20);


        }

        public IList<Stock>? Stocks()
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand($"SELECT ID, Nombre AS Descripcion" +
                $", ISNULL((SELECT Kilos FROM Stock WHERE Id_Sucursales={Sucursal} AND Fecha='{Fecha:MM/dd/yyyy}' AND Id_Productos=Productos.Id), 0) " +
                $"AS Kilos" +
                $", {Sucursal} AS ID_Sucursales, CONVERT(DATETIME, '{Fecha:MM/dd/yyyy}') AS Fecha" +
                $", ISNULL((SELECT TOP 1 ID FROM Stock WHERE Id_Sucursales={Sucursal} AND Fecha='{Fecha:MM/dd/yyyy}' AND Id_Productos=Productos.Id), 0) " +
                $"AS ID_Stock" +
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
                        ID = Convert.ToInt32(dr["ID_Stock"]),
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
    }
}

