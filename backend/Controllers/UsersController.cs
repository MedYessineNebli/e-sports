using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/users")]
public sealed class UsersController(
    UserManager<ApplicationUser> userManager,
    RoleManager<IdentityRole> roleManager) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<UserResponse>>> GetAll(CancellationToken cancellationToken)
    {
        var users = await userManager.Users
            .OrderBy(user => user.UserName)
            .ToListAsync(cancellationToken);
        var responses = new List<UserResponse>(users.Count);

        foreach (var user in users)
        {
            cancellationToken.ThrowIfCancellationRequested();
            responses.Add(ToResponse(user, await userManager.GetRolesAsync(user)));
        }

        return Ok(responses);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<UserResponse>> GetById(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound(new ProblemDetails { Title = "Not Found", Detail = "Utilisateur introuvable.", Status = 404 });

        return Ok(ToResponse(user, await userManager.GetRolesAsync(user)));
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(CreateUserDto dto, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var userName = dto.UserName.Trim();
        var email = dto.Email.Trim();

        if (!await roleManager.RoleExistsAsync(dto.RoleName.Trim()))
            return BadRequest(new ProblemDetails { Title = "Bad Request", Detail = "Le rôle sélectionné est introuvable.", Status = 400 });

        if (await userManager.FindByEmailAsync(email) is not null || await userManager.FindByNameAsync(userName) is not null)
            return Conflict(new ProblemDetails { Title = "Conflict", Detail = "Le nom d'utilisateur ou l'email est déjà utilisé.", Status = 409 });

        var user = new ApplicationUser
        {
            UserName = userName,
            Email = email,
            EmailConfirmed = true
        };
        var result = await userManager.CreateAsync(user, dto.Password);
        if (!result.Succeeded)
            return BadRequest(new ValidationProblemDetails(
                result.Errors.ToDictionary(error => error.Code, error => new[] { error.Description })));

        var roleName = dto.RoleName.Trim();
        var roleResult = await userManager.AddToRoleAsync(user, roleName);
        if (!roleResult.Succeeded)
        {
            await userManager.DeleteAsync(user);
            return BadRequest(new ValidationProblemDetails(
                roleResult.Errors.ToDictionary(error => error.Code, error => new[] { error.Description })));
        }

        var response = ToResponse(user, [roleName]);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult<UserResponse>> Update(
        string id,
        UpdateUserDto dto,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound(new ProblemDetails { Title = "Not Found", Detail = "Utilisateur introuvable.", Status = 404 });

        var userName = dto.UserName.Trim();
        var email = dto.Email.Trim();
        var roleName = dto.RoleName.Trim();
        if (!await roleManager.RoleExistsAsync(roleName))
            return BadRequest(new ProblemDetails { Title = "Bad Request", Detail = "Le rôle sélectionné est introuvable.", Status = 400 });

        var emailOwner = await userManager.FindByEmailAsync(email);
        var userNameOwner = await userManager.FindByNameAsync(userName);
        if ((emailOwner is not null && emailOwner.Id != id) || (userNameOwner is not null && userNameOwner.Id != id))
            return Conflict(new ProblemDetails { Title = "Conflict", Detail = "Le nom d'utilisateur ou l'email est déjà utilisé.", Status = 409 });

        user.UserName = userName;
        user.Email = email;
        var result = await userManager.UpdateAsync(user);
        if (!result.Succeeded)
            return BadRequest(new ValidationProblemDetails(
                result.Errors.ToDictionary(error => error.Code, error => new[] { error.Description })));

        if (!string.IsNullOrWhiteSpace(dto.Password))
        {
            var token = await userManager.GeneratePasswordResetTokenAsync(user);
            result = await userManager.ResetPasswordAsync(user, token, dto.Password);
            if (!result.Succeeded)
                return BadRequest(new ValidationProblemDetails(
                    result.Errors.ToDictionary(error => error.Code, error => new[] { error.Description })));
        }

        var currentRoles = await userManager.GetRolesAsync(user);
        if (!currentRoles.Contains(roleName, StringComparer.OrdinalIgnoreCase))
        {
            if (currentRoles.Count > 0)
            {
                result = await userManager.RemoveFromRolesAsync(user, currentRoles);
                if (!result.Succeeded)
                    return BadRequest(new ValidationProblemDetails(
                        result.Errors.ToDictionary(error => error.Code, error => new[] { error.Description })));
            }

            result = await userManager.AddToRoleAsync(user, roleName);
            if (!result.Succeeded)
                return BadRequest(new ValidationProblemDetails(
                    result.Errors.ToDictionary(error => error.Code, error => new[] { error.Description })));
        }

        return Ok(ToResponse(user, [roleName]));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(string id, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value == id)
            return Conflict(new ProblemDetails { Title = "Conflict", Detail = "Vous ne pouvez pas supprimer votre propre compte.", Status = 409 });

        var user = await userManager.FindByIdAsync(id);
        if (user is null)
            return NotFound(new ProblemDetails { Title = "Not Found", Detail = "Utilisateur introuvable.", Status = 404 });

        var result = await userManager.DeleteAsync(user);
        return result.Succeeded
            ? NoContent()
            : BadRequest(new ValidationProblemDetails(
                result.Errors.ToDictionary(error => error.Code, error => new[] { error.Description })));
    }

    private static UserResponse ToResponse(ApplicationUser user, IList<string> roles) =>
        new(user.Id, user.UserName ?? string.Empty, user.Email ?? string.Empty, roles.ToList());
}
