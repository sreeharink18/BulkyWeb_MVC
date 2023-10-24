using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyWeb.Areas.Customer.Controllers
{
	[Area("Customer")]
	public class AddMultipleAddressController : Controller
	{
		private IUnitOfWork _unitOfWork;

		public MultipleAddress MultipleAddress { get; set; }
		public AddMultipleAddressController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		[Authorize]
		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			List<MultipleAddress> addresses = _unitOfWork.AddMultipleAddressess.GetAll(u=>u.ApplicationUserId==userId).ToList();
			return View(addresses);
		}
		public IActionResult AddAddress()
		{
			return View();
		}
		[Authorize]
		[HttpPost]
		
		public IActionResult AddAddress(MultipleAddress multipleAddress)
		{ // Check if the user is authenticated
			if (User.Identity != null && User.Identity.IsAuthenticated)
			{
				// Retrieve user information from claims
				var claimsIdentity = (ClaimsIdentity)User.Identity;
				var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier)?.Value;

				// Set default options and associate the address with the user
				multipleAddress.Options = 1;
				
				multipleAddress.ApplicationUserId = userId;

				// Check if the model state is valid
				if (ModelState.IsValid)
				{
					// If valid, add the address and save changes
					_unitOfWork.AddMultipleAddressess.Add(multipleAddress);

					_unitOfWork.Save();

					// Set a success message and redirect to the Index action
					TempData["success"] = "Add Address Successfully";
					return RedirectToAction(nameof(Index));
				}
			}

			return View();
			
		}
        public IActionResult Delete(int? id)
        {
			if(id == null)
			{
				return NotFound();
			}
            MultipleAddress address = _unitOfWork.AddMultipleAddressess.Get(u => u.Id == id);
            
            return View(address);
        }
        [HttpPost,ActionName("Delete")]
        public IActionResult DeletePost(int? id)
        {
			MultipleAddress address = _unitOfWork.AddMultipleAddressess.Get(u => u.Id == id);
			if (address != null)
			{
				_unitOfWork.AddMultipleAddressess.Remove(address);
				_unitOfWork.Save();
				TempData["success"] = "Category delete Successfully";
                return RedirectToAction(nameof(Index));
			}
            return View();
        }
    }
}
