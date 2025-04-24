# API Development - .NET Core Web API (Minimal API)

## Project Setup

1. Create solution and web api project

   ```bash
   dotnet new sln
   mkdir <project_name> && cd <project_name>
   dotnet new webapi
   ```

2. Identify the resources and draft the **endpoints** and **handlers** for them: `GET`, `POST`, `PUT`, `DELETE`.

   - Make sure to use `TypedResults` and `Task<>
   - Create an extension class for the resource api and handlers so that endpoints are organized and so that the handlers can be accessed by the unit test

   ```c#
   //EmployeeExtensions.cs
   public static partial class EmployeesEndpoints {
       public static void MapEmployeesApi(this IEndpointRouteBuilder app)
       {
           //create group for the resource
           var employees = app.MapGroup("/employees");
           //add the enpoints for the resource group
           employees.MapGet("/", HandlerGetEmployeesAsync).WithName("GetEmployees");
           employees.MapGet("/{id}", HandlerGetEmployeeAsync).WithName("GetEmployeeById");

           var employee = app.MapGroup("/employee");
           employee.MapPost("/", HandlerAddEmployeeAsync).WithName("AddEmployee");
           employee.MapPut("/{id}", HandlerUpdateEmployee).WithName("UpdateEmployeeById");
           employee.MapDelete("/{id}", HandlerDeleteEmployee).WithName("DeleteEmployeeById");
       }
   }

   //EmployeeHandlers.cs
   public static partial class EmployeesEndpoints {
       public static async Task<Ok<List<EmployeesResponse>>> GetEmployeesAsync(){
            var result = new List<EmployeesResponse>{new EmployeesResponse("A"), new EmployeesResponse("B")}
           return await Task.FromResult(TypedResults.Ok(result));
       }
   }
   ```

3. Test the endpoints and make sure they are working. Add the endpoints to the `<project_name>.http` file and test the endpoints manually.

4. Set up unit tests for the API. Use `NUnit` and create the test methods for the api handlers. Ensure that the handlers can be called and the types are asserted

5. Create the database and the models. Add `Entity Framework` packages, then add 2 folders - `Database` and `Models`
   - Models should have `CreatedDate`, `UpdatedDate`, `RowVersion` (for concurrency). Create an `Auditable.cs` base class for this. This is the base model.
   - Draft the Models (for the resources) for the database.
   - Make sure to add the field constraints
   - Create the `AppDbContext.cs` file for the database and create `DbSet<TModel>` properties.
   - Configure the `AppDbContext` in `Program.cs`
   - ==Make sure the connection string is built using `SqlConnectionStringBuilder` if it is not for Development environment. Get the credentials from the env variables==
   - Model the database further - by overriding `OnModelCreating(ModelBuilder builder)`
   - Add `HasValueGenerator<TValueGenerator>` to property to configure the value for the property when it is null
   - Add `HasDefaultSqlValue("GETUTCDATE()")` to set created / updated date time properties when they are null
   - Add `HasDefaultSqlValue("NEWID()")` for generating GUID in db
   - Add `IsRowVersion` to configure the property for concurrency checks
   - Add `HasConversion` to convert the data type of the property to and from the model. This is helpful when you have properties that you want to save to a different type in the db, but use a different type in the model

     ```c#
             modelBuilder.Entity<Company>
             .Property(c => c.Id)
             .HasConversion(
                 v => Guid.Parse(v), //convert to guid when saving to db
                 v => v.ToString() //convert from guid (to string) when reading from db
             )`
     ```

   - Configuring entity types by reflection when they inherit from the same base class:

   ```c#
        //configure Auditable for each entity inheriting from it
        foreach(var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if(typeof(Entity).IsAssignableFrom(clrType) && clrType != typeof(Entity))
            {
                modelBuilder.Entity(clrType)
                    .Property(nameof(Entity.CreatedDate))
                    .HasDefaultValueSql("GETUTCDATE()");
                modelBuilder.Entity(clrType)
                    .Property(nameof(Entity.UpdatedDate))
                    .HasDefaultValueSql("GETUTCDATE()");
                modelBuilder.Entity(clrType)
                    .Property(nameof(Entity.ETag))
                    .IsRowVersion();
            }
        }
   ```

   - Seed the database - by overriding `OnConfiguring(DbContextOptionsBuilder optionsBuilder)`
   - Run the migrations and update the database

```bash
# add the entity framework packages
dotnet add Microsoft.EntityFrameworkCore.SqlServer
dotnet add Microsoft.EntityFrameworkCore.Design
dotnet add Microsoft.EntityFrameworkCore.Tools

#add new directories - Database and Models
mkdir Database Models
```

### Creating Models

- Ensure they have `CreatedDate`, `UpdatedDate`, `byte[] ETag` properties for audit and concurrency checks
- Ensure that **properties with value generation has default value of null / are uninitialized**
- Ensure that **constraints for the properties** are configured using `ComponentModel.DataAnnotations`
- Ensure that the **indexes** and **unique indexes** are created e.g. How to determine whether a record for the model is duplicated i.e. what is the Unique Index for the model?
- Ensure that **navigation properties and the navigation property id** is present in the model if there are any dependencies
- Ensure that properties that can be computed are included in the model, but are not mapped

### Creating Services
- Create an interface `IRepository<T>` to contain all the methods for GetAll(),Get(), Add(), Update(), Delete()
```c#
// IRepository.cs
public interface IRepository<T> where T : class
{
    //GET

    //POST

    //PUT

    //DELETE
}
```
- Implement the `IRepository` interface for each service and register the service in `Program.cs`. Create an extension class for the services to be more organized
```c#
// EmployeeService.cs
public class EmployeeService : IRepository<Employee>
{
    private readonly AppDbContext dbContext;
    public EmployeeService(AppDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    // IRepository implementation here...
}

//Program.cs - Register the service
app.Services.AddScoped<IRepositoryService<Employee>, EmployeeService>();

//Optional: Create an extension method for adding services
public static class ServiceExtensions
{
    public static IServiceCollection AddEmployeeServices(this IServiceCollection services)
    {
        services.AddScoped<IRepositoryService<Employee>, EmployeeService>();
        return services;
    }
}
```

#### IRepository<T>

- Pass a condition/query/filter to a method as `Expression<Func<T, bool> condition> condition` when using it to query the `DbSet<T>` so that it can be passed to the where clause as `.Where(conditon)`

## Testing APIs
[Reference](https://learn.microsoft.com/en-us/aspnet/core/fundamentals/minimal-apis/test-min-api?view=aspnetcore-9.0)
- Create unit test for each **Service implementation** by configuring and using In-Memory database
    - (Reference)[https://github.com/dotnet/AspNetCore.Docs.Samples/blob/main/fundamentals/minimal-apis/samples/MinApiTestsSample/UnitTests/TodoInMemoryTests.cs]
- Create unit test for the **API Endpoint Handler methods** by mocking the service implementations using `Moq`
    - [Reference](https://github.com/dotnet/AspNetCore.Docs.Samples/blob/main/fundamentals/minimal-apis/samples/MinApiTestsSample/UnitTests/TodoMoqTests.cs)
- Create integration tests for the API endpoints using `WebApplicationFactory<TEntrypoint>` and `HttpClient`. This is where query strings are tested

### Docker

- When connecting to a docker container for SQL Server, make sure the `Data Source` in the connection string is as below if connecting to `localhost` and you already have a `localhost` instance locally:

```json
"ConnectionStrings":{
    "AppDb": "Data Source=sql_server,1433;Initial Catalog=cafeemployeeapp_db;User Id=SA;Password=sQLs3rverRpAssw0rD!!;Encrypt=True;Trust Server Certificate=True"
}
```
