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
    public class AddMultipleAddressRepository : Repository<MultipleAddress>, IAddMultipleAddressRepository
	{
        public readonly ApplicationDbContext _db;
        public AddMultipleAddressRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }
        public void Update(MultipleAddress obj)
        {
            _db.MultipleAddresses.Update(obj);
        }
    }
}
