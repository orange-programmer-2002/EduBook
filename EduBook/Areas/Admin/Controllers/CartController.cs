using Microsoft.AspNetCore.Mvc;

namespace EduBook.Areas.Admin.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}