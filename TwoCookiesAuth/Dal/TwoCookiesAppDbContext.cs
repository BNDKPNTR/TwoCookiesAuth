using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TwoCookiesAuth.Dal.Models;

namespace TwoCookiesAuth.Dal
{
    public class TwoCookiesAppDbContext : IdentityDbContext<User>
    {
        public TwoCookiesAppDbContext(DbContextOptions options) : base(options)
        {
        }
    }
}
