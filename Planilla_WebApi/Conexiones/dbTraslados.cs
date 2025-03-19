using Planilla_WebApi.Modelos;
using System.Data;
using System.Data.SqlClient;

namespace Planilla_WebApi.Conexiones
{
    public class dbTraslados
    {
        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");

        public int Id;

        #region " Traslados "
        public IList<Traslados>? Traslados(int f_sucS, int f_sucE, DateTime fecha, int tipo)
        {
            string cadena = $"SELECT P.Id, P.Nombre Descripcion, ISNULL((SELECT SUM(V.Kilos) FROM vw_Traslados V WHERE V.Fecha='{fecha:MM/dd/yy}'" +
                $" AND v.Suc_Salida={f_sucS} AND v.Suc_Entrada={f_sucE}" +
                $" AND V.Id_Productos=P.Id), 0) Kilos " +
                $"FROM Productos P WHERE P.Id_Tipo={tipo} AND P.Ver=1 ORDER BY P.Id";

            sql.Open();
            SqlCommand cmd = new SqlCommand(cadena, sql);

            cmd.CommandType = CommandType.Text;

            List<Traslados> _Traslados = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    _Traslados.Add(new()
                    {
                        Id_Productos = Convert.ToInt32(dr["Id"]),
                        Descripcion = dr["Descripcion"].ToString(),
                        Id_Tipo = tipo,
                        Kilos = Convert.ToSingle(dr["Kilos"])
                    });
                }

                dr.Close();
                sql.Close();

                return _Traslados;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Traslados = null;
                return _Traslados;
            }
        }

        public void Agregar(Traslados traslado)
        {
            sql.Open();
            // primero borrar la traslado
            string cadena = $"DELETE FROM Traslados WHERE Fecha='{traslado.Fecha:MM/dd/yy}' AND Suc_Salida={traslado.Suc_Salida} AND Suc_Entrada={traslado.Suc_Entrada} AND Id_Productos={traslado.Id_Productos}";
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

            if (traslado.Kilos > 0)
            {
                Productos p = new();
                traslado.Descripcion = p.buscar_Descripcion(traslado.Id_Productos);

                cadena = $"INSERT INTO Traslados (Fecha, Suc_Salida, Suc_Entrada, Id_Productos, Descripcion, Kilos) " +
                        $"VALUES ('{traslado.Fecha:MM/dd/yy}', {traslado.Suc_Salida}, {traslado.Suc_Entrada}, {traslado.Id_Productos}, '{traslado.Descripcion}',  {Math.Round(traslado.Kilos, 3).ToString().Replace(",", ".")})";
                cmd = new SqlCommand(cadena, sql);
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
