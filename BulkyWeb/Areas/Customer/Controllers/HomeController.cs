using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using BulkyWeb.Models.ViewModel;
using BulkyWeb.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Stripe.Checkout;
using System.Diagnostics;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
    [Area("Customer")]
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IUnitOfWork _unitOfWork;
        private static int walletAmount;
        [BindProperty]
        public RatingReviewVM RatingReviewVM { get; set; }
        public HomeController(ILogger<HomeController> logger, IUnitOfWork unitOfWork)
        {
            _logger = logger;
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {

            List<Product> products = _unitOfWork.Product.GetAll(includeProperties: "category").ToList();
            return View(products);
        }
        public IActionResult Details(int productId)
        {
            ShoppingCart cart = new()
            {
                Product = _unitOfWork.Product.Get(u => u.Id == productId, includeProperties: "category"),
                Count = 1,
                ProductId = productId,

            };
            int discountAmount;
            if(cart.Product.category.IsDiscount == SD.IsValid || cart.Product.IsDiscountProduct == SD.IsValid)
            {
                if (cart.Product.IsDiscountProduct == SD.IsValid && cart.Product.category.IsDiscount == SD.IsValid)
                {
                    discountAmount = (int)cart.Product.Price * (100 - (int)cart.Product.category.DiscountAmount) / 100;
                    discountAmount = (int)discountAmount * (100 - (int)cart.Product.DiscountAmount) / 100;
                    ViewBag.DiscountAmount = discountAmount;
                }
                else
                {
                    if (cart.Product.category.IsDiscount == SD.IsValid)
                    {
                        discountAmount = (int)cart.Product.Price * (100 - (int)cart.Product.category.DiscountAmount) / 100;
                        ViewBag.DiscountAmount = discountAmount;
                    }

                    if (cart.Product.IsDiscountProduct == SD.IsValid)
                    {
                        discountAmount = (int)cart.Product.Price * (100 - (int)cart.Product.DiscountAmount) / 100;
                        ViewBag.DiscountAmount = discountAmount;
                    }
                }
              
                
            }
           
            return View(cart);
        }
        [HttpPost]
        [Authorize]
        public IActionResult Details(ShoppingCart shoppingCart)
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            shoppingCart.ApplicationUserId = UserId;
            ShoppingCart cartFromdb = _unitOfWork.ShoppingCart.Get(u => u.ApplicationUserId == UserId && u.ProductId == shoppingCart.ProductId);
            if (cartFromdb != null)
            {

                //If exists than Update
                cartFromdb.Count += shoppingCart.Count;
                _unitOfWork.ShoppingCart.Update(cartFromdb);
                _unitOfWork.Save();
            }
            else
            {
                //Add new 
                _unitOfWork.ShoppingCart.Add(shoppingCart);
                _unitOfWork.Save();
                HttpContext.Session.SetInt32(SD.sessionCart, _unitOfWork.ShoppingCart.GetAll(u => u.ApplicationUserId == UserId).Count());
                
            }
            
            TempData["success"] = "Shopping Cart is Updated";
            return RedirectToAction(nameof(Index));
        }
        public IActionResult Privacy()
        {
            return View(); 
        }
        [Authorize]
        public  IActionResult UserProfile()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ApplicationUser Userobj = _unitOfWork.ApplicationUser.Get(u => u.Id == UserId);
            if (Userobj != null)
            {
                return View(Userobj);
            }
            return View();
        }
        public IActionResult EditUserProfile()
        {
            var claimsIdentity = (ClaimsIdentity)User.Identity;
            var UserId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            ApplicationUser Userobj = _unitOfWork.ApplicationUser.Get(u => u.Id == UserId);
            if (Userobj != null)
            {
                return View(Userobj);
            }
            return View();
        }
        [HttpPost]
        public IActionResult EditUserProfile(ApplicationUser applicationUser)
        {
         
                _unitOfWork.ApplicationUser.Update(applicationUser);
                _unitOfWork.Save();
                return RedirectToAction(nameof(UserProfile));
            
           
        }
        public IActionResult Wallet()
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var UserId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

            var userobj= _unitOfWork.ApplicationUser.Get(u=>u.Id == UserId);
            if(userobj.Wallet == null)
            {
                userobj.Wallet = 0;
            }
            if(userobj.Wallet <= 0)
            {
                userobj.Wallet = 0;
            }
            return View(userobj);
        }
        [HttpPost]
        public IActionResult Wallet(ApplicationUser applicationUser)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var UserId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            SetWalletValue((int)applicationUser.Wallet);

            var UserObj = _unitOfWork.ApplicationUser.Get(u=>u.Id == UserId);   
            if(UserObj != null)
            {
                if(UserObj.Wallet == null)
                {
                    UserObj.Wallet = 0;
                }
                if(applicationUser.Wallet !=null)
                {
                    if (applicationUser.Wallet <= 0 )
                    {
                        TempData["ValueNot"] = "Plz enter the value above 0 ";
                        return View(applicationUser);

                    }
                    else
                    {
                        try
                        {
                            var domain = "https://localhost:7279/";
                            var options = new SessionCreateOptions
                            {

                                SuccessUrl = domain + $"customer/Home/WalletSuccess?id={UserObj.Id}",
                                CancelUrl = domain + "customer/cart/Index",
                                LineItems = new List<SessionLineItemOptions>(),
                                Mode = "payment",
                            };
                            var sessionLineItem = new SessionLineItemOptions
                            {
                                PriceData = new SessionLineItemPriceDataOptions()
                                {
                                    UnitAmount = (long?)(applicationUser.Wallet * 100), //its can include in here about amount
                                    Currency = "INR",

                                    ProductData = new SessionLineItemPriceDataProductDataOptions()
                                    {

                                        Name = "Book Store",
                                        Description = "Add amount in your wallet"
                                    }

                                },
                                Quantity = 1,

                            };
                            options.LineItems.Add(sessionLineItem);
                            var service = new SessionService();
                            Session session = service.Create(options);
                            service.Create(options);
                            UserObj.Wallet += applicationUser.Wallet;
                            _unitOfWork.ApplicationUser.Update(UserObj);
                            _unitOfWork.Save();
                            TempData["success"] = "Add Amount in Wallet";
                            Response.Headers.Add("Location", session.Url);
                            return new StatusCodeResult(303);
                        }catch(Exception ex)
                        {
                            Console.WriteLine(ex.ToString());
                        }

                    }
                    
                    return RedirectToAction(nameof(UserProfile));
                }  
            }
            TempData["error"] = "Not add Amount in Wallet";
            return View(applicationUser);

        }
        public IActionResult WalletSuccess(string id)
        {
           ApplicationUser userObj= _unitOfWork.ApplicationUser.Get(u=>u.Id == id);
            try
            {
                ViewBag.WalletAmount = walletAmount;
            }catch(Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            
            return View(userObj);
        }
        private void SetWalletValue(int amount)
        {
            walletAmount = amount;
        }
        private int GetWalletAmount()
        {
            return walletAmount;
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        [Authorize]
        public IActionResult RatingReview(int productId)
        {
            RatingReviewVM = new()
            {
                ProductId = productId,
                RatingReviewAll = _unitOfWork.RatingReview.GetAll(u => u.ProductId == productId, includeProperties: "Product").ToList(),
               
                RatingReview =new() ,
            };
            
          /*  List<RatingReview> ratingReview = _unitOfWork.RatingReview.GetAll(u=>u.ProductId == productId,includeProperties:"Product").ToList();
            if(ratingReview.Count >0)
            {
                return View(ratingReview);
            }*/
            return View(RatingReviewVM);
        }

        [HttpPost]
        public IActionResult RatingReview(RatingReviewVM ratingReviewVM)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var UserId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            if (ratingReviewVM == null)
            {
                return View();
            }
            ApplicationUser applicationUser =_unitOfWork.ApplicationUser.Get(u=>u.Id == UserId);
            RatingReview ratingReview = new()
            {
                Rating = ratingReviewVM.RatingValue,
                Review = ratingReviewVM.Review,
                ProductId = ratingReviewVM.ProductId,
                ApplicationUserId = UserId,
                User = _unitOfWork.ApplicationUser.Get(u => u.Id == UserId , includeProperties: "ApplicationUser"),
                Name=applicationUser.Name,
                TimeStamp = DateTime.Now
            };
            _unitOfWork.RatingReview.Add(ratingReview);
            _unitOfWork.Save();
            RatingReviewVM = new()
            {
                ProductId = ratingReviewVM.ProductId,
                RatingReviewAll = _unitOfWork.RatingReview.GetAll(u => u.ProductId == ratingReviewVM.ProductId, includeProperties: "Product").ToList(),
                RatingReview = new(),
            };
            return RedirectToAction(nameof(RatingReview), new {ratingReview.ProductId});
        }
        public IActionResult RatingStarValue(int productId, int RatingValue)
        {
            var claimIdentity = (ClaimsIdentity)User.Identity;
            var UserId = claimIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;
            try
            {
                RatingReview ratingRaviewObj = new() { 
                    ProductId = productId,
                    Rating = RatingValue,
                    ApplicationUserId =UserId
                };

                IEnumerable<RatingReview> ratingReview = _unitOfWork.RatingReview.GetAll().ToList();
                IEnumerable< RatingReview> ratingProduct = _unitOfWork.RatingReview.GetAll(u => u.ProductId == productId).ToList();
                if(ratingProduct.Count()>=0)
                {
                    if (ratingProduct.Any())
                    {
                        RatingReview ratingUser = ratingProduct.FirstOrDefault(u => u.ApplicationUserId == UserId);
                        if(ratingUser != null)
                        {
                            _unitOfWork.RatingReview.Update(ratingRaviewObj);
                            _unitOfWork.Save();
                            return RedirectToAction(nameof(RatingReview), new {RatingReviewVM});
                        }
                    }
                    
                    
                }
                
                    _unitOfWork.RatingReview.Add(ratingRaviewObj);
                    _unitOfWork.Save();
                    return RedirectToAction(nameof(RatingReview), new { RatingReviewVM });
                
            }
            catch (Exception ex)
            {
                Console.WriteLine("................."+ex.ToString());
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
