using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Planilla_WebApi.Services;
using Microsoft.IdentityModel.Tokens;
using Planilla_WebApi.Conexiones;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;

namespace Planilla_WebApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        public User user = new User();
        private readonly IConfiguration _configuration;
        private readonly IUserService _userService;

        public AuthController(IConfiguration configuration, IUserService userService)
        {
            _configuration = configuration;
            _userService = userService;
        }

        [HttpGet, Authorize]
        public ActionResult<string> GetMe()
        {
            var userName = _userService.GetMyName();
            return Ok(userName);
        }

      
        [HttpPost("login"), AllowAnonymous]
        public async Task<ActionResult<string>> Login(UserDto request)
        {
            const string error = "Error en el usuario o contraseña";
            var nombre = (request?.Username ?? "").Trim();
            var ip = HttpContext.Connection.RemoteIpAddress?.ToString() ?? "";

            // Demasiados intentos fallidos: se frena un rato ese usuario y esa dirección
            if (IntentosLogin.Bloqueado(nombre, ip))
            {
                return StatusCode(StatusCodes.Status429TooManyRequests,
                    "Demasiados intentos fallidos. Esperá unos minutos y probá de nuevo.");
            }

            dbUsuarios db = new dbUsuarios();
            IList<User>? usuaros = db.Usuarios();
            if (usuaros == null)
            {
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "No se pudo consultar los usuarios.");
            }

            var encontrado = usuaros.FirstOrDefault(x => string.Equals(x.Username, nombre, StringComparison.OrdinalIgnoreCase));

            // Mismo mensaje si el usuario no existe o la contraseña está mal
            if (encontrado == null || !VerifyPasswordHash(request?.Password ?? "", encontrado.PasswordHash, encontrado.PasswordSalt))
            {
                IntentosLogin.Fallido(nombre, ip);
                return BadRequest(error);
            }

            IntentosLogin.Correcto(nombre, ip);
            user = encontrado;
            var refreshToken = GenerateRefreshToken();
            SetRefreshToken(refreshToken);
            
            string a = CreateToken(user);            
            string [] token = { a, user.Username, user.Sucursal.ToString(), user.NombreSucursal, user.Nivel.ToString(), user.Semana.ToString()};


            return Ok(token);
        }

        [HttpPost("refresh-token"), AllowAnonymous]
        public async Task<ActionResult<string>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"];

            //if (!user.RefreshToken.Equals(refreshToken))
            //{
            //    return Unauthorized("Invalid Refresh Token.");
            //}
            //else
            if (user.TokenExpires < DateTime.Now)
            {
                return Unauthorized("Token expired.");
            }

            string token = CreateToken(user);
            var newRefreshToken = GenerateRefreshToken();
            SetRefreshToken(newRefreshToken);

            return Ok(token);
        }

        private RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(3),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken newRefreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);

            user.RefreshToken = newRefreshToken.Token;
            user.TokenCreated = newRefreshToken.Created;
            user.TokenExpires = newRefreshToken.Expires;
        }

        private string CreateToken(User user)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                _configuration.GetSection("AppSettings:Token").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(10),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512())
            {
                passwordSalt = hmac.Key;
                passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            }
        }

        private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
        {
            using (var hmac = new HMACSHA512(passwordSalt))
            {
                var computedHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
                return computedHash.SequenceEqual(passwordHash);
            }
        }
    }
}
