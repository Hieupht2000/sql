using CarManagetment.Config;
using CarManagetment.Data;
using CarManagetment.Middleware;
using CarManagetment.Services;
using CarManagetment.Services.Invoices;
using CarManagetment.Services.Notification;
using Hangfire;
using Hangfire.SqlServer;
using Microsoft.AspNetCore.Authentication.JwtBearer;

using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<BookingQueueService>();
builder.Services.AddScoped<IBookingService, BookingService>();
builder.Services.AddScoped<EmailService>();
builder.Services.AddScoped<EmailServiceCustomer>();
builder.Services.AddScoped<IEmailServiceCustomer, EmailServiceCustomer>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IServiceService, ServiceService>();
builder.Services.AddScoped<INotificationService,TelegramNotificationService>();
builder.Services.AddScoped<TelegramService>();
builder.Services.Configure<TelegramSettings>(builder.Configuration.GetSection("Telegram"));

builder.Services.AddScoped<PdfInvoiceService>();

builder.Services.AddDbContext<CarDBContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DBCS")));
builder.Services.Configure<EmailSettings>(builder.Configuration.GetSection("EmailSettings"));
builder.Services.AddTransient<IEmailBookingService, EmailService>();

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Add("http://schemas.microsoft.com/ws/2008/06/identity/claims/role", ClaimTypes.Role);

// Add Hangfire services
builder.Services.AddHangfire(config =>
{
    config.SetDataCompatibilityLevel(CompatibilityLevel.Version_170)
          .UseSimpleAssemblyNameTypeSerializer()
          .UseRecommendedSerializerSettings()
          .UseSqlServerStorage(builder.Configuration.GetConnectionString("DBCS"), new SqlServerStorageOptions
          {
              CommandBatchMaxTimeout = TimeSpan.FromMinutes(5),
              SlidingInvisibilityTimeout = TimeSpan.FromMinutes(5),
              QueuePollInterval = TimeSpan.Zero,
              UseRecommendedIsolationLevel = true,
              DisableGlobalLocks = true,
              JobExpirationCheckInterval = TimeSpan.FromHours(1) // Kiểm tra job hết hạn
          });
});


builder.Services.AddHangfire(x => x.UseSqlServerStorage(builder.Configuration.GetConnectionString("DBCS")));
builder.Services.AddHangfireServer();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = "http://localhost:7225/", // like _issuer
            ValidAudience = "http://localhost:7225/", // like _audience
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"])), // like _secretKey
            RoleClaimType = ClaimTypes.Role, // ← Quan trọng
            //NameClaimType= ClaimTypes.Name, // ← Quan trọng
            NameClaimType = ClaimTypes.NameIdentifier, // cho chắc

        };
    });
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Car Management API", Version = "v1" });

    // Thêm cấu hình Authorize
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Nhập token theo format: Bearer {token}"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
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


builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor(); // Add this line to enable IHttpContextAccessor
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseStatusCodePages();
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseMiddleware<FullNameMiddleware>();
app.UseAuthorization();
app.UseHangfireDashboard();
app.MapControllers();

app.Run();
