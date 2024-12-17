using ApprovalApp.Application;
using ApprovalApp.Domain.Abstractions;
using ApprovalApp.Domain.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Security.Claims;

namespace ApprovalAppMonolit.Controllers
{
    public class LoginController : Controller
    {
        private readonly IAuthService _authService;
        private readonly IPersonsService _personsService;
        public LoginController(IAuthService authService, IPersonsService personsService)
        {
            _authService = authService;
            _personsService = personsService;
        }

        public async Task<IActionResult> Login()
        {
            List<User> users = await _authService.GetUsersAsync();
            return View(users);
        }
        
        [HttpPost]
        public async Task<IActionResult> Login(string? returnUrl, string login, string password)
        {
            if(String.IsNullOrEmpty(password) || String.IsNullOrEmpty(login))
            {
                return BadRequest("Логин и/или пароль не установлены");
            }

            User user = await _authService.GetUserAsync(login: login, password: password);

            if(user is null)
            {
                return Unauthorized();
            }

            Person person = await _personsService.GetPersonByIdAsync(user.Id);
            user.AddFullName(person?.FullName);

            List<Claim> claims = new List<Claim>
            {
                new Claim (ClaimTypes.Name, user?.UserName ?? "Anonimous"),
                new Claim ("PersonId", user?.PersonId.ToString() ?? "-1"),
                new Claim("FullName", user?.FullName ?? "Anonimous")
            };

            ClaimsIdentity claimsIdentity = new ClaimsIdentity(claims, "Cookies");

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

            return Json(Response.StatusCode = 200);
        }
    }
}
