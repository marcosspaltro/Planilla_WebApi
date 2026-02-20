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
                        if (_Principal[^1].Suma == false)
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

        internal IList<frCarne> Carne(DateTime fecha, int sucursal, bool semana = true)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("sp_frCarne", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@F", fecha);
            cmd.Parameters.AddWithValue("@Suc", sucursal);
            cmd.Parameters.AddWithValue("@Semana", semana);

            List<frCarne> _Datos = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();


                while (dr.Read())
                {
                    _Datos.Add(new()
                    {
                        Fecha = DateTime.Parse(dr["Fecha"].ToString()),
                        Producto = dr["Producto"].ToString(),
                        Costo = Convert.ToSingle(dr["Costo"]),
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

        internal IList<frMercaderiaSemana> MercaderiaSemana(DateTime fecha, int sucursal)
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

        internal IList<frMercaderiaSemana> MercaderiaDia(DateTime fecha, int sucursal, int tipo, bool semana = true, bool ceros = true)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("sp_frMercaderia", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@F", fecha);
            cmd.Parameters.AddWithValue("@Suc", sucursal);
            cmd.Parameters.AddWithValue("@Resumen", 0);
            cmd.Parameters.AddWithValue("@Tipo", tipo);
            cmd.Parameters.AddWithValue("@Semana", semana);
            cmd.Parameters.AddWithValue("@Ceros", ceros);

            List<frMercaderiaSemana> _Datos = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();


                while (dr.Read())
                {
                    _Datos.Add(new()
                    {
                        ID = Convert.ToInt32(dr["ID"]),
                        ID_Producto = Convert.ToInt32(dr["ID_Producto"]),
                        Fecha = DateTime.Parse(dr["Fecha"].ToString()),
                        Tipo = Convert.ToInt32(dr["Tipo"]),
                        Descripcion = dr["Producto"].ToString(),
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

        internal IList<frMercaderiaSemana> GastosSemana(DateTime fecha, int sucursal, bool dia = false)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("sp_frGastos", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@F", fecha);
            cmd.Parameters.AddWithValue("@Suc", sucursal);
            cmd.Parameters.AddWithValue("@Dia", dia);

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
        internal IList<Traslados> TrasladosDia(DateTime fecha, int sucursal, bool salida = true)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("sp_frTraslados", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@F", fecha);
            cmd.Parameters.AddWithValue("@Suc", sucursal);
            cmd.Parameters.AddWithValue("@Salida", salida);
            cmd.Parameters.AddWithValue("@dia", true);


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

        internal IList<frGastosOficina> GastosOficina(DateTime fecha, int sucursal, bool dia = false)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("sp_frGastosOficina", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@F", fecha);
            cmd.Parameters.AddWithValue("@Suc", sucursal);
            cmd.Parameters.AddWithValue("@Dia", dia);

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

        internal IList<Stock> Stock(DateTime fecha, int sucursal)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("sp_frStock", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@F", fecha);
            cmd.Parameters.AddWithValue("@Suc", sucursal);

            List<Modelos.Stock> _Datos = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    _Datos.Add(new()
                    {
                        Fecha = DateTime.Parse(dr["Fecha"].ToString()),
                        Producto = Convert.ToInt32(dr["Producto"]),
                        Descripcion = dr["Descripcion"].ToString(),
                        Kilos = Convert.ToSingle(dr["Kilos"]),
                        Tipo = Convert.ToInt32(dr["Tipo"]),
                        Precio = Convert.ToSingle(dr["Precio"])
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

        //Listado de Tipo_Entrega
        internal IList<Tipo_Entrega> TiposEntregas()
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM [dbGastos].[dbo].[Tipo_Cuentas]", sql);
            cmd.CommandType = CommandType.Text;
            List<Tipo_Entrega> _Datos = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    _Datos.Add(new()
                    {
                        ID = Convert.ToInt32(dr["Id"]),
                        Nombre = dr["Nombre"].ToString()
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

        // Listado de Entregas por fecha y sucursal
        internal IList<Entrega> Entregas(DateTime fecha, int sucursal, int tipo)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("dbGastos.dbo.sp_frEntregas", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@F", fecha);
            cmd.Parameters.AddWithValue("@S", sucursal);
            cmd.Parameters.AddWithValue("@T", tipo);

            List<Entrega> _Datos = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    _Datos.Add(new()
                    {
                        ID = Convert.ToInt32(dr["ID"]),
                        Fecha = DateTime.Parse(dr["Fecha"].ToString()),
                        Sucursal = Convert.ToInt32(dr["Suc"]),
                        Tipo = Convert.ToInt16(dr["Tipo"]),
                        Descripcion = dr["Nombre"].ToString(),
                        Importe = Convert.ToSingle(dr["Importe"]),
                        Aprobado = Convert.ToBoolean(dr["Aprobado"])
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

        internal IList<Entrega> Entregas_semana(DateTime fecha, int sucursal)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand("dbGastos.dbo.sp_frEntregas_Semana", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@F", fecha);
            cmd.Parameters.AddWithValue("@S", sucursal);
            List<Entrega> _Datos = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    _Datos.Add(new()
                    {
                        ID = 0,
                        Fecha = Convert.ToDateTime(dr["Fecha"]),
                        Cantidad = Convert.ToInt32(dr["Cantidad"]),
                        Sucursal = sucursal,
                        Tipo = Convert.ToInt16(dr["Tipo"]),
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

        internal int Nueva_Entrega(Entrega entrega)
        {
            // Agrega una nueva entrega y devuelve el ID generado
            try
            {
                string cmdText = $"INSERT INTO [dbGastos].[dbo].[Entregas_Franquicia] (Fecha, Suc, Tipo, Importe) " +
                                 $"VALUES (@Fecha, @Suc, @Tipo, @Importe); " +
                                 "SELECT SCOPE_IDENTITY();";
                SqlCommand command = new SqlCommand(cmdText, sql);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@Fecha", entrega.Fecha);
                command.Parameters.AddWithValue("@Suc", entrega.Sucursal);
                command.Parameters.AddWithValue("@Tipo", entrega.Tipo);                
                command.Parameters.AddWithValue("@Importe", entrega.Importe);                
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

        internal bool Eliminar_Entrega(int id)
        {
            // Elimina una entrega por ID
            try
            {
                string cmdText = $"DELETE FROM [dbGastos].[dbo].[Entregas_Franquicia] WHERE ID = @ID";
                SqlCommand command = new SqlCommand(cmdText, sql);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ID", id);
                command.Connection = sql;
                sql.Open();
                int rowsAffected = command.ExecuteNonQuery();
                sql.Close();
                return rowsAffected > 0;
            }
            catch (Exception e)
            {
                if (sql.State == System.Data.ConnectionState.Open)
                    sql.Close();
                return false;
            }
        }

        internal bool Actualizar_Entrega(int id, float importe)
        {
            // Actualiza una entrega por ID
            try
            {
                string cmdText = $"UPDATE [dbGastos].[dbo].[Entregas_Franquicia] SET Importe = @Importe WHERE ID = @ID";
                SqlCommand command = new SqlCommand(cmdText, sql);
                command.CommandType = CommandType.Text;
                command.Parameters.AddWithValue("@ID", id);
                command.Parameters.AddWithValue("@Importe", importe);
                command.Connection = sql;
                sql.Open();
                int rowsAffected = command.ExecuteNonQuery();
                sql.Close();
                return rowsAffected > 0;
            }
            catch (Exception e)
            {
                if (sql.State == System.Data.ConnectionState.Open)
                    sql.Close();
                return false;
            }
        }

        internal IList<CuentaCorriente> CuentaCorriente(int suc, DateTime semana)
        {
            // Consulta el sp_CCTE_Franquicia con la sucursal y semana indicada, devuelve una lista de movimientos de cuenta corriente
            sql.Open();
            SqlCommand cmd = new SqlCommand("dbGastos.dbo.sp_CCTE_Franquicia", sql);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@F", semana);
            cmd.Parameters.AddWithValue("@S", suc);
            List<CuentaCorriente> _Datos = new();
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();
                while (dr.Read())
                {
                    _Datos.Add(new()
                    {
                        ID = Convert.ToInt32(dr["ID"]),
                        Fecha = Convert.ToDateTime(dr["Fecha"]),                                                                        
                        Descripcion = dr["Descripcion"].ToString(),
                        Importe = Convert.ToSingle(dr["Importe"]),
                        Entrada = Convert.ToBoolean(dr["Entrada"]),
                        Tipo = Convert.ToInt16(dr["Tipo"])

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