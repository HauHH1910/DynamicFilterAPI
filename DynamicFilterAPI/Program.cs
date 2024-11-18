using DynamicFilterAPI.Databases;
using DynamicFilterAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

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

//TODO: Watch the tutorial 
app.MapGet("/products", async ([FromBody] ProductSearchCriteria productSearch, ProductDbContext dbContext) =>
{
    await dbContext.Database.EnsureCreatedAsync();

    var parameterExp = Expression.Parameter(typeof(Product), "x");
    Expression predicate = Expression.Constant(true); //True &&

    if (productSearch.IsActive.HasValue)
    {
        //x.IsActive=true
        MemberExpression memberExpression = Expression.Property(parameterExp, nameof(Product.isActive));
        ConstantExpression constantExpression = Expression.Constant(productSearch.IsActive.Value);
        BinaryExpression binaryExpression = Expression.Equal(memberExpression, constantExpression);

        predicate = Expression.AndAlso(predicate, binaryExpression);
    }

    if (productSearch.Categories is not null && productSearch.Categories.Any())
    {
        //x.Category
        MemberExpression memberExpression = Expression.Property(parameterExp, nameof(Product.Category));
        Expression orExpression = Expression.Constant(false);
        foreach (var category in productSearch.Categories)
        {
            var constExp = Expression.Constant(category.Name);
            BinaryExpression binary = Expression.Equal(memberExpression, constExp);
            orExpression = Expression.OrElse(orExpression, binary);
        }

        predicate = Expression.AndAlso(predicate, orExpression);
    }

    if (productSearch.Names is not null && productSearch.Names.Any())
    {
        //x.Name
        MemberExpression memberExpression = Expression.Property(parameterExp, nameof(Product.Name));
        Expression orExpression = Expression.Constant(false);
        foreach (var name in productSearch.Names)
        {
            var constExp = Expression.Constant(name.Name);
            BinaryExpression binary = Expression.Equal(memberExpression, constExp);
            orExpression = Expression.OrElse(orExpression, binary);
        }
        
        predicate = Expression.AndAlso(predicate, orExpression);
    }

    if (productSearch.Price is not null)
    {
        //x.Price >= min
        MemberExpression memberExpression = Expression.Property(parameterExp, nameof(Product.Price));
        if (productSearch.Price.Min is not null)
        {
            var constantExpression = Expression.Constant(productSearch.Price.Min);
            var binaryExpression = Expression.GreaterThanOrEqual(memberExpression, constantExpression);
            
            predicate = Expression.AndAlso(predicate, binaryExpression);
        }

        //x.Price >= min && x.Price <= Max
        if (productSearch.Price.Max is not null)
        {
            var constantExpression = Expression.Constant(productSearch.Price.Max);
            var binaryExpression = Expression.LessThanOrEqual(memberExpression, constantExpression);
            
            predicate = Expression.AndAlso(predicate, binaryExpression);
        }
    }

    var lambdaExpression = Expression.Lambda<Func<Product, bool>>(predicate, parameterExp);
    var data = await dbContext.Products.Where(lambdaExpression).ToListAsync();

    return Results.Ok(data);
});
app.UseHttpsRedirection();

app.Run();
