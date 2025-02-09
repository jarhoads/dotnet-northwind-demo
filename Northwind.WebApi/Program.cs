using Microsoft.AspNetCore.Mvc.Formatters;
using Northwind.Shared; // AddNorthwindContext extension method
using Northwind.WebApi.Repositories;
using Swashbuckle.AspNetCore.SwaggerUI; // SubmitMethod
using Microsoft.AspNetCore.HttpLogging; // HttpLoggingFields
using static System.Console;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.WebHost.UseUrls("https://localhost:5002/");

builder.Services.AddHttpLogging(options =>
{
    options.LoggingFields = HttpLoggingFields.All;
    options.RequestBodyLogLimit = 4096; // default is 32k
    options.ResponseBodyLogLimit = 4096; // default is 32k
});

builder.Services.AddNorthwindContext();

builder.Services.AddControllers(options => 
{
    WriteLine("Default output formatters:");
    foreach (IOutputFormatter formatter in options.OutputFormatters)
    {
        OutputFormatter? mediaFormatter = formatter as OutputFormatter;
        if (mediaFormatter == null)
        {
            WriteLine($" {formatter.GetType().Name}");
        }
        else
        {
            // OutputFormatter class has SupportedMediaTypes
            WriteLine(" {0}, Media types: {1}", arg0: mediaFormatter.GetType().Name, arg1: string.Join(", ", mediaFormatter.SupportedMediaTypes));
        }
    }
})
.AddXmlDataContractSerializerFormatters()
.AddXmlSerializerFormatters();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new() { Title = "Northwind Service API", Version = "v1" });
});

// register repository as a service
builder.Services.AddScoped<ICustomerRepository, CustomerRepository>();

builder.Services.AddCors();

// DB health check
// By default, the database context check calls EF Core's 
// CanConnectAsync method. You can customize what operation is run 
// by calling the AddDbContextCheck method
builder.Services.AddHealthChecks()
                .AddDbContextCheck<NorthwindContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.

app.UseHttpLogging();

app.UseCors(configurePolicy: options =>
{
    options.WithMethods("GET", "POST", "PUT", "DELETE");
    // allow requests from the MVC client
    options.WithOrigins("https://localhost:5001");
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json",
        "Northwind Service API Version 1");
        c.SupportedSubmitMethods(new[] {
            SubmitMethod.Get, 
            SubmitMethod.Post,
            SubmitMethod.Put, 
            SubmitMethod.Delete 
        });
    });
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseHealthChecks(path: "/howdoyoufeel");

app.MapControllers();

app.Run();
