using DCWS.MainSite.Api.Domain;
using DCWS.MainSite.Api.Domain.Clients;
using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.Repositories;
using DCWS.MainSite.Api.Domain.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(options =>
{
    options.AddPolicy("MainSite", policy =>
    {
        policy
            .WithOrigins(
                "https://dcwebsystems.com",
                "https://www.dcwebsystems.com")
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors("MainSite");

app.MapControllers();

app.Run();