using BulkyWeb.DataAccess.Data;
using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.DataAccess.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private ApplicationDbContext _context;
        public ICategoryRepository Category { get; private set; }
        public IProductRepository Product { get; private set; }
        public ICompanyRepository Company { get; private set; }
        public IApplicationUserRepository ApplicationUser { get; private set; }
        public IShoppingCartRepository ShoppingCart { get; private set; }
        public IOrderHeaderRepository OrderHeader { get; private set; }
        public IOrderDetailRepository OrderDetail { get; private set; }
        public IAddMultipleAddressRepository AddMultipleAddressess { get; private set; }
        public ICouponRepository Coupon { get; private set; }
        public ISalesReportRepository SalesReport { get; private set; }
        public IRatingReviewRepository RatingReview { get; private set; }

        public UnitOfWork(ApplicationDbContext db)
        {
            _context = db;
            Category = new CategoryRepository(_context);
            Product =new ProductRepository(_context);
            Company =new CompanyRepository(_context);
            ApplicationUser = new ApplicationUserRepository(_context);
            ShoppingCart =new ShoppingCartRepository(_context);
            OrderHeader =new OrderHeaderRepository(_context);
            OrderDetail =new OrderDetailRepository(_context);
            AddMultipleAddressess =new AddMultipleAddressRepository(_context);
            Coupon =new CouponRepository(_context);
            SalesReport =new SalesReportRepository(_context);
            RatingReview =new RatingReviewRepository(_context);
        }
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}
