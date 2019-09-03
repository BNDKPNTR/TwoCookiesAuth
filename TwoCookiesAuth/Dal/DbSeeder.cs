using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwoCookiesAuth.Dal.Models;

namespace TwoCookiesAuth.Dal
{
    public class DbSeeder
    {
        private readonly TwoCookiesAppDbContext _db;
        private readonly UserManager<User> _userManager;

        public DbSeeder(TwoCookiesAppDbContext db, UserManager<User> userManager)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        }

        public static async Task Seed(IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            using (var db = scope.ServiceProvider.GetRequiredService<TwoCookiesAppDbContext>())
            using (var userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>())
            {
                await new DbSeeder(db, userManager).Seed();
            }
        }

        public async Task Seed()
        {
            await _db.Database.MigrateAsync();

            if (await _db.Users.AnyAsync())
            {
                return;
            }

            var email = "user@mail.com";
            var user = new User { Email = email, UserName = email };
            await _userManager.CreateAsync(user, "123_Asdf");
        }
    }
}
