using BulkyWeb.DataAccess.Data;
using BulkyWeb.DataAccess.Repository.IRepository;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyWeb.DataAccess.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        public readonly ApplicationDbContext _db;
        internal DbSet<T> dbSet;
        public Repository(ApplicationDbContext db)
        {
            _db = db;
            dbSet =  _db.Set<T>();//_db.Categories = dbset
            _db.products.Include(u => u.category).Include(u=>u.CategoryId);
        }
        public void Add(T entity)
        {
            dbSet.Add(entity);
        }
        public T Get(Expression<Func<T, bool>> filter, string? includeProperties = null, bool tracked = false)
        {
            try
            {
                IQueryable<T> query;
                if (tracked)
                {
                    query = dbSet;
                }
                else
                {
                    query = dbSet.AsNoTracking();
                }

                query = query.Where(filter);

                if (!string.IsNullOrEmpty(includeProperties))
                {
                    foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                    {
                        query = query.Include(includeProp);
                    }
                }

                return query.FirstOrDefault();
            }
            catch (Exception ex)
            {
                // Log the exception or handle it as needed
                // You can rethrow the exception if you want to propagate it
                // throw;

                // Return a default value or handle the exception in another way
                return default(T);
            }

        }

        //category,CoverType
        public IEnumerable<T> GetAll(Expression<Func<T, bool>>? filter, string? includeProperties = null)
        {
            IQueryable<T> query = dbSet;
            if (filter != null)
            {
                query = query.Where(filter);
            }
            if (!string.IsNullOrEmpty(includeProperties))
            {
                foreach (var includeProp in includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProp);
                }
            }
            return query.ToList();
            try
            {
               
            }
            catch (DbException ex)
            {
                // Handle specific database exceptions
                Console.WriteLine($"Database exception: {ex.Message}");
            }
            catch (Exception ex)
            {
                // Handle other general exceptions
                Console.WriteLine($"An unexpected error occurred: {ex.Message}");
            }

            // Return an empty collection or throw the exception again based on your needs
            return Enumerable.Empty<T>();
        }
        public void Remove(T entity)
        {
           dbSet.Remove(entity);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            dbSet.RemoveRange(entities);
        }
    }
}
