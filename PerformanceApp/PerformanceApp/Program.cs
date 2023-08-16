using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PerformanceApp.Api;
using PerformanceApp.Models;
using PerformanceApp.Utilites;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
app.UseSwagger();
app.UseSwaggerUI(c=>
{
    c.DisplayRequestDuration();
});
app.UseHttpsRedirection();
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

app.MapGet("/start", GetChallenge)
    .WithName("Start");

app.MapPost("/sort", SortArray)
    .WithName("Sort");
app.Run();
