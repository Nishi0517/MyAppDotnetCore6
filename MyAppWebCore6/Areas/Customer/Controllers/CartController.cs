using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MyAppDotnetCore6.CommonHelper;
using MyAppDotnetCore6.DataAccessLayer.Infrastructure.IRepository;
using MyAppDotnetCore6.Models;
using MyAppDotnetCore6.Models.ViewModels;
using Newtonsoft.Json.Linq;
using Stripe;
using Stripe.BillingPortal;
using Stripe.Checkout;
using System;
using System.Security.Claims;
using Session = Stripe.Checkout.Session;
using SessionCreateOptions = Stripe.Checkout.SessionCreateOptions;
using SessionService = Stripe.Checkout.SessionService;

namespace MyAppWebCore6.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private IUnitOfWork _unitOfWork;
        public CartVM vm { get; set; }
        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            vm = new CartVM()
            {
                ListOfCart = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };
            foreach (var item in vm.ListOfCart)
            {
                vm.OrderHeader.OrderTotal += (item.Product.Price * item.Count);
            }
            return View(vm);
        }
        public IActionResult plus(int id)
        {
            var cart = _unitOfWork.Cart.GetT(x => x.Id == id);
            _unitOfWork.Cart.IncrementCartItem(cart, 1);
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult minus(int id)
        {
            var cart = _unitOfWork.Cart.GetT(x => x.Id == id);
            if (cart.Count <= 1)
            {
                _unitOfWork.Cart.Delete(cart);
                var count = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count - 1;
                HttpContext.Session.SetInt32("SessionCart", count);

            }
            else
            {
                _unitOfWork.Cart.DecrementCartItem(cart, 1);
            }
            _unitOfWork.Save();
            return RedirectToAction(nameof(Index));
        }
        public IActionResult delete(int id)
        {
            var cart = _unitOfWork.Cart.GetT(x => x.Id == id);
            _unitOfWork.Cart.Delete(cart);
            _unitOfWork.Save();

            var count = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == cart.ApplicationUserId).ToList().Count;
            HttpContext.Session.SetInt32("SessionCart",count);

            return RedirectToAction(nameof(Index));
        }
        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            vm = new CartVM()
            {
                ListOfCart = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties: "Product"),
                OrderHeader = new OrderHeader()
            };
            vm.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetT(x => x.Id == claims.Value);
            vm.OrderHeader.Name = vm.OrderHeader.ApplicationUser.Name;
            vm.OrderHeader.Address = vm.OrderHeader.ApplicationUser.Address;
            vm.OrderHeader.Phone = vm.OrderHeader.ApplicationUser.PhoneNumber;
            vm.OrderHeader.City = vm.OrderHeader.ApplicationUser.City;
            vm.OrderHeader.State = vm.OrderHeader.ApplicationUser.State;
            vm.OrderHeader.PostalCode = vm.OrderHeader.ApplicationUser.PinCode;

            foreach (var item in vm.ListOfCart)
            {
                vm.OrderHeader.OrderTotal += (item.Product.Price * item.Count);
            }
            return View(vm);
        }
        public IActionResult ordersuccess(int id)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetT(x => x.Id == id);
            var service = new SessionService();
            Session session = service.Get(orderHeader.SessionId);

            if(session.PaymentStatus.ToLower() == "paid")
            {
                _unitOfWork.OrderHeader.UpdateStatus(id, OrderStatus.StatusApproved,PaymentStatus.StatusApproved);
            }
            List<Cart> cart = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == orderHeader.ApplicationUserId).ToList();
            _unitOfWork.Cart.DeleteRange(cart);
            _unitOfWork.Save();

            return View(id);
        }
        [HttpPost]
        public IActionResult Summary(CartVM vm)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            vm.ListOfCart = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties: "Product");
            vm.OrderHeader.OrderStatus = OrderStatus.StatusPending;
            vm.OrderHeader.PaymentStatus = PaymentStatus.StatusPending;
            vm.OrderHeader.DateOfOrder = DateTime.Now;
            vm.OrderHeader.ApplicationUserId = claims.Value;

            foreach (var item in vm.ListOfCart)
            {
                vm.OrderHeader.OrderTotal += (item.Product.Price * item.Count);
            }
            _unitOfWork.OrderHeader.Add(vm.OrderHeader);
            _unitOfWork.Save();

            foreach (var item in vm.ListOfCart)
            {
                OrderDetail orderDetail = new OrderDetail()
                {
                    ProductId = item.ProductId,
                    Count = item.Count,
                    OrderHeaderId = vm.OrderHeader.Id,
                    Price = item.Product.Price
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
            //StripeConfiguration.ApiKey = "sk_test_51N6BrrSAeJGrl3tt2k6NQjxsR8XlHbpxcba7Ctf51q8ZnknvrmSGvMUE0LYv6Vyw7elgOqt180wmRxQ26gRs1k4U00hbqZZKPj";
            var domain = "https://localhost:7277/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"customer/cart/ordersuccess?id={vm.OrderHeader.Id}",
                CancelUrl = domain + $"customer/cart/Index",
            };

            foreach (var item in vm.ListOfCart)
            {
                var lineItemsOptions = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)item.Product.Price,
                        Currency = "INR",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(lineItemsOptions);
            }
            var service = new SessionService();
            Session session = service.Create(options);

           
            //// Retrieve the Session object with expanded PaymentIntent
            //var service2 = new SessionService();
            //Session sessionWithPaymentIntent = service2.Get(session.Id, new SessionGetOptions
            //{
            //    Expand = new List<string> { "payment_intent" }
            //});

            //// Retrieve PaymentIntentId from the expanded Session object
            //var paymentIntentId = sessionWithPaymentIntent.PaymentIntent.Id;


            _unitOfWork.OrderHeader.PaymentStatus(vm.OrderHeader.Id, session.Id, session.Id);
            _unitOfWork.Save();

            _unitOfWork.Cart.DeleteRange(vm.ListOfCart);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

            return RedirectToAction("Index", "Home");

        }
       
        public IActionResult PayNow(OrderVM vm)
        {
            var orderHeader = _unitOfWork.OrderHeader.GetT(x=>x.Id==vm.OrderHeader.Id , includeProperties:"ApplicationUser");
            var orderDetail = _unitOfWork.OrderDetail.GetAll(x => x.OrderHeaderId == vm.OrderHeader.Id, includeProperties: "Product");

            var domain = "https://localhost:7277/";
            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>(),
                Mode = "payment",
                SuccessUrl = domain + $"customer/cart/ordersuccess?id={vm.OrderHeader.Id}",
                CancelUrl = domain + $"customer/cart/Index",
            };

            foreach (var item in orderDetail)
            {
                var lineItemsOptions = new SessionLineItemOptions
                {
                    PriceData = new SessionLineItemPriceDataOptions
                    {
                        UnitAmount = (long)item.Product.Price,
                        Currency = "INR",
                        ProductData = new SessionLineItemPriceDataProductDataOptions
                        {
                            Name = item.Product.Name
                        },
                    },
                    Quantity = item.Count,
                };
                options.LineItems.Add(lineItemsOptions);
            }
            var service = new SessionService();
            Session session = service.Create(options);

            _unitOfWork.OrderHeader.PaymentStatus(vm.OrderHeader.Id, session.Id, session.Id);
            _unitOfWork.Save();

            Response.Headers.Add("Location", session.Url);
            return new StatusCodeResult(303);

            return RedirectToAction("Index", "Home");
        }
    }
}
