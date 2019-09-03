using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using TwoCookiesAuth.Dal;
using TwoCookiesAuth.Dal.Models;
using TwoCookiesAuth.Models;

namespace TwoCookiesAuth.Controllers
{
    [Route("/api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly SignInManager<User> _signInManager;

        public UserController(SignInManager<User> signInManager)
        {
            _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        }

        [HttpPost(nameof(Login))]
        public async Task<ActionResult> Login([FromBody]LoginDto dto)
        {
            var signInResult = await _signInManager.PasswordSignInAsync(dto.Email, dto.Password, isPersistent: false, lockoutOnFailure: false);
            
            if (signInResult.Succeeded)
            {
                return NoContent();
            }

            return BadRequest();
        }

        [HttpGet]
        [Authorize]
        public ActionResult Get()
        {
            return Ok(new
            {
                Message = "Detailed description of the user's personal tragedies, relationship issues and financial woes"
            });
        }

        [HttpPost(nameof(Logout))]
        [Authorize]
        public async Task<ActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return NoContent();
        }
    }
}
