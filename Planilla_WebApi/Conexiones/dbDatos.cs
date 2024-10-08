﻿using Microsoft.AspNetCore.Identity;
using Planilla_WebApi.Modelos;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Security.Cryptography;

namespace Planilla_WebApi.Conexiones
{
    public class dbDatos
    {
        SqlConnection sql = new SqlConnection("Data Source=192.168.1.11;Initial Catalog=dbDatos;User Id=Nikorasu;Password=Oficina02");


        public class datamodel
        {
            public DateTime fecha { get; set; }
            public int suc { get; set; }
            public int id_prod { get; set; }
            public string desc { get; set; }
            public double costo { get; set; }
            public double kilos { get; set; }
        }


        public int Sucursal { get; set; }
        public string? Nombre { get; set; }
        public DateTime Fecha { get; set; }

        public dbDatos()
        {
            Sucursal = 1;
            Nombre = "LOS ALAMOS";
            Fecha = DateTime.Now;
        }

        public IList<Stock>? Stocks(int f_suc)
        {
            DateTime f = DateTime.Now;
            int rd = (int)f.DayOfWeek;

            if (rd < 3) {  f = f.AddDays(-rd); }
            else { f = f.AddDays(7 - rd); }

            sql.Open();
            SqlCommand cmd = new SqlCommand($"SELECT ID, Nombre AS Descripcion" +
                $", ISNULL((SELECT Kilos FROM Stock WHERE Id_Sucursales={f_suc} AND Fecha='{f:MM/dd/yyyy}' AND Id_Productos=Productos.Id), 0) " +
                $"AS Kilos" +
                $", {f_suc} AS ID_Sucursales, CONVERT(DATETIME, '{f:MM/dd/yyyy}') AS Fecha" +
                $", ISNULL((SELECT TOP 1 ID FROM Stock WHERE Id_Sucursales={f_suc} AND Fecha='{f:MM/dd/yyyy}' AND Id_Productos=Productos.Id), 0) " +
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

        internal void Actualizar(Stock value)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand($"DELETE FROM Stock WHERE Fecha='{Fecha:MM/dd/yyyy}' AND ID_Sucursales={Sucursal} AND ID_Productos={value.Producto}", sql);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            sql.Close();
            Agregar(value);
        }

        internal void Agregar(Stock value)
        {
            sql.Open();
            SqlCommand cmd = new SqlCommand($"INSERT INTO Stock (Fecha, ID_Sucursales, ID_Productos, Descripcion, Kilos) VALUES(" +
                $"'{Fecha:MM/dd/yyyy}', {Sucursal}, {value.Producto}, '{value.Descripcion}', {value.Kilos.ToString().Replace(",", ".")})", sql);
            cmd.CommandType = CommandType.Text;
            cmd.ExecuteNonQuery();
            sql.Close();
        }

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

        //public Stock Stocks(int prod)
        //{
        //    sql.Open();
        //    SqlCommand cmd = new SqlCommand($"SELECT ID, Nombre AS Descripcion" +
        //        $", ISNULL((SELECT Kilos FROM Stock WHERE Id_Sucursales={Sucursal} AND Fecha='{Fecha:MM/dd/yyyy}' AND Id_Productos=Productos.Id), 0) " +
        //        $"AS Kilos" +
        //        $", {Sucursal} AS ID_Sucursales, CONVERT(DATETIME, '{Fecha:MM/dd/yyyy}') AS Fecha" +
        //        $", ISNULL((SELECT TOP 1 ID FROM Stock WHERE Id_Sucursales={Sucursal} AND Fecha='{Fecha:MM/dd/yyyy}' AND Id_Productos=Productos.Id), 0) " +
        //        $"AS ID_Stock" +
        //        $", Id_Tipo" +
        //        $" FROM Productos WHERE Id={prod} ORDER BY Id_Tipo, Id", sql);
        //    cmd.CommandType = CommandType.Text;

        //    Stock _Stock = new();
        //    try
        //    {
        //        SqlDataReader dr = cmd.ExecuteReader();

        //        if (dr.Read())
        //        {
        //            _Stock.ID = Convert.ToInt32(dr["ID_Stock"]);
        //            _Stock.Fecha = Convert.ToDateTime(dr["Fecha"].ToString());
        //            _Stock.Sucursal = Convert.ToInt32(dr["ID_Sucursales"]);
        //            _Stock.Producto = Convert.ToInt32(dr["ID"]);
        //            _Stock.Tipo = Convert.ToInt32(dr["Id_Tipo"]);
        //            _Stock.Descripcion = dr["Descripcion"].ToString();
        //            _Stock.Kilos = Convert.ToSingle(dr["Kilos"]);
        //        }

        //        dr.Close();
        //        sql.Close();

        //        return _Stock;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //        _Stock = null;
        //        return _Stock;
        //    }
        //}

        public IList<User> Usuarios()
        {
            List<User> nlist = new();

            sql.Open();
            SqlCommand cmd = new SqlCommand("SELECT * FROM Usuarios_Web", sql);
            cmd.CommandType = CommandType.Text;
            
            try
            {
                SqlDataReader dr = cmd.ExecuteReader();

                while (dr.Read())
                {
                    User user = new User();

                    user.Username = dr["Usuario"].ToString();
                    CreatePasswordHash(dr["Contraseña"].ToString(), out byte[] passwordHash, out byte[] passwordSalt);
                    user.suc = Convert.ToInt32(dr["Sucursal"]);
                    user.PasswordHash = passwordHash;
                    user.PasswordSalt = passwordSalt;

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

        public void Agregar_registro(DateTime Fecha, int Suc, int Id_prod, double Kilos)
        {
            try
            {
                SqlCommand command =
                    new SqlCommand($"INSERT INTO Stock (Fecha, Id_Sucursales, Id_Productos, Descripcion, Costo, Kilos) " +
                        $"VALUES('{Fecha.ToString("MM/dd/yyy")}', {Suc}, {Id_prod}, (SELECT TOP 1 Nombre FROM Productos WHERE Id = {Id_prod}), " +
                        $"(SELECT TOP 1 Precio FROM Precios_Sucursales WHERE Id_Productos = {Id_prod} AND Fecha <= '{Fecha.ToString("MM/dd/yyy")}' ORDER BY Fecha DESC), {Math.Round(Kilos, 3).ToString().Replace(",", ".")})", sql);
                command.CommandType = CommandType.Text;
                command.Connection = sql;
                sql.Open();

                var d = command.ExecuteNonQuery();

                sql.Close();

            }
            catch (Exception e)
            {
            }

            try
            {
                SqlCommand command = new SqlCommand($"DELETE FROM Stock WHERE Id_Sucursales = {Suc} AND Fecha = '{Fecha.ToString("MM/dd/yyy")}' AND Id_Productos = {Id_prod} AND id <> " +
                    $" (SELECT TOP 1 id FROM Stock WHERE Id_Sucursales = {Suc} AND Fecha = '{Fecha.ToString("MM/dd/yyy")}' AND Id_Productos = {Id_prod} ORDER BY Id DESC) ", sql);
                command.CommandType = CommandType.Text;
                command.Connection = sql;
                sql.Open();

                var d = command.ExecuteNonQuery();

                sql.Close();
            }
            catch (Exception e)
            {
            }

        }

        public int Agregar_Buzones(DateTime Fecha, double Importe, int Suc)
        {
            int s = 0;
            try
            {
                SqlCommand command =
                    new SqlCommand($"INSERT INTO Fecha_Entregas (Fecha, Importe, Suc) " +
                        $"VALUES ('{Fecha.ToString("MM/dd/yyy")}', {Math.Round(Importe, 3).ToString().Replace(",", ".")}, {Suc})");
                command.CommandType = CommandType.Text;
                command.Connection = sql;
                sql.Open();

                var d = command.ExecuteNonQuery();

                sql.Close();

                s = Max_ID("Fecha_Entregas", "ID");

            }
            catch (Exception e)
            {
            }
            return s;

        }

        public void Eliminar_Buzones(DateTime fecha, int suc, int id)
        {
            try
            {
                SqlCommand command = new SqlCommand($"DELETE FROM Fecha_Entregas WHERE Id = {id} AND Fecha = '{Fecha.ToString("MM/dd/yyy")}' AND Suc = {suc}", sql);
                command.CommandType = CommandType.Text;
                command.Connection = sql;
                sql.Open();

                var d = command.ExecuteNonQuery();

                sql.Close();
            }
            catch (Exception e)
            {
            }
        }

        public int Max_ID(string tabla, string Campo_ID)
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

