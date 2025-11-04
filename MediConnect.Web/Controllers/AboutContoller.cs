using Microsoft.AspNetCore.Mvc;

namespace MediConnect.Web.Controllers
{
    public class AboutController : Controller
    {
        public IActionResult Index() => View();
    }
}
