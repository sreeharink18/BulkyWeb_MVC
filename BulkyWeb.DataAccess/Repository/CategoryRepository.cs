﻿using BulkyWeb.DataAccess.Data;
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
    public class CategoryRepository : Repository<Category>,ICategoryRepository
    {
        public readonly ApplicationDbContext _db;
        public CategoryRepository(ApplicationDbContext db) : base(db) 
        {
            _db = db;
        }
        public void Update(Category obj)
        {
            _db.categories.Update(obj);
        }
    }
}
