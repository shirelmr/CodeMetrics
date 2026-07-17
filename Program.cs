using System.Text;
using FluentValidation;
using MetricsAPI.Data;
using MetricsAPI.Models;
using MetricsAPI.Repositories;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


builder.Services.AddScoped<IRepositoryRepository, RepositoryRepository>();
builder.Services.AddAutoMapper(typeof(Program));

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token here"
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
            ValidAudience = builder.Configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]!))
        };
    });

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

// Seed database on startup if empty
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    context.Database.EnsureCreated();

    // Create admin user if none exists
    if (!context.Users.Any())
    {
        var adminUser = new User
        {
            Email = "admin@codemetrics.com",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("Admin123!"),
            Role = "Admin"
        };
        context.Users.Add(adminUser);
        context.SaveChanges();
    }

    // Seed repos assigned to the admin user
    if (!context.Repositories.Any())
    {
        var adminId = context.Users.First().Id;

        var seedRepos = new List<Repository>
        {
            new() { Name = "CodeMetricsPro", Url = "https://github.com/you/codemetricspro", Language = "C#", CreatedAt = DateTime.UtcNow, UserId = adminId },
            new() { Name = "TodoApi", Url = "https://github.com/you/todoapi", Language = "C#", CreatedAt = DateTime.UtcNow, UserId = adminId },
            new() { Name = "WeatherService", Url = "https://github.com/you/weatherservice", Language = "C#", CreatedAt = DateTime.UtcNow, UserId = adminId },
            new() { Name = "ReactDashboard", Url = "https://github.com/you/reactdashboard", Language = "JavaScript", CreatedAt = DateTime.UtcNow, UserId = adminId },
            new() { Name = "VueShop", Url = "https://github.com/you/vueshop", Language = "JavaScript", CreatedAt = DateTime.UtcNow, UserId = adminId },
            new() { Name = "DataPipeline", Url = "https://github.com/you/datapipeline", Language = "Python", CreatedAt = DateTime.UtcNow, UserId = adminId },
            new() { Name = "MLClassifier", Url = "https://github.com/you/mlclassifier", Language = "Python", CreatedAt = DateTime.UtcNow, UserId = adminId },
            new() { Name = "SpringBootApi", Url = "https://github.com/you/springbootapi", Language = "Java", CreatedAt = DateTime.UtcNow, UserId = adminId },
            new() { Name = "RustCli", Url = "https://github.com/you/rustcli", Language = "Rust", CreatedAt = DateTime.UtcNow, UserId = adminId },
            new() { Name = "GoMicroservice", Url = "https://github.com/you/gomicroservice", Language = "Go", CreatedAt = DateTime.UtcNow, UserId = adminId },
            new() { Name = "NextjsBlog", Url = "https://github.com/you/nextjsblog", Language = "TypeScript", CreatedAt = DateTime.UtcNow, UserId = adminId },
            new() { Name = "GraphqlServer", Url = "https://github.com/you/graphqlserver", Language = "TypeScript", CreatedAt = DateTime.UtcNow, UserId = adminId },
            new() { Name = "DockerOrchestrator", Url = "https://github.com/you/dockerorchestrator", Language = "Go", CreatedAt = DateTime.UtcNow, UserId = adminId },
            new() { Name = "AndroidApp", Url = "https://github.com/you/androidapp", Language = "Kotlin", CreatedAt = DateTime.UtcNow, UserId = adminId },
            new() { Name = "IosClient", Url = "https://github.com/you/iosclient", Language = "Swift", CreatedAt = DateTime.UtcNow, UserId = adminId },
        };

        context.Repositories.AddRange(seedRepos);
        context.SaveChanges();
    }
}
app.Run();