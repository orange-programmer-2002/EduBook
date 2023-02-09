using EduBook.DataAccess.Repository.IRepository;
using EduBook.Models;
using EduBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EduBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin + "," + SD.Role_Employee)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork _db;

        public CompanyController(IUnitOfWork db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            Company company = new Company();
            if (id == null)
            {
                return View(company);
            }
            company = _db.Company.Get(id.GetValueOrDefault());
            if (company == null) 
            {
                return NotFound();
            }
            return View(company);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _db.Company.GetAll();
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _db.Company.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting!" });
            }
            _db.Company.Remove(objFromDb);
            _db.Save();
            return Json(new { success = true, message = "Deleted Successfully" });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company company)
        {
            if (ModelState.IsValid) 
            { 
                if (company.Id == 0) 
                {
                    _db.Company.Add(company);
                    TempData["success"] = "Added Successfully";
                } 
                else
                {
                    _db.Company.Update(company);
                    TempData["success"] = "Updated Successfully";
                }
                _db.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(company);
        }
    }
}