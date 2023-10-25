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
    public class CouponRepository : Repository<Coupon>, ICouponRepository
    {
        public readonly ApplicationDbContext _db;
        public CouponRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(Coupon coupon)
        {
            _db.coupons.Update(coupon);
        }
    }
}
