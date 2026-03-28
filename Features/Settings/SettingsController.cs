using AngularWithNET.ViewModel;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;

namespace AngularWithNET.Features.Settings
{
    [ApiController]
    [Route("api/settings")]
    public class SettingsController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly IConfiguration _configuration;

        public SettingsController(IOptions<AppSettings> appSettings, IConfiguration configuration)
        {
            _appSettings = appSettings.Value;
            _configuration = configuration;
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok(new AppSettingsDto
            {
                AppSettings1 = _appSettings.AppSettings1,
                AppSettings2 = _appSettings.AppSettings2,
                AppSettings3 = _appSettings.AppSettings3,
                AppSettings4 = _appSettings.AppSettings4,
                AppSettings5 = _appSettings.AppSettings5,
                IsDemoEnvironment = _configuration.GetValue<bool>("DemoMode", true)
            });
        }
    }

    public class AppSettingsDto
    {
        public string AppSettings1 { get; set; }
        public string AppSettings2 { get; set; }
        public string AppSettings3 { get; set; }
        public string AppSettings4 { get; set; }
        public string AppSettings5 { get; set; }
        public bool IsDemoEnvironment { get; set; }
    }
}
