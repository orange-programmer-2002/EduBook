using EduBook.DataAccess.Repository.IRepository;
using EduBook.Models;
using EduBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CoverTypeController : Controller
    {
        private readonly IUnitOfWork _db;

        public CoverTypeController(IUnitOfWork db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            CoverType coverType = new CoverType();
            if (id == null)
            {
                return View(coverType);
            }
            coverType = _db.CoverType.Get(id.GetValueOrDefault());
            if (coverType == null)
            {
                return NotFound();
            }
            return View(coverType);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _db.CoverType.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _db.CoverType.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting!" });
            }
            _db.CoverType.Remove(objFromDb);
            _db.Save();
            return Json(new { success = true, message = "Deleted Successfully" });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid)
            {
                if (coverType.Id == 0)
                {
                    _db.CoverType.Add(coverType);
                    TempData["success"] = "Added Successfully";
                }
                else
                {
                    _db.CoverType.Update(coverType);
                    TempData["success"] = "Updated Successfully";
                }
                _db.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }
    }
}
