using CommonHelper;
using DataAccessLayer.Infrastructure.IRepository;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Models;
using Models.ViewModels;
using System.Security.Claims;

namespace Web.Areas.Customer.Controllers
{
    [Area("Customer")]
    [Authorize]
    public class CartController : Controller
    {
        private IUnitOfWork _unitOfWork;

        public CartVM cartVM { get; set; }

        public CartController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }   

        public IActionResult Index()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            cartVM = new CartVM()
            {
                ListOfCart = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties: "Product"),
                OrderHeader = new Models.OrderHeader()
            };
            
            foreach (var item in cartVM.ListOfCart)
            {
                cartVM.OrderHeader.OrderTotal += (item.Product.Price * item.Count);
            }
            return View(cartVM);
        }

        public IActionResult Summary()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var claims = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier);

            cartVM = new CartVM()
            {
                ListOfCart = _unitOfWork.Cart.GetAll(x => x.ApplicationUserId == claims.Value, includeProperties: "Product"),
                OrderHeader = new Models.OrderHeader()
            };

            cartVM.OrderHeader.ApplicationUser = _unitOfWork.ApplicationUser.GetT(x => x.Id == claims.Value);
            cartVM.OrderHeader.Name = cartVM.OrderHeader.ApplicationUser.Name;
            cartVM.OrderHeader.Phone = cartVM.OrderHeader.ApplicationUser.PhoneNumber;
            cartVM.OrderHeader.Address = cartVM.OrderHeader.ApplicationUser.Address;
            cartVM.OrderHeader.City = cartVM.OrderHeader.ApplicationUser.City;
            cartVM.OrderHeader.State = cartVM.OrderHeader.ApplicationUser.State;
            cartVM.OrderHeader.PostalCode = cartVM.OrderHeader.ApplicationUser.PinCode;

            foreach (var item in cartVM.ListOfCart)
            {
                cartVM.OrderHeader.OrderTotal += (item.Product.Price * item.Count);
            }
            return View(cartVM);
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
                    OrderHeaderId = vm.OrderHeader.Id,
                    Count = item.Count,
                    Price = item.Product.Price,
                };
                _unitOfWork.OrderDetail.Add(orderDetail);
                _unitOfWork.Save();
            }
            
            _unitOfWork.Cart.DeleteRange(vm.ListOfCart);
            _unitOfWork.Save();

            return RedirectToAction("Index", "Home");
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
            return RedirectToAction(nameof(Index));
        }
    }
}
