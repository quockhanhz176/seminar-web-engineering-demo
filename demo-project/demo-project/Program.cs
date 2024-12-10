using demo_project.Models;
using Microsoft.AspNetCore.OData;
using Microsoft.OData.ModelBuilder;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

//builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var modelBuilder = new ODataConventionModelBuilder();
modelBuilder.EntitySet<Group>("Groups");
modelBuilder.EntitySet<Student>("Students");
//modelBuilder.EnableLowerCamelCase();

builder.Services.AddControllers().AddOData(
    options => { 
        options.EnableQueryFeatures(maxTopValue: null).AddRouteComponents(modelBuilder.GetEdmModel());
    });

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseODataRouteDebug();

app.UseRouting();

//app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

app.Run();
