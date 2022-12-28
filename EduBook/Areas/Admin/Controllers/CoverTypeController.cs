﻿using Dapper;
using EduBook.DataAccess.Repository.IRepository;
using EduBook.Models;
using EduBook.Utility;
using Microsoft.AspNetCore.Mvc;

namespace EduBook.Areas.Admin.Controllers
{
    [Area("Admin")]
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
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);
            coverType = _db.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter);
            if (coverType == null) 
            {
                return NotFound();
            }
            return View(coverType);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _db.SP_Call.List<CoverType>(SD.Proc_CoverType_GetAll, null);
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var parameter = new DynamicParameters();
            parameter.Add("@Id", id);
            var objFromDb = _db.SP_Call.OneRecord<CoverType>(SD.Proc_CoverType_Get, parameter);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting!" });
            }
            _db.SP_Call.Execute(SD.Proc_CoverType_Delete, parameter);
            _db.Save();
            return Json(new { success = true, message = "Deleted Successfully" });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(CoverType coverType)
        {
            if (ModelState.IsValid) 
            {
                var parameter = new DynamicParameters();
                parameter.Add("@Name", coverType.Name);
                if (coverType.Id == 0) 
                {
                    _db.SP_Call.Execute(SD.Proc_CoverType_Create, parameter);
                    TempData["success"] = "Added Successfully";
                } 
                else
                {
                    parameter.Add("@Id", coverType.Id);
                    _db.SP_Call.Execute(SD.Proc_CoverType_Update, parameter);
                    TempData["success"] = "Updated Successfully";
                }
                _db.Save();
                return RedirectToAction(nameof(Index));
            }
            return View(coverType);
        }
    }
}