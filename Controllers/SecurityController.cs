using AngularWithNET.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace AngularWithNET.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SecurityController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<SecurityController> _logger;
        private readonly AppSettings _appSettings;

        public SecurityController(ILogger<SecurityController> logger, IOptions<AppSettings> appSettings)
        {
            _logger = logger;
            _appSettings = appSettings.Value;
        }

        [HttpGet("forcast")]
        [Authorize]
        public IEnumerable<WeatherForecast> GetForecast()
        {
            _logger.LogInformation($"Get Forecast Called");
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            })
            .ToArray();
        }

        [HttpPost("login")]
        public IActionResult Login(LoginDetails loginDetails)
        {
            if ((loginDetails.UserName == "admin" && loginDetails.Password == "admin") ||
                (loginDetails.UserName == "user" && loginDetails.Password == "user")
                ) {
                var userDetail = new UserViewModel()
                {
                    Username = loginDetails.UserName,
                    Token = GenerateJwtToken(loginDetails),
                    Permissions = GetPermissions(loginDetails.UserName)
                };

                _logger.LogInformation($"User: {loginDetails.UserName}, Login successful");

                return Ok(userDetail);
            }
            else
            {
                _logger.LogError($"User: {loginDetails.UserName}, Login failed");
                return BadRequest("User Id or Password Invalid!!!");
            }
        }

        [HttpGet("getSettings")]
        public IActionResult GetSettings()
        {
            return Ok(GetAppSettings());
        }


        private string GenerateJwtToken(LoginDetails loginDetails)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[] {
                    new Claim("Id", loginDetails.UserName),
                }),
                Expires = DateTime.UtcNow.AddMinutes(_appSettings.TokenExpiry == 0 ? 180 : _appSettings.TokenExpiry),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string[] GetPermissions(string userName)
        {
            var permissions = new List<string> ();

            if (userName == "admin")
            {
                permissions.Add("ViewCounter");
                permissions.Add("ViewForecast");
                permissions.Add("ViewAppSettings");
            }
            else
            {
                permissions.Add("ViewCounter");
            }

            return permissions.ToArray();
        }

        private AppSettingsViewModel GetAppSettings()
        {
            return new AppSettingsViewModel()
            {
                AppSettings1 = _appSettings.AppSettings1,
                AppSettings2 = _appSettings.AppSettings2,
                AppSettings3 = _appSettings.AppSettings3,
                AppSettings4 = _appSettings.AppSettings4,
                AppSettings5 = _appSettings.AppSettings5,
            };
        }
    }
}
