using Microsoft.AspNetCore.Mvc;

namespace lvl.Oidc.AuthorizationServer.Controllers
{
    public class LoginController: Controller
    {
        [HttpGet]
        public IActionResult Index() => View();
    }
}
