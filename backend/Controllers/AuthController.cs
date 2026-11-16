using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace API.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController(
    UserManager<ApplicationUser> userManager,
    SignInManager<ApplicationUser> signInManager,
    IConfiguration configuration) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult> Register(RegisterDto dto, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var existingUser = await userManager.FindByEmailAsync(dto.Email);
        if (existingUser is not null)
            return Conflict(new ProblemDetails { Title = "Conflict", Detail = "Cet email est déjà utilisé.", Status = 409 });

        var user = new ApplicationUser
        {
            UserName = dto.UserName.Trim(),
            Email = dto.Email.Trim(),
            EmailConfirmed = false
        };
        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(new ValidationProblemDetails(
                result.Errors.ToDictionary(error => error.Code, error => new[] { error.Description })));

        var roleResult = await userManager.AddToRoleAsync(user, "Player");
        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return Problem("Le compte n'a pas pu recevoir son rôle initial.");
        }

        return Created(string.Empty, new { user.Id, user.Email, Role = "Player" });
    }

    [HttpPost("login")]
    public async Task<ActionResult<LoginResponseDto>> Login(LoginDTO dto, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await userManager.FindByNameAsync(dto.UserName);
        var passwordSignIn = user is not null
            ? await signInManager.CheckPasswordSignInAsync(user, dto.Password, false)
            : null;
        if (user is null || passwordSignIn is null || !passwordSignIn.Succeeded)
            return Unauthorized(new ProblemDetails { Title = "Unauthorized", Detail = "Identifiants invalides.", Status = 401 });

        var roles = await userManager.GetRolesAsync(user);
        var token = CreateToken(user, roles);
        return Ok(token);
    }

    private LoginResponseDto CreateToken(ApplicationUser user, IList<string> roles)
    {
        var secret = configuration["JWT:SecretKey"]
            ?? throw new InvalidOperationException("JWT:SecretKey est manquant dans la configuration.");
        var issuer = configuration["JWT:Issuer"]
            ?? throw new InvalidOperationException("JWT:Issuer est manquant dans la configuration.");
        var audience = configuration["JWT:Audience"]
            ?? throw new InvalidOperationException("JWT:Audience est manquant dans la configuration.");
        var expiration = DateTime.UtcNow.AddHours(1);
        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName ?? user.Email ?? user.Id),
            new(ClaimTypes.Email, user.Email ?? string.Empty),
            new(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };
        claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
        var credentials = new SigningCredentials(
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret)),
            SecurityAlgorithms.HmacSha256);
        var jwt = new JwtSecurityToken(issuer, audience, claims, expires: expiration, signingCredentials: credentials);
        return new LoginResponseDto(new JwtSecurityTokenHandler().WriteToken(jwt), expiration, user.Id, user.Email ?? string.Empty, roles.ToList());
    }
}
