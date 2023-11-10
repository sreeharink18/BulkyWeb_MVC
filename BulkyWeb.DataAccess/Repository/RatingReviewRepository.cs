using BulkyWeb.DataAccess.Data;
using BulkyWeb.DataAccess.Repository.IRepository;
using BulkyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.DataAccess.Repository
{
    public class RatingReviewRepository : Repository<RatingReview>, IRatingReviewRepository
    {
        public readonly ApplicationDbContext _db;
        public RatingReviewRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }
        public void Update(RatingReview obj)
        {
            _db.RatingReviews.Update(obj);
        }
    }
}
