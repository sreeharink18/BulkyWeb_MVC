using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.DataAccess.Repository.IRepository
{
    public interface IUnitOfWork
    {
        ICategoryRepository Category { get; }
        IProductRepository Product { get; }
        ICompanyRepository Company { get; }
        IApplicationUserRepository ApplicationUser { get; }
        IShoppingCartRepository ShoppingCart { get; }
        IOrderDetailRepository OrderDetail { get; }
        IOrderHeaderRepository OrderHeader { get; }
		IAddMultipleAddressRepository AddMultipleAddressess { get; }
        ICouponRepository Coupon { get; }
        ISalesReportRepository SalesReport { get; }
        IRatingReviewRepository RatingReview { get; }

        void Save();
    }
}
