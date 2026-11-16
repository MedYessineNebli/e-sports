using API.DTOs;
using API.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Authorize(Roles = "Admin")]
[Route("api/roles")]
public sealed class RolesController(
    RoleManager<IdentityRole> roleManager,
    UserManager<ApplicationUser> userManager) : ControllerBase
{
    [HttpGet]
    public async Task<ActionResult<PagedResult<string>>> GetAll(
        [FromQuery] PagedRequest request,
        CancellationToken cancellationToken)
    {
        var query = roleManager.Roles.OrderBy(role => role.Name);
        var totalCount = await query.CountAsync(cancellationToken);
        var roles = await query
            .Skip((request.ValidatedPage - 1) * request.ValidatedPageSize)
            .Take(request.ValidatedPageSize)
            .Select(role => role.Name!)
            .ToListAsync(cancellationToken);
        var totalPages = (int)Math.Ceiling(totalCount / (double)request.ValidatedPageSize);
        return Ok(new PagedResult<string>(
            request.ValidatedPage,
            request.ValidatedPageSize,
            totalCount,
            totalPages,
            roles));
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateRoleDto dto, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        if (await roleManager.RoleExistsAsync(dto.Name))
            return Conflict(new ProblemDetails { Title = "Conflict", Detail = "Ce rôle existe déjà.", Status = 409 });
        var result = await roleManager.CreateAsync(new IdentityRole(dto.Name.Trim()));
        return result.Succeeded
            ? Created(string.Empty, new { role = dto.Name.Trim() })
            : BadRequest(new ValidationProblemDetails(
                result.Errors.ToDictionary(error => error.Code, error => new[] { error.Description })));
    }

    [HttpDelete("{roleName}")]
    public async Task<IActionResult> Delete(string roleName, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var role = await roleManager.FindByNameAsync(roleName);
        if (role is null)
            return NotFound();
        var result = await roleManager.DeleteAsync(role);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPost("assign")]
    public async Task<IActionResult> Assign(AssignRoleDto dto, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await userManager.FindByIdAsync(dto.UserId);
        if (user is null || !await roleManager.RoleExistsAsync(dto.RoleName))
            return NotFound(new ProblemDetails { Title = "Not Found", Detail = "Utilisateur ou rôle introuvable.", Status = 404 });
        if (await userManager.IsInRoleAsync(user, dto.RoleName))
            return Conflict(new ProblemDetails { Title = "Conflict", Detail = "L'utilisateur possède déjà ce rôle.", Status = 409 });
        var result = await userManager.AddToRoleAsync(user, dto.RoleName);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }

    [HttpPost("remove")]
    public async Task<IActionResult> Remove(AssignRoleDto dto, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var user = await userManager.FindByIdAsync(dto.UserId);
        if (user is null || !await roleManager.RoleExistsAsync(dto.RoleName))
            return NotFound(new ProblemDetails { Title = "Not Found", Detail = "Utilisateur ou rôle introuvable.", Status = 404 });
        var result = await userManager.RemoveFromRoleAsync(user, dto.RoleName);
        return result.Succeeded ? NoContent() : BadRequest(result.Errors);
    }
}
