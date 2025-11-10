using codebridge.BLL.Services;
using codebridge.BLL.Services.Interfaces;
using codebridge.Common.Data;
using codebridge.Common.Helpers;
using codebridge.DAL.Repositories;
using codebridge.DAL.Repositories.Interfaces;
using Codebridge.API.Extensions;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<DogsDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        b => b.MigrationsAssembly("Codebridge.API")));

builder.Services.AddControllers();
builder.Services.AddOpenApi();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", builder =>
    {
        builder.SetIsOriginAllowed(_ => true)
                   .AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader()
                   .SetPreflightMaxAge(TimeSpan.FromDays(1d));
    });
});

builder.Services.AddServiceExtensions();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}
app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
