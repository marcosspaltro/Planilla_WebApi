namespace Planilla_WebApi
{
    public class User
    {
        public string Username { get; set; } = string.Empty;
        public int Nivel { get; set; }
        public int Sucursal { get; set; }
        public string NombreSucursal { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public int suc { get; set; }
        public string RefreshToken { get; set; } = string.Empty;
        public DateTime TokenCreated { get; set; }
        public DateTime TokenExpires { get; set; }
        public DateTime Semana { get; set; }
    }
}
