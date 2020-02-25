using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebshopFietsen.Models;
using WebshopFietsen.Persistence;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;

namespace WebshopFietsen.Controllers
{
    public class AuthController : Controller
    {
        PersistenceCode persistenceCode = new PersistenceCode();

        public IActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Login(string returnUrl, LoginCredentials lc)
        {
            if (ModelState.IsValid)
            {
                lc.ID = persistenceCode.checkLoginCredentials(lc);
                if (lc.ID != -1)
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.Name, lc.ID.ToString())
                    };

                    var userIdentity = new ClaimsIdentity(claims, "SecureLogin");
                    var userPrincipal = new ClaimsPrincipal(userIdentity);
                    HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, userPrincipal,
                        new AuthenticationProperties
                        {
                            ExpiresUtc = DateTime.MaxValue,
                            IsPersistent = false,
                            AllowRefresh = false
                        });
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ViewBag.fout = "Ongeldige login, probeer opnieuw.";
                    return View();
                }
            }
            else
            {
                return View();

            }
        }

        public IActionResult Logout()
        {
            HttpContext.SignOutAsync();
            return RedirectToAction("Login");
        }
    }
}
