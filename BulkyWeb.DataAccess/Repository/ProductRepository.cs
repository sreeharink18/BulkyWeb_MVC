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
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        internal readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext db) : base(db) { 
            _context = db;

        }
        public void Update(Product obj)
        {
            _context.products.Update(obj);
        }
    }
}
