using System;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Npgsql;
using PerformanceApp.Api;
using PerformanceApp.Context;
using PerformanceApp.Entities;
using PerformanceApp.Models;
using PerformanceApp.Utilites;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<PerformanceContext>(options =>
{
    options.UseNpgsql(connectionString);
});
var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c=>
{
    c.DisplayRequestDuration();
});
app.UseHttpsRedirection();

PerformanceContext context;
var scope = builder.Services.BuildServiceProvider().CreateScope();
context = scope.ServiceProvider.GetRequiredService<PerformanceContext>();


    Jitter.PreJit(Assembly.GetAssembly(typeof(Program))!);
[PreJit]
IResult GetChallenge()
{
    var responseObject = new { message = "The challenge accepted!!!" };
    return Results.Json(responseObject);
}
[PreJit]
IResult SortArray(SortingModel model)
{
    //var responseObject = new { integers = model.Integers.OrderBy(x => x) };
    QuickSort.Sort(model.Integers, 0, model.Integers.Length - 1);
    return Results.Json(model.Integers);

}
[PreJit]
IResult GetData(int minValue, int maxValue)
{

    var list = new List<Product>();

    using (var conn = new NpgsqlConnection(connectionString))
    {
        try
        {
            conn.Open();
            using var cmd = new NpgsqlCommand();
            cmd.Connection = conn;
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = $"SELECT * FROM public.\"Products\" WHERE \"productPrice\" BETWEEN {minValue} AND {maxValue};\r\n";

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Product
                {
                    Id = reader.GetInt32(0),
                    ProductName = reader.GetString(1),
                    ProductPrice = reader.GetFloat(2),
                    ProductDescription = reader.GetString(3)
                });
            }
        }
        catch (Exception exp)
        {
            return Results.BadRequest(exp);
        }
    }

    return Results.Ok(list);
}
[PreJit]
IResult GetProductById(int productId)
{
    using var conn = new NpgsqlConnection(connectionString);
    try
    {
        conn.Open();
        using var cmd = new NpgsqlCommand();
        cmd.Connection = conn;
        cmd.CommandType = CommandType.Text;
        cmd.CommandText = $"SELECT * FROM public.\"Products\" WHERE \"id\" = {productId};";

        using var reader = cmd.ExecuteReader();
        if (!reader.Read()) return Results.NotFound("Product not found");
        var product = new Product
        {
            Id = reader.GetInt32(0),
            ProductName = reader.GetString(1),
            ProductPrice = reader.GetFloat(2),
            ProductDescription = reader.GetString(3)
        };

        return Results.Ok(product);

    }
    catch (Exception exp)
    {
        return Results.BadRequest(exp);
    }
}
[PreJit]
static long CalculateFactorial(int n)
{
    if (n == 0 || n == 1)
    {
        return 1;
    }
    return n * CalculateFactorial(n - 1);
}

app.MapGet("/Start", GetChallenge)
    .WithName("Start");

app.MapPost("/Sort", SortArray)
    .WithName("Sort");

app.MapGet("/Data", GetData)
    .WithName("GetData");

app.MapGet("/Data/{productId}", GetProductById)
    .WithName("GetDataById");

app.MapGet("/Factorial", CalculateFactorial).WithName("CalculateFactorial");



app.Run();
