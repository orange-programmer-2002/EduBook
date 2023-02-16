using EduBook.DataAccess.Repository.IRepository;
using EduBook.Models;
using EduBook.Models.ViewModels;
using EduBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Data;

namespace EduBook.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _db;
        private readonly IWebHostEnvironment _hostEnvironment;

        public ProductController(IUnitOfWork db, IWebHostEnvironment hostEnvironment)
        {
            _db = db;
            _hostEnvironment = hostEnvironment;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new Product(),
                Categories = _db.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                }),
                CoverTypes = _db.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                })
            };
            if (id == null)
            {
                return View(productVM);
            }
            productVM.Product = _db.Product.Get(id.GetValueOrDefault());
            if (productVM.Product == null) 
            {
                return NotFound();
            }
            return View(productVM);
        }

        [HttpGet]
        public IActionResult GetAll()
        {
            var allObj = _db.Product.GetAll(includeProperties: "Category,CoverType");
            return Json(new { data = allObj });
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var objFromDb = _db.Product.Get(id);
            if (objFromDb == null)
            {
                return Json(new { success = false, message = "Error While Deleting!" });
            }
            string webRootPath = _hostEnvironment.WebRootPath;
            var imagePath = Path.Combine(webRootPath, objFromDb.ImageUrl.TrimStart('\\'));
            if (System.IO.File.Exists(imagePath))
            {
                System.IO.File.Delete(imagePath);
            }
            _db.Product.Remove(objFromDb);
            _db.Save();
            return Json(new { success = true, message = "Deleted Successfully" });

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(ProductVM productVM)
        {
            if (ModelState.IsValid)
            {
                string webRootPath = _hostEnvironment.WebRootPath;
                var files = HttpContext.Request.Form.Files;
                if (files.Count > 0)
                {
                    string fileName = Guid.NewGuid().ToString();
                    var uploads = Path.Combine(webRootPath, @"images\products");
                    var extension = Path.GetExtension(files[0].FileName);

                    if (productVM.Product.ImageUrl != null) 
                    {
                        var imagePath = Path.Combine(webRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(imagePath)) 
                        {
                            System.IO.File.Delete(imagePath);
                        }
                    }
                    using (var filesStreams = new FileStream(Path.Combine(uploads, fileName + extension), FileMode.Create))
                    {
                        files[0].CopyTo(filesStreams);
                    }
                    productVM.Product.ImageUrl = @"\images\products\" + fileName + extension;
                } 
                else
                {
                    if (productVM.Product.Id != 0) 
                    {
                        Product objFromDb = _db.Product.Get(productVM.Product.Id);
                        productVM.Product.ImageUrl = objFromDb.ImageUrl;
                    }
                }
                if (productVM.Product.Id == 0)
                {
                    _db.Product.Add(productVM.Product);
                    TempData["success"] = "Added Successfully";
                }
                else
                {
                    _db.Product.Update(productVM.Product);
                    TempData["success"] = "Updated Successfully";
                }
                _db.Save();
                return RedirectToAction(nameof(Index));
            }
            else 
            {
                productVM.Categories = _db.Category.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                productVM.CoverTypes = _db.CoverType.GetAll().Select(i => new SelectListItem
                {
                    Text = i.Name,
                    Value = i.Id.ToString()
                });
                if (productVM.Product.Id != 0) 
                {
                    productVM.Product = _db.Product.Get(productVM.Product.Id);
                }
            }
            return View(productVM);
        }
    }
}
