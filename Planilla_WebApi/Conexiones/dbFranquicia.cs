using Planilla_WebApi.Modelos;
using System.Data;
using System.Data.SqlClient;

namespace Planilla_WebApi.Conexiones
{
    internal class dbFranquicia
    {
        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");        
        SqlConnection sql2 = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");        
        internal IList<Principal> Principal(DateTime fecha, int sucursal)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand($"SELECT * FROM Franquicia ORDER BY id", sql);

            cmd.CommandType = CommandType.Text;

            List<Principal> _Principal = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                sql2.Open();

                while (dr.Read())
                {
                    _Principal.Add(new()
                    {
                        id = Convert.ToInt32(dr["id"]),
                        Descripcion = dr["Descripcion"].ToString(),
                        Funcion = dr["Funcion"].ToString(),
                        Link = dr["Link"].ToString(),
                        Suma = Convert.ToBoolean(dr["Suma"])
                    });

                    if (_Principal[^1].Funcion != "" && _Principal[^1].Funcion != null)
                    {
                        SqlCommand cmdValor = new SqlCommand($"SELECT dbo.{_Principal[^1].Funcion} ({sucursal}, '{fecha:MM/dd/yyy}')", sql2);
                        cmdValor.CommandType = CommandType.Text;
                        _Principal[^1].Valor = Convert.ToSingle(cmdValor.ExecuteScalar());
                        if(_Principal[^1].Suma == false)
                        {
                            _Principal[^1].Valor = _Principal[^1].Valor * -1;
                        }
                    }
                }

                dr.Close();

                sql.Close();
                sql2.Close();

                return _Principal;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Principal = null;
                return _Principal;
            }
        }

        internal IList<frCarne> Carne(DateTime fecha, int sucursal)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("sp_frCarne", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@F", fecha);
            cmd.Parameters.AddWithValue("@Suc", sucursal);

            List<frCarne> _Datos = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                

                while (dr.Read())
                {
                    _Datos.Add(new()
                    {
                        Fecha = DateTime.Parse(dr["Fecha"].ToString()) ,
                        Producto = dr["Producto"].ToString(),
                        Costo = Convert.ToSingle(dr["Costo"]),
                        Kilos = Convert.ToSingle(dr["Kilos"])    ,                    
                        Total = Convert.ToSingle(dr["Total"])
                    });

                   
                }

                dr.Close();

                sql.Close();                

                return _Datos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Datos = null;
                return _Datos;
            }
        }

        internal IList<frMercaderiaSemana> MercadeiaSemana(DateTime fecha, int sucursal)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("sp_frMercaderia", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@F", fecha);
            cmd.Parameters.AddWithValue("@Suc", sucursal);
            cmd.Parameters.AddWithValue("@Resumen", 1);

            List<frMercaderiaSemana> _Datos = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();


                while (dr.Read())
                {
                    _Datos.Add(new()
                    {
                        Fecha = DateTime.Parse(dr["Fecha"].ToString()),
                        Tipo = Convert.ToInt32(dr["Tipo"]),
                        Descripcion = dr["Descripcion"].ToString(),
                        Items = Convert.ToInt32(dr["Items"]),
                        Kilos = Convert.ToSingle(dr["Kilos"]),
                        Total = Convert.ToSingle(dr["Total"])
                    });


                }

                dr.Close();

                sql.Close();

                return _Datos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Datos = null;
                return _Datos;
            }
        }

        internal IList<frMercaderiaSemana> GastosSemana(DateTime fecha, int sucursal)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("sp_frGastos", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@F", fecha);
            cmd.Parameters.AddWithValue("@Suc", sucursal);
            

            List<frMercaderiaSemana> _Datos = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();


                while (dr.Read())
                {
                    _Datos.Add(new()
                    {
                        Fecha = DateTime.Parse(dr["Fecha"].ToString()),
                        Tipo = Convert.ToInt32(dr["Id_Tipo"]),
                        Descripcion = dr["Descripcion"].ToString(),
                        Items = 0,
                        Kilos = 0,
                        Total = Convert.ToSingle(dr["Importe"])
                    });


                }

                dr.Close();

                sql.Close();

                return _Datos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Datos = null;
                return _Datos;
            }
        }

        internal IList<frMercaderiaSemana> DescartesSemana(DateTime fecha, int sucursal)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("sp_frDescartes", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@F", fecha);
            cmd.Parameters.AddWithValue("@Suc", sucursal);
            

            List<frMercaderiaSemana> _Datos = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();


                while (dr.Read())
                {
                    _Datos.Add(new()
                    {
                        Fecha = DateTime.Parse(dr["Fecha"].ToString()),
                        Tipo = Convert.ToInt32(dr["Tipo"]),
                        Descripcion = dr["Descripcion"].ToString(),
                        Items = 0,
                        Kilos = Convert.ToSingle(dr["Kilos"]),
                        Costo = Convert.ToSingle(dr["Costo"]),
                        Total = Convert.ToSingle(dr["Total"])
                    });


                }

                dr.Close();

                sql.Close();

                return _Datos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Datos = null;
                return _Datos;
            }
        }

        internal IList<Traslados> TrasladosSemana(DateTime fecha, int sucursal, bool salida = true)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("sp_frTraslados", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@F", fecha);
            cmd.Parameters.AddWithValue("@Suc", sucursal);
            cmd.Parameters.AddWithValue("@Salida", salida);
            

            List<Traslados> _Datos = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();


                while (dr.Read())
                {
                    _Datos.Add(new()
                    {
                        Fecha = DateTime.Parse(dr["Fecha"].ToString()),
                        Id_Tipo = Convert.ToInt32(dr["Tipo"]),
                        Nombre_Entrada = dr[3].ToString(),
                        Descripcion = dr["Descripcion"].ToString(),                        
                        Kilos = Convert.ToSingle(dr["Kilos"]),
                        Costo_Salida = Convert.ToSingle(dr["Costo"]),
                        Total_Salida = Convert.ToSingle(dr["Total"])
                    });


                }

                dr.Close();

                sql.Close();

                return _Datos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Datos = null;
                return _Datos;
            }
        }

        internal IList<frGastosOficina> GastosOficina(DateTime fecha, int sucursal)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("sp_frGastosOficina", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@F", fecha);
            cmd.Parameters.AddWithValue("@Suc", sucursal);
            
            List<frGastosOficina> _Datos = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    _Datos.Add(new()
                    {
                        Fecha = DateTime.Parse(dr["Fecha"].ToString()),
                        Descripcion = dr["Descripcion"].ToString(),
                        Importe = Convert.ToSingle(dr["Importe"])                        
                    });
                }
                dr.Close();
                sql.Close();
                return _Datos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Datos = null;
                return _Datos;
            }
        }
        
        internal IList<frGastosOficina> EfectivosSemana(DateTime fecha, int sucursal)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("sp_frEfectivos", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@F", fecha);
            cmd.Parameters.AddWithValue("@Suc", sucursal);
            
            List<frGastosOficina> _Datos = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    _Datos.Add(new()
                    {
                        Fecha = DateTime.Parse(dr["Fecha"].ToString()),
                        Descripcion = dr["Descripcion"].ToString(),
                        Importe = Convert.ToSingle(dr["Importe"])                        
                    });
                }
                dr.Close();
                sql.Close();
                return _Datos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Datos = null;
                return _Datos;
            }
        }

        public int AgregarEfectivo(Efectivo efectivo)
        {
            try
            {
                string cmdText = $"INSERT INTO Fecha_Entregas (Fecha, Suc, Importe) " +
                                 $"VALUES (@Fecha, @Suc, @Importe); " +
                                 "SELECT SCOPE_IDENTITY();";

                SqlCommand command = new SqlCommand(cmdText, sql);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@Suc", efectivo.Sucursal);
                command.Parameters.AddWithValue("@Fecha", efectivo.Fecha);
                command.Parameters.AddWithValue("@Importe", efectivo.Importe);

                command.Connection = sql;
                sql.Open();
                var id = command.ExecuteScalar();
                sql.Close();

                return Convert.ToInt32(id);
            }
            catch (Exception e)
            {
                
                if (sql.State == System.Data.ConnectionState.Open)
                    sql.Close();
                return 0;
            }
        }

        internal IList<frTarjetas> TarjetasSemana(DateTime fecha, int sucursal, bool agrupar)
        {
            // devuelve el total de tarjetas por dia, agrupando por descripcion
            sql.Open();
            SqlCommand cmd = new SqlCommand("sp_frTarjetas", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@F", fecha);
            cmd.Parameters.AddWithValue("@Suc", sucursal);
            cmd.Parameters.AddWithValue("@Agrupar", agrupar);
            List<frTarjetas> _Datos = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    _Datos.Add(new()
                    {
                        Fecha = DateTime.Parse(dr["Fecha"].ToString()),
                        Descripcion = dr["Nombre"].ToString(),
                        Importe = Convert.ToSingle(dr["Importe"])
                    });
                }
                dr.Close();
                sql.Close();
                return _Datos;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                _Datos = null;
                return _Datos;
            }
        }
    }
}