using System.Threading.RateLimiting;
using DCWS.MainSite.Api.Domain;
using DCWS.MainSite.Api.Domain.Clients;
using DCWS.MainSite.Api.Domain.Configuration;
using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.Repositories;
using DCWS.MainSite.Api.Domain.Services;
using DCWS.MainSite.Api.Web.Configuration;
using DCWS.MainSite.Api.Web.Services;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.Configure<TestimonialsOptions>(
    builder.Configuration.GetSection(TestimonialsOptions.SectionName));
builder.Services.Configure<SmtpOptions>(
    builder.Configuration.GetSection(SmtpOptions.SectionName));
builder.Services.AddSingleton(TimeProvider.System);

builder.Services.AddCors(options =>
{
    options.AddPolicy("MainSite", policy =>
    {
        policy
            .WithOrigins(builder.Configuration
                .GetSection("Cors:AllowedOrigins")
                .Get<string[]>() ?? [])
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});

builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
    options.AddPolicy("TestimonialSubmission", context =>
        RateLimitPartition.GetFixedWindowLimiter(
            context.Connection.RemoteIpAddress?.ToString() ?? "unknown",
            _ => new FixedWindowRateLimiterOptions
            {
                PermitLimit = 5,
                Window = TimeSpan.FromHours(1),
                QueueLimit = 0,
                AutoReplenishment = true
            }));
});

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("The 'DefaultConnection' connection string is not configured.")));

builder.Services.AddScoped<IStatusRepository, StatusRepository>();
builder.Services.AddScoped<IStatusService, StatusService>();

builder.Services.AddHttpClient<IUsGeocoderClient, UsGeocoderClient>(client =>
{
    client.BaseAddress = new Uri("https://geocoding.geo.census.gov/");
});
builder.Services.AddScoped<IAddressService, AddressService>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();
builder.Services.AddScoped<ITestimonialService, TestimonialService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("MainSite");
app.UseRateLimiter();

app.MapControllers();

app.Run();
