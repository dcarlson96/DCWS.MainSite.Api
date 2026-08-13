using DCWS.MainSite.Api.Domain.Contracts;
using DCWS.MainSite.Api.Domain.Services;

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

builder.Services.AddSingleton<IStatusService, StatusService>();

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