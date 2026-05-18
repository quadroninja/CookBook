using CookBookBackend.Api.Controllers;
using CookBookBackend.Api.Controllers.ModelBinders;
using CookBookBackend.Api.DTO;
using CookBookBackend.Api.DTO.Dish;
using CookBookBackend.Api.DTO.FoodItem;
using CookBookBackend.Core.Services;
using CookBookBackend.Data;
using CookBookBackend.Data.Models;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.IO;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddOpenApi();
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddAutoMapper(cfg => { }, typeof(Program));

builder.Services.AddScoped<FoodItemService, FoodItemService>();
builder.Services.AddScoped<DishService, DishService>();
builder.Services.AddScoped<PhotoService, PhotoService>();
builder.Services.AddScoped<NutritionFactsService, NutritionFactsService>();
builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddHttpContextAccessor();
builder.Services.AddControllers(options =>
{
    options.ModelBinderProviders.Insert(0, new DecimalPointModelBinderProvider());
})
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());

    });
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("null") // локальный запуск (origin - file://)
              .AllowAnyHeader()
              .AllowAnyMethod();
        policy.WithOrigins("http://127.0.0.1:5500") // Live Server в VS 
              .AllowAnyHeader()
              .AllowAnyMethod();

        policy.WithOrigins("http://localhost:5500") // Live Server в VS 
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});


var app = builder.Build();



if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "Swagger");
    });
}


app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseCors();
app.MapControllers();

app.Run();

public partial class Program { }
