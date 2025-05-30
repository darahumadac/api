

using api.Database;
using api.Endpoints;
using api.Services;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var appDbConnectionString = builder.Configuration.GetConnectionString("AppDb")
    ?? throw new InvalidOperationException("No AppDB connection string found");
var appDbConnectionStringBuilder = new SqlConnectionStringBuilder(appDbConnectionString);
//build the connection string because we don't want to store sensitive info in appsettings.json
if (!builder.Environment.IsDevelopment() || !appDbConnectionStringBuilder.IntegratedSecurity)
{
    //get the credentials from the env variables
    builder.Configuration.AddEnvironmentVariables(prefix: "APP_");
    appDbConnectionStringBuilder.UserID = builder.Configuration["SQLDB_USER"];
    appDbConnectionStringBuilder.Password = builder.Configuration["SQLDB_PASSWORD"];
}
builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(appDbConnectionStringBuilder.ConnectionString));

//TODO: add sanitation/html encoding/normalization for data
//TODO: Add serilog
//TODO: Add caching and etag / concurrency check

builder.Services.AddProblemDetails();
builder.Services.AddEmployeeServices();
builder.Services.AddCompanyServices();

var app = builder.Build();

app.UseExceptionHandler("/error");
app.Map("/error", (HttpContext context, ILogger<Program> logger) =>
{
    var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();

    var ex = exceptionHandlerPathFeature?.Error;
    if (ex != null)
    {
        logger.LogError(ex, "Unhandled exception occurred on path: {Path}", exceptionHandlerPathFeature?.Endpoint);
    }

    var problem = ex switch
    {
        DbUpdateException => 
            ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627) ? 
            Results.Problem(statusCode: StatusCodes.Status409Conflict, detail: "This record already exists. Check if you've already created it.") 
            : Results.Problem(statusCode: StatusCodes.Status500InternalServerError, detail: "Error encountered while saving data update. Please try again later."),
        
        BadHttpRequestException => Results.Problem(statusCode: StatusCodes.Status400BadRequest, detail: "The request was not understood. Please ensure your input is properly formatted"),
        
        _ => Results.Problem("An unexpected error occurred. Please try again later.")
    };

    return problem;
});

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

//API Endpoints
app.MapEmployeesApi();
app.MapCompanyApi();


app.Run();
