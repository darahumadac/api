

using api.Database;
using api.Endpoints;
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


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapEmployeesApi();




app.Run();
