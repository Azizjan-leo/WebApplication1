using Microsoft.AspNetCore.Mvc;
using WebApplication2.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using WebApplication2.Data;
using WebApplication2.Helpers;

namespace WebApplication2.Controllers;

public class AccountController : Controller
{
    private ApplicationContext _context;
    public AccountController(ApplicationContext context)
    {
        _context = context;
    }

    public ActionResult Token()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> Token(User model)
    {
        var identity = await GetIdentity(model.Email, model.Password);
        if (identity == null)
        {
            return BadRequest("Invalid username or password.");
        }

        var now = DateTime.UtcNow;
        // создаем JWT-токен
        var jwt = new JwtSecurityToken(
            issuer: AuthOptions.ISSUER,
            audience: AuthOptions.AUDIENCE,
            notBefore: now,
            claims: identity.Claims,
            expires: now.Add(TimeSpan.FromMinutes(AuthOptions.LIFETIME)),
            signingCredentials: new SigningCredentials(AuthOptions.GetSymmetricSecurityKey(), SecurityAlgorithms.HmacSha256));
        var encodedJwt = new JwtSecurityTokenHandler().WriteToken(jwt);

        var response = new
        {
            access_token = encodedJwt,
            username = identity.Name
        };
        return Json(response);
    }

    async Task<ClaimsIdentity> GetIdentity(string username, string password)
    {
        User person = await _context.Users.Include(r => r.Role).FirstOrDefaultAsync(x => x.Email == username && x.Password == password);
        if (person is null)
            return null;

        var claims = new List<Claim>
            {
                new Claim(ClaimsIdentity.DefaultNameClaimType, person.Email),
                new Claim(ClaimsIdentity.DefaultRoleClaimType, person.Role.Name)
            };
        return new(claims, "Token", ClaimsIdentity.DefaultNameClaimType, ClaimsIdentity.DefaultRoleClaimType);
    }
}
