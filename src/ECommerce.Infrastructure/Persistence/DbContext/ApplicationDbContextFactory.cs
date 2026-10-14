using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace ECommerce.Infrastructure.Persistence.DbContext;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var currentDirectory = Directory.GetCurrentDirectory();
        var webApiPath = currentDirectory;

        if (!File.Exists(Path.Combine(webApiPath, "appsettings.json")))
        {
            var siblingPath = Path.Combine(currentDirectory, "../ECommerce.WebAPI");
            var childPath = Path.Combine(currentDirectory, "ECommerce.WebAPI");

            if (Directory.Exists(siblingPath))
            {
                webApiPath = Path.GetFullPath(siblingPath);
            }
            else if (Directory.Exists(childPath))
            {
                webApiPath = Path.GetFullPath(childPath);
            }
        }

        var configuration = new ConfigurationBuilder()
            .SetBasePath(webApiPath)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
            .AddJsonFile("appsettings.Development.json", optional: true, reloadOnChange: false)
            .AddEnvironmentVariables()
            .Build();

        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrWhiteSpace(connectionString))
        {
            throw new InvalidOperationException(
                "Could not find a connection string named 'DefaultConnection' in appsettings.json.");
        }

        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
        optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
        {
            sqlOptions.MigrationsAssembly(typeof(ApplicationDbContext).Assembly.FullName);
        });

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}