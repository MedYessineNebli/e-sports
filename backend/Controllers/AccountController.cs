using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IConfiguration configuration;

        public AccountController(UserManager<ApplicationUser> userManager
                                  ,IConfiguration configuration  )
        {
            this.userManager = userManager;
            this.configuration = configuration;
        }
        [HttpPost("Register")]
        public async Task<IActionResult> register(NewUserDTO newUserDTO)
        {
            var user = await userManager.FindByEmailAsync(newUserDTO.Email);
            if(user != null)
            {
                return BadRequest("User exist!");
            }
            ApplicationUser appuser = new ApplicationUser()
            {
                UserName = newUserDTO.UserName,
                Email = newUserDTO.Email
            };
            var result = await userManager.CreateAsync(appuser, newUserDTO.Password);
            if (result.Succeeded)
                return Created();

            return BadRequest("Erreur de creation");
        }
        [HttpPost("login")]
        public async Task<IActionResult> login(LoginDTO loginDTO)
        {
            var user = await userManager.FindByNameAsync(loginDTO.UserName);
            if(user == null)
            {
                return Unauthorized("Bad Credentials");
            }
            if(await userManager.CheckPasswordAsync(user, loginDTO.Password))
            {
                var claims = new List<Claim>
                {
                new Claim(ClaimTypes.Name, user.UserName
                    ?? throw new InvalidOperationException("L'utilisateur n'a pas de nom d'utilisateur.")),
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
                };
                var key = new
                SymmetricSecurityKey(Encoding.UTF8.GetBytes(
                    configuration["JWT:SecretKey"]
                    ?? throw new InvalidOperationException("JWT:SecretKey est manquant dans la configuration.")));
                var signingCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
                var token = new JwtSecurityToken(
                claims: claims,
                issuer: configuration["JWT:Issuer"],
                audience: configuration["JWT:Audience"],
                expires: DateTime.Now.AddHours(1),
                signingCredentials: signingCredentials
                );
                return Ok(new
                {
                    token = new JwtSecurityTokenHandler().WriteToken(token),
                    expiration = token.ValidTo,
                    username = loginDTO.UserName
                });


            }
            return Unauthorized("Bad Request");
        }
    }
}
