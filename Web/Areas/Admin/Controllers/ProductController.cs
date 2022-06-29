using DataAccessLayer.Data;
using DataAccessLayer.Infrastructure.IRepository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Models;
using Models.ViewModels;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private IUnitOfWork _unitOfWork;
        private IWebHostEnvironment _webHostEnvironment;

        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
            _webHostEnvironment = webHostEnvironment;
        }

        #region APICALL

        public IActionResult AllProducts()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return Json(new { data = products });
        }

        #endregion

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        public IActionResult CreateEdit(int? id)
        {
            ProductVM productVM = new ProductVM()
            {
                Product = new(),
                Categories = _unitOfWork.Category.GetAll().Select(x =>
                new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                })
            };
            if (id == null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.Product.GetT(x => x.Id == id);
                if(productVM == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(productVM);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEdit(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string fileName = String.Empty;
                if(file != null)
                {
                    string uploadDir = Path.Combine(_webHostEnvironment.WebRootPath, "content/ProductImages");
                    fileName = Guid.NewGuid().ToString() + "-" + file.FileName;
                    string filePath = Path.Combine(uploadDir, fileName);

                    if(productVM.Product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if (System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\content\ProductImages\" + fileName;
                }
                if(productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Product Created Done!";
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                    TempData["success"] = "Product Updated Done!";
                }
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        #region DeleteAPICALL
        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var product = _unitOfWork.Product.GetT(x => x.Id == id);
            if(product == null)
            {
                return Json(new { success = false, message = "Error in Fetching Data" });
            }
            else
            {
                var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                {
                    System.IO.File.Delete(oldImagePath);
                }
                _unitOfWork.Product.Delete(product);
                _unitOfWork.Save();
                return Json(new { success = true, message = "Product Delete Done!" });
            }
        }

        #endregion
    }
}