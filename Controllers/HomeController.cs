using Microsoft.AspNetCore.Mvc;

namespace ASPNETCoreChatGPT.Controllers
{
    
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IConfiguration _configuration;
        public HomeController(ILogger<HomeController> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }


        [HttpGet]
        public async Task<IActionResult> IntroductoryOption()
        {
            var options = _configuration.GetSection("OptionsDoWork").Get<string[]>();

            return Json(options);
        }


        public IActionResult Index()
        {
            return View();
        }
    }
}
