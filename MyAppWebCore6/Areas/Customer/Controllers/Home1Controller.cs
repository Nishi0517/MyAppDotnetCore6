using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MyAppDotnetCore6.DataAccessLayer.Infrastructure.IRepository;
using MyAppDotnetCore6.Models;
using System.Security.Claims;

namespace MyAppWebCore6.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class Home1Controller : Controller
    {
        private readonly IUnitOfWork _unitOfWork;

        public Home1Controller(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            IEnumerable<Product> products = _unitOfWork.Product.GetAll(includeProperties: "Category");
            return View(products);
        }
        [HttpGet]
        public IActionResult Details(int? ProductId)
        {
            Cart cart = new Cart()
            {
                Product = _unitOfWork.Product.GetT(x => x.Id == ProductId, includeProperties: "Category"),
                Count = 1,
                ProductId = (int)ProductId

            };
            return View(cart);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public IActionResult Details(Cart cart)
        {

            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);
            cart.ApplicationUserId = claims.Value;


            var cartItem = _unitOfWork.Cart.GetT(x => x.ProductId == cart.ProductId && x.ApplicationUserId == claims.Value);

            if (cartItem == null)
            {
                //if (ModelState.IsValid)
                //{
                _unitOfWork.Cart.Add(cart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32("SessionCart", _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claims.Value).ToList().Count);

                //}
            }
            else
            {
                _unitOfWork.Cart.IncrementCartItem(cartItem, cart.Count);
            }
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }
    }
}
