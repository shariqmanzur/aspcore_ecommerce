using DataAccessLayer.Data;
using DataAccessLayer.Infrastructure.IRepository;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;

namespace Web.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoryController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public CategoryController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            CategoryVM categoryVM = new CategoryVM();
            categoryVM.Categories = _unitOfWork.Category.GetAll();
            return View(categoryVM);
        }

        [HttpGet]
        public IActionResult CreateEdit(int? id)
        {
            CategoryVM categoryVM = new CategoryVM();
            if (id == null || id == 0)
            {
                return View(categoryVM);
            }
            else
            {
                categoryVM.Category = _unitOfWork.Category.GetT(x => x.Id == id);
                if(categoryVM == null)
                {
                    return NotFound();
                }
                else
                {
                    return View(categoryVM);
                }
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult CreateEdit(CategoryVM categoryVM)
        {
            if (ModelState.IsValid)
            {
                if(categoryVM.Category.Id == 0)
                {
                    _unitOfWork.Category.Add(categoryVM.Category);
                    TempData["success"] = "Category Created Done!";
                }
                else
                {
                    _unitOfWork.Category.Update(categoryVM.Category);
                    TempData["success"] = "Category Updated Done!";
                }                
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }
            var category = _unitOfWork.Category.GetT(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public IActionResult DeleteData(int? id)
        {
            var category = _unitOfWork.Category.GetT(x => x.Id == id);
            if (category == null)
            {
                return NotFound();
            }
            _unitOfWork.Category.Delete(category);
            _unitOfWork.Save();
            TempData["success"] = "Category Deleted Done!";
            return RedirectToAction("Index");
        }
    }
}