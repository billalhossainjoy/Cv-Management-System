using Microsoft.AspNetCore.Mvc;

namespace CVMS.Web.Controllers;

public class ProfileController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}