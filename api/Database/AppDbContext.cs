using System.Numerics;
using api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.ValueGeneration;

namespace api.Database;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions options) : base(options) { }
    public DbSet<Employee> Employees { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>()
            .Property(e => e.Id)
            .HasValueGenerator<EmployeeIdGenerator>();
        
        modelBuilder.Entity<Employee>()
            .Property(e => e.Gender)
            .HasConversion(
                v => Convert.ToBoolean(v),
                v => Convert.ToInt16(v)
            );

        modelBuilder.Entity<Company>()
            .Property(c => c.Id)
            .HasConversion(
                v => Guid.Parse(v), //convert to guid when saving to db
                v => v.ToString() //convert from guid (to string) when reading from db
            )
            .HasDefaultValueSql("NEWID()");

        //configure Auditable for each entity inheriting from it
        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            var clrType = entityType.ClrType;
            if (typeof(Entity).IsAssignableFrom(clrType) && clrType != typeof(Entity))
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
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSeeding((dbContext, _) =>
        {
            var random = new Random();
            var companies = dbContext.Set<Company>();
            if (companies.Count() == 0)
            {
                var newCompanies = Enumerable.Range(0, 10).Select(i =>
                {
                    return new Company()
                    {
                        Name = $"Company {i}",
                        Description = $"Rank #{i} in best companies",
                        Location = random.GetItems(["Singapore", "USA", "Philippines", "Japan"], 1)[0]
                    };
                });

                companies.AddRange(newCompanies);
                dbContext.SaveChanges();
            }

            var employees = dbContext.Set<Employee>();
            if (employees.Count() == 0)
            {
                var newEmployees = Enumerable.Range(0, 20).Select(i =>
                {

                    DateTime? startDate = null;
                    string? companyId = null;
                    if (i % 2 == 1)
                    {
                        startDate = DateTime.Now.AddDays(random.Next(31) * -1);
                        companyId = companies.ToList()[random.Next(companies.Count())].Id;
                    }
                    return new Employee()
                    {
                        Name = $"Emp{i}",
                        Email = $"employee_{i}@test.com",
                        Phone = "81444444",
                        Gender = Convert.ToInt16(i % 2 == 0),
                        CompanyId = companyId,
                        StartDate = startDate

                    };
                });
                employees.AddRange(newEmployees);
                dbContext.SaveChanges();
            }
        });
    }

    private class EmployeeIdGenerator : ValueGenerator<string>
    {
        public override bool GeneratesTemporaryValues => false;

        public override string Next(EntityEntry entry)
        {
            var guid = new BigInteger(Guid.NewGuid().ToByteArray().Concat(new byte[] { 0 }).ToArray());
            return $"UI{base62(guid)}";
        }
        private readonly string _validChars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private string base62(BigInteger id)
        {
            string result = string.Empty;
            var quotient = id;
            while (quotient != 0)
            {
                quotient = BigInteger.DivRem(quotient, 62, out var remainder);
                result = $"{_validChars[(int)remainder]}{result}";
                quotient /= 62;
            }
            return result;
        }
    }
}