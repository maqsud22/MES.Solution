using MES.Application.Interfaces;
using MES.Application.Services;
using MES.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// =======================================
// DATABASE
// =======================================
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
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
// CONTROLLERS & SWAGGER
// =======================================
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// =======================================
// BUILD APP
// =======================================
var app = builder.Build();

// =======================================
// MIDDLEWARE
// =======================================
app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.MapControllers();

// =======================================
// DATABASE SEED
// =======================================
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await AppDbContextSeed.SeedAsync(db);
}

app.Run();
