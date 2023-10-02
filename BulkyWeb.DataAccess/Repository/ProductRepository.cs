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
            //Implicetly update
            //_context.products.Update(obj);
          var productfromdb = _context.products.FirstOrDefault(u=>u.Id == obj.Id);
            if (productfromdb != null)
            {
                //explictly update
                productfromdb.Title = obj.Title;
                productfromdb.Description = obj.Description;
                productfromdb.ISBN = obj.ISBN;
                productfromdb.ListPrice = obj.ListPrice;
                productfromdb.Price = obj.Price;
                productfromdb.Price100 = obj.Price100;
                productfromdb.Price50 = obj.Price50;
                productfromdb.Author = obj.Author;
                productfromdb.CategoryId = obj.CategoryId;
                if(obj.ImageUrl != null)
                {
                    productfromdb.ImageUrl = obj.ImageUrl;
                }
            }
        }
    }
}
