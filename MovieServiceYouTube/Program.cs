using System;
using DomainLayer;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using MovieServiceCore3.Middleware;

var builder = WebApplication.CreateBuilder(args);

// Clear logging providers as per original logic
builder.Logging.ClearProviders();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddSingleton<DomainFacade>();

var app = builder.Build();

app.UseCustomExceptionHandling();

app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapRazorPages();
app.MapControllers();

app.Run();