using System.Security.Claims;
using System.Text;
using LibraryManagement.API.Middleware;
using LibraryManagement.Application;
using LibraryManagement.Infrastructure;
using LibraryManagement.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Controllers
builder.Services.AddControllers();

// Clean Architecture
builder.Services.AddApplication();

builder.Services.AddInfrastructure(
    builder.Configuration);

// JWT
var jwtKey = builder.Configuration["Jwt:Key"]
    ?? throw new InvalidOperationException(
        "JWT key is missing.");

if (Encoding.UTF8.GetByteCount(jwtKey) < 32)
{
    throw new InvalidOperationException(
        "JWT key must be at least 32 bytes.");
}

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters =
            new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,

                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience = builder.Configuration["Jwt:Audience"],

                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(jwtKey)),

                NameClaimType = ClaimTypes.Name,
                RoleClaimType = ClaimTypes.Role,

                ClockSkew = TimeSpan.Zero
            };

        // Reject tokens belonging to disabled/deleted accounts
        // or accounts whose roles have changed.
        options.Events = new JwtBearerEvents
        {
            OnTokenValidated = async context =>
            {
                var userIdText = context.Principal?
                    .FindFirstValue(ClaimTypes.NameIdentifier);

                if (!Guid.TryParse(userIdText, out var userId))
                {
                    context.Fail("Invalid user.");
                    return;
                }

                var db = context.HttpContext.RequestServices
                    .GetRequiredService<ApplicationDbContext>();

                var user = await db.Users
                    .AsNoTracking()
                    .Include(x => x.UserRoles)
                        .ThenInclude(x => x.Role)
                    .FirstOrDefaultAsync(
                        x => x.Id == userId,
                        context.HttpContext.RequestAborted);

                if (user == null || !user.IsActive)
                {
                    context.Fail("Account is inactive.");
                    return;
                }

                var tokenRoles = context.Principal!
                    .FindAll(ClaimTypes.Role)
                    .Select(x => x.Value)
                    .OrderBy(x => x)
                    .ToArray();

                var currentRoles = user.UserRoles
                    .Select(x => x.Role.Name)
                    .OrderBy(x => x)
                    .ToArray();

                if (!tokenRoles.SequenceEqual(currentRoles))
                {
                    context.Fail("Roles have changed.");
                }
            }
        };
    });

builder.Services.AddAuthorization();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer",
        new Microsoft.OpenApi.Models.OpenApiSecurityScheme
        {
            Name = "Authorization",
            Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
            In = Microsoft.OpenApi.Models.ParameterLocation.Header,
            Description = "Enter your JWT token."
        });

    options.AddSecurityRequirement(
        new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
        {
            {
                new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                {
                    Reference =
                        new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type =
                                Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                },
                Array.Empty<string>()
            }
        });
});

builder.Services.AddProblemDetails();

builder.Services.AddExceptionHandler<
    GlobalExceptionHandler>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
