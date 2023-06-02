using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using MyAppDotnetCore6.CommonHelper;
using MyAppDotnetCore6.DataAccessLayer.Infrastructure.IRepository;
using MyAppDotnetCore6.Models;
using MyAppDotnetCore6.Models.ViewModels;
using MyAppWebCore6.DataAccessLayer;
using NuGet.ProjectModel;

namespace MyAppWebCore6.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = WebsiteRole.Roles_Admin)]
    [Authorize]
    public class ProductController : Controller
    {
        private IUnitOfWork _unitOfWork;

        private IWebHostEnvironment _hostingEnvinment;
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment hostingEnvinment)
        {
            _unitOfWork = unitOfWork;
            _hostingEnvinment = hostingEnvinment;
        }

        #region APICALL

        public IActionResult AllProducts()
        {
            var products = _unitOfWork.Product.GetAll(includeProperties:"Category");
            return Json(new { data = products });
        }

        #endregion APICALL
        public IActionResult Index()
        {
            ProductVM productVM = new ProductVM();
            productVM.products = _unitOfWork.Product.GetAll();
            return View();
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
            ProductVM productVM = new ProductVM();
            {
                productVM.Product = new();
                productVM.Categories = _unitOfWork.Category.GetAll().Select(x =>
                new SelectListItem()
                {
                    Text = x.Name,
                    Value = x.Id.ToString()
                });
            }
            if (id == null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = _unitOfWork.Product.GetT(x => x.Id == id);
                if (productVM.Product == null)
                {
                    return NotFound();
                }
                else
                    return View(productVM);
            }
        }
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public IActionResult CreateUpdate(ProductVM productVM, IFormFile? file)
        {
            if (ModelState.IsValid)
            {
                string fileName = String.Empty;
                if (file != null)
                {
                    string uploadDir = Path.Combine(_hostingEnvinment.WebRootPath, "ProductImage");
                    fileName = Guid.NewGuid().ToString() + "-" + file.FileName;
                    string filePath = Path.Combine(uploadDir, fileName);
                    if (productVM.Product.ImageUrl != null)
                    {
                        var oldImagePath = Path.Combine(_hostingEnvinment.WebRootPath, productVM.Product.ImageUrl.TrimStart('\\'));
                        if(System.IO.File.Exists(oldImagePath))
                        {
                            System.IO.File.Delete(oldImagePath);
                        }
                    }
                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        file.CopyTo(fileStream);
                    }
                    productVM.Product.ImageUrl = @"\ProductImage\" + fileName;
                }

                if (productVM.Product.Id == 0)
                {
                    _unitOfWork.Product.Add(productVM.Product);
                    TempData["success"] = "Product Inserted SuccessFully!!";
                }
                else
                {
                    _unitOfWork.Product.Update(productVM.Product);
                    TempData["success"] = "Product Updated SuccessFully!!";
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
            var product = _unitOfWork.Product.GetT(x => x.Id == id);

            if (product == null)
            {
                return NotFound();
            }
            _unitOfWork.Product.Delete(product);
            _unitOfWork.Save();
            TempData["success"] = "Product Deleted SuccessFully!!";
            return RedirectToAction("Index");
        }
        #region DeleteAPICall
        [HttpDelete]
        public IActionResult DeleteProduct(int? id)
        {
            var product = _unitOfWork.Product.GetT(x => x.Id == id);
            if(product == null)
            {
                return Json(new { success = false, Error = "Error in fetching data" });
            }
            else
            {
                var oldImagePath = Path.Combine(_hostingEnvinment.WebRootPath, product.ImageUrl.TrimStart('\\'));
                if (System.IO.File.Exists(oldImagePath))
                    System.IO.File.Delete(oldImagePath);
                _unitOfWork.Product.Delete(product);
                _unitOfWork.Save();
                TempData["success"] = "Product Deleted SuccessFully!!";
                return Json(new { success = true, Error = "Product deleted successfully!!" });
            }
        }

        #endregion
    }
}