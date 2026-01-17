using MES.Application.Interfaces;
using MES.Application.Services;
using System.Text;
using MES.Infrastructure.Persistence;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.IdentityModel.Tokens;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =======================================
// DATABASE
// =======================================
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrWhiteSpace(connectionString))
{
    throw new InvalidOperationException(
        "Missing connection string. Set ConnectionStrings__DefaultConnection via environment variables or user secrets.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        b => b.MigrationsAssembly("MES.Infrastructure")
    )
);

// IApplicationDbContext → AppDbContext
builder.Services.AddScoped<IApplicationDbContext>(sp =>
    sp.GetRequiredService<AppDbContext>());

// =======================================
// APPLICATION SERVICES
// =======================================
builder.Services.AddScoped<WorkOrderService>();
builder.Services.AddScoped<ProductionResultService>();
builder.Services.AddScoped<LineKpiService>();
builder.Services.AddScoped<ShiftTimeService>();
builder.Services.AddScoped<DowntimeService>();
builder.Services.AddScoped<ScanService>();
builder.Services.AddScoped<OeeService>();
builder.Services.AddScoped<WorkerService>();
builder.Services.AddScoped<LineOverviewService>();
builder.Services.AddScoped<UnitTraceService>();
builder.Services.AddScoped<WorkOrderQueryService>();
builder.Services.AddScoped<UnitQueryService>();
builder.Services.AddScoped<OperatorStatsService>();

// =======================================
// AUTHENTICATION & AUTHORIZATION
// =======================================
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
var jwtKey = builder.Configuration["Jwt:SigningKey"];

if (string.IsNullOrWhiteSpace(jwtIssuer)
    || string.IsNullOrWhiteSpace(jwtAudience)
    || string.IsNullOrWhiteSpace(jwtKey))
{
    throw new InvalidOperationException(
        "Missing JWT settings. Set Jwt__Issuer, Jwt__Audience, and Jwt__SigningKey in configuration.");
}

var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));

builder.Services
    .AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwtIssuer,
            ValidAudience = jwtAudience,
            IssuerSigningKey = signingKey
        };
    });

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy => policy.RequireRole("Admin"));
});

// =======================================
// CONTROLLERS & SWAGGER
// =======================================
builder.Services.AddControllers(options =>
{
    options.Filters.Add(new AuthorizeFilter());
});
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new()
    {
        Name = "Authorization",
        Type = Microsoft.OpenApi.Models.SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Description = "Enter 'Bearer {token}'."
    });
    options.AddSecurityRequirement(new()
    {
        {
            new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Reference = new Microsoft.OpenApi.Models.OpenApiReference
                {
                    Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// =======================================
// BUILD APP
// =======================================
var app = builder.Build();

// =======================================
// MIDDLEWARE
// =======================================
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// =======================================
// DATABASE SEED
// =======================================
if (app.Environment.IsDevelopment() || app.Environment.IsEnvironment("Test"))
{
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await AppDbContextSeed.SeedAsync(db);
}

app.Run();
