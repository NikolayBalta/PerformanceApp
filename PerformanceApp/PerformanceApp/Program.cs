using Azure.Storage.Blobs.Models;
using Azure.Storage.Blobs;
using Npgsql;
using PerformanceApp.Entities;
using PerformanceApp.Models;
using PerformanceApp.Utilites;
using System.Data;
using System.Reflection;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using System.IO;

var builder = WebApplication.CreateBuilder(args);
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
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
BlobServiceClient blobServiceClient = new BlobServiceClient("DefaultEndpointsProtocol=https;AccountName=saperformanceapp;AccountKey=pLypAvaUH7nOmAFHleXv8LS/aYGW6vlLDmpyhBYPDiCANPOR9PIA8dBQQXYtofNxnUqZzu3w6c+Z+ASto0bTaQ==;EndpointSuffix=core.windows.net");
BlobContainerClient containerClient = blobServiceClient.GetBlobContainerClient("documents");

[PreJit]
IResult GetChallenge() => Results.Json(new { message = "The challenge accepted!!!" });

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
[PreJit]
static IEnumerable<int> GenerateFibonacci(int n)
{
    if (n <= 0)
        yield break;

    int a = 0, b = 1;

    for (int i = 0; i < n; i++)
    {
        yield return a;
        int next = a + b;
        a = b;
        b = next;
    }
}
[PreJit]
static string ReverseWords(ReverseWordsModel sentence)
{
    var sb = new StringBuilder();
    int start = 0;

    for (int i = 0; i < sentence.Sentence.Length; i++)
    {
        if (sentence.Sentence[i] != ' ' && i != sentence.Sentence.Length - 1) continue;
        int end = i == sentence.Sentence.Length - 1 ? i : i - 1;
        sb.Insert(0, sentence.Sentence.Substring(start, end - start + 1) + " ");
        start = i + 1;
    }

    return sb.ToString().TrimEnd();
}
[PreJit]
async Task<string> ReadTextFileFromBlobStorageAsync(string fileName)
{
    
    BlobClient blobClient = containerClient.GetBlobClient(fileName);

    BlobDownloadInfo blobDownloadInfo = await blobClient.DownloadAsync();

    using StreamReader reader = new StreamReader(blobDownloadInfo.Content);
    string content = await reader.ReadToEndAsync();
    return content;
}
[PreJit]
async Task<FileContentResult> DownloadFile(string fileName)
{
    BlobClient blobClient = containerClient.GetBlobClient(fileName);

    BlobDownloadInfo blobDownloadInfo = await blobClient.DownloadAsync();

    var memoryStream = new MemoryStream();
    await blobDownloadInfo.Content.CopyToAsync(memoryStream);
    memoryStream.Seek(0, SeekOrigin.Begin);

    return new FileContentResult(memoryStream.ToArray(), "application/octet-stream")
    {
        FileDownloadName = fileName
    };
}
app.MapGet("/challenge", GetChallenge)
    .WithName("challenge");

app.MapGet("/products", GetData)
    .WithName("products");

app.MapGet("/products/{productId}", GetProductById)
    .WithName("productsById");

app.MapGet("/factorial", CalculateFactorial).WithName("factorial");

app.MapGet("/fibonacci", GenerateFibonacci).WithName("fibonacci");

app.MapPost("/sortArray", SortArray).WithName("sortArray");

app.MapPost("/reverseWords", ReverseWords).WithName("reverseWords");

app.MapPost("/textFromFile", ReadTextFileFromBlobStorageAsync).WithName("textFromFile");

app.MapPost("/downloadFile", DownloadFile).WithName("download");

app.Run();
