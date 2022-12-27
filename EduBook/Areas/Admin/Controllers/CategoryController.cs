using EduBook.DataAccess.Repository.IRepository;
using EduBook.Models;
using Microsoft.AspNetCore.Mvc;

namespace EduBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private readonly IUnitOfWork _db;

        public CategoryController(IUnitOfWork db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Category category = new Category();
            if (id == null)
            {
                return View(category);
            }
            category = _db.Category.Get(id.GetValueOrDefault());
            if (category == null) 
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _db.Category.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _db.Category.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting!" });
            }
            _db.Category.Remove(objFromDb);
            _db.Save();
            return Json(new { success = true, message = "Deleted Successfully" });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Category category)
        {
            if (ModelState.IsValid) 
            { 
                if (category.Id == 0) 
                {
                    _db.Category.Add(category);
                    TempData["success"] = "Added Successfully";
                } 
                else
                {
                    _db.Category.Update(category);
                    TempData["success"] = "Updated Successfully";
                }
                _db.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(category);
        }
    }
}
