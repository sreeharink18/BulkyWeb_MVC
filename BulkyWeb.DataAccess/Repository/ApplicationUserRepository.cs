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
    public class ApplicationUserRepository : Repository<ApplicationUser>, IApplicationUserRepository
    {
        public readonly ApplicationDbContext _db;
        public ApplicationUserRepository(ApplicationDbContext db) : base(db)
        {
            _db = db;
        }

        public void Update(ApplicationUser obj)
        {
            var applicatiionUser =_db.ApplicationUsers.FirstOrDefault(u => u.Id == obj.Id);
            if(applicatiionUser != null)
            {
                applicatiionUser.PhoneNumber = obj.PhoneNumber;
                applicatiionUser.Name = obj.Name;
                applicatiionUser.StreetAddress = obj.StreetAddress;
                applicatiionUser.City   = obj.City;
                applicatiionUser.State = obj.State;
                applicatiionUser.PostCode = obj.PostCode;
                applicatiionUser.Wallet = obj.Wallet;
            }
           
        }
    }
}
