using DynamicFilterAPI.Databases;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ProductDbContext>(x => x.UseInMemoryDatabase("ProductDb"));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//TODO: Watch the tutorial   https://www.youtube.com/watch?v=lkZxn76w88U&t=39s
app.MapGet("/products", async (ProductDbContext dbContext) =>
{ 

});
app.UseHttpsRedirection();

app.Run();
