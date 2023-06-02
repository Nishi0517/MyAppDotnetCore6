using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAppDotnetCore6.DataAccessLayer.Infrastructure.IRepository;
using MyAppDotnetCore6.Models;
using MyAppDotnetCore6.Models.ViewModels;
using MyAppWebCore6.DataAccessLayer;

namespace MyAppWebCore6.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles =WebsiteRole.Roles_Admin)]
    [Authorize]
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
            categoryVM.categories = _unitOfWork.Category.GetAll();
            return View(categoryVM);
        }
        //[HttpGet]
        //public IActionResult Create()
        //{
        //    return View();
        //}

        //[HttpPost]
        ////[ValidateAntiForgeryToken]
        //public IActionResult Create(Category category)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _unitOfWork.Category.Add(category);
        //        _unitOfWork.Save();
        //        TempData["success"] = "Category Inserted SuccessFully!!";
        //        return RedirectToAction("Index");
        //    }
        //    return View(category);
        //}
        [HttpGet]
        public IActionResult CreateUpdate(int? id)
        {
            CategoryVM categoryVM = new CategoryVM();
            if (id == null || id == 0)
            {
                return View(categoryVM);
            }
            else
            {
                categoryVM.Category= _unitOfWork.Category.GetT(x => x.Id == id);
                if (categoryVM.Category == null)
                {
                    return NotFound();
                }
                else
                    return View(categoryVM);
            }
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult CreateUpdate(CategoryVM categoryVM)
        {
            if(categoryVM.Category.Id == 0)
            {
                _unitOfWork.Category.Add(categoryVM.Category);
                TempData["success"] = "Category Created SuccessFully!!";
            }
            else
            {
                _unitOfWork.Category.Update(categoryVM.Category);
                TempData["success"] = "Category Updated SuccessFully!!";
            }
            if (ModelState.IsValid)
            {
                _unitOfWork.Category.Update(categoryVM.Category);
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
            _unitOfWork.Category.Delete(category);
            _unitOfWork.Save();
            TempData["success"] = "Category Deleted SuccessFully!!";
            return RedirectToAction("Index");
        }
    }
}