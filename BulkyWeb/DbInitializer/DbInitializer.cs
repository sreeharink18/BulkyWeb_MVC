using BulkyWeb.DataAccess.Data;
using BulkyWeb.Models;
using BulkyWeb.Utility;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace BulkyWeb.DbInitializer
{
    public class DbInitializer : IDbInitializer
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ApplicationDbContext _db;
        public DbInitializer(
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager,
            ApplicationDbContext db) {
            userManager = _userManager;
            _roleManager = roleManager;
            _db = db;
        }
        public void Initialize()
        {
            //migration if they are not applied
            try
            {
                if (_db.Database.GetPendingMigrations().Count() > 0)
                {
                    _db.Database.Migrate();
                }
            }catch (Exception ex)
            {

            }
            //crate role if they are not created
            if (!_roleManager.RoleExistsAsync(SD.Role_Customer).GetAwaiter().GetResult())
            {
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Customer)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Admin)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Employee)).GetAwaiter().GetResult();
                _roleManager.CreateAsync(new IdentityRole(SD.Role_Company)).GetAwaiter().GetResult();

                //if roles are not created , then we will created admin user as well
                _userManager.CreateAsync(new ApplicationUser
                {
                    UserName = "adminLearnClub@gmail.com",
                    Email = "adminLearnClub@gmail.com",
                    Name = "Sreehari Admin",
                    PhoneNumber = "1234567890",
                    StreetAddress = "123 street address",
                    State = "Kerala",
                    City = "Thrissur",
                    PostCode = "680568"

                }, "Admin@123").GetAwaiter().GetResult();
                ApplicationUser applicationUser = _db.ApplicationUsers.FirstOrDefault(u => u.Email == "adminLearnClub@gmail.com");
                _userManager.AddToRoleAsync(applicationUser, SD.Role_Admin).GetAwaiter().GetResult();

            }
            return;
            

        }
    }
}
