using System;
using System.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using AzureSecurityDemo.Models;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;

namespace AzureSecurityDemo.Controllers
{   
    public class HomeController : Controller
    {             
        public async Task<ActionResult> IndexAsync()
        {
            if(User.Identity.IsAuthenticated){
                var token = await HttpContext.GetTokenAsync(OpenIdConnectParameterNames.IdToken);
                ClaimsIdentity claimsIdentity = ((ClaimsIdentity)User.Identity);
                ViewBag.DisplayName = claimsIdentity.Name;
                ViewBag.token = token;
            }

            return View();
        }

        public ActionResult About()
        {
            string userfirstname = User.FindFirstValue(ClaimTypes.GivenName);
            ViewBag.Message = $"Hey {userfirstname}! Welcome {User.FindFirstValue(ClaimTypes.Role)}";
            return View();
        }

      
        public ActionResult Contact()
        {
            string userfirstname = User.FindFirstValue(ClaimTypes.GivenName);
            ViewBag.Message = string.Format("Welcome, {0}!", userfirstname);

            return View();
        }

        [HttpGet]
        [Route("/Account/AccessDenied")]
        public ActionResult AccessDenied()
        {
            return View();
        }

        public async Task Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme);
        }
    }
}
