using System.Text;
using API.Infrastructure;
using API.Middleware;
using API.Models;
using API.Repositories;
using API.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using MediatR;

var builder = WebApplication.CreateBuilder(args);
const string FrontendCorsPolicy = "Frontend";

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy(FrontendCorsPolicy, policy =>
    {
        policy.WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.AddMediatR(configuration =>
    configuration.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddProblemDetails();
builder.Services.AddExceptionHandler<ApiExceptionHandler>();
builder.Services.AddDbContext<ApplicationContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("cnx")));
builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
    .AddEntityFrameworkStores<ApplicationContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAutoMapper(options => options.AddProfile<MappingProfile>());

var secretKey = builder.Configuration["JWT:SecretKey"]
    ?? throw new InvalidOperationException("JWT:SecretKey est manquant dans la configuration.");

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey)),
        ValidIssuer = builder.Configuration["JWT:Issuer"]
            ?? throw new InvalidOperationException("JWT:Issuer est manquant dans la configuration."),
        ValidAudience = builder.Configuration["JWT:Audience"]
            ?? throw new InvalidOperationException("JWT:Audience est manquant dans la configuration."),
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        RoleClaimType = System.Security.Claims.ClaimTypes.Role,
        ClockSkew = TimeSpan.Zero
    };
});

builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "Entrez votre token JWT"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IGameRepository, GameRepository>();
builder.Services.AddScoped<ITeamRepository, TeamRepository>();
builder.Services.AddScoped<IPlayerRepository, PlayerRepository>();
builder.Services.AddScoped<ITournamentRepository, TournamentRepository>();
builder.Services.AddScoped<ITournamentRegistrationRepository, TournamentRegistrationRepository>();
builder.Services.AddScoped<IMatchRepository, MatchRepository>();
builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped<IGameService, GameService>();
builder.Services.AddScoped<ITeamService, TeamService>();
builder.Services.AddScoped<IPlayerService, PlayerService>();
builder.Services.AddScoped<ITournamentService, TournamentService>();
builder.Services.AddScoped<ITournamentRegistrationService, TournamentRegistrationService>();
builder.Services.AddScoped<IMatchService, MatchService>();
var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    await DatabaseSeeder.SeedAsync(scope.ServiceProvider, app.Configuration);
}

// Configure the HTTP request pipeline.
app.UseExceptionHandler();
app.UseStatusCodePages();
app.UseSwagger();
app.UseSwaggerUI(options =>
{
    options.RoutePrefix = string.Empty;
    options.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1");
});

app.UseHttpsRedirection();

app.UseCors(FrontendCorsPolicy);
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
