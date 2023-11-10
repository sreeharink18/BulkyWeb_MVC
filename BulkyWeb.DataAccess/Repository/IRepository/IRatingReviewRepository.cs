using BulkyWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.DataAccess.Repository.IRepository
{
    public interface IRatingReviewRepository : IRepository<RatingReview>
    {
        void Update(RatingReview obj); 
    }
}
