using EF10_ExecuteUpdateDemo.Data;
using EF10_ExecuteUpdateDemo.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

bool nameChanged = true; // Toggle this to false to see behavior

var host = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        var connStr = context.Configuration.GetConnectionString("DefaultConnection");
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(connStr));
    })
    .Build();

using var scope = host.Services.CreateScope();
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();

// Ensure database and seed data
await db.Database.EnsureDeletedAsync();
await db.Database.EnsureCreatedAsync();

db.Blogs.AddRange(
    new Blog { Name = "EF Core Blog", Views = 100 },
    new Blog { Name = "EF10 Features", Views = 150 }
);
await db.SaveChangesAsync();

// ✅ EF10: ExecuteUpdateAsync with regular lambda and conditional logic

//The following code updates every row in the Blogs table.
// It sets the value of: Views column to 999 and Name column to 'Updated Blog Name'
//
//This is the code:
// If nameChanged == true, EF Core 10 will generate a SQL statement like this:
// UPDATE Blogs
// SET Views = 999,
// Name = 'Updated Blog Name';
//
//The following code updates all rows in the Blogs table.
//It changes the value of the Views column to 999 for every blog.
//
//This is the code:
// If nameChanged == false, it will only update views:
// UPDATE Blogs
// SET Views = 999;

await db.Blogs.ExecuteUpdateAsync(s =>
{
    s.SetProperty(b => b.Views, 999);
    if (nameChanged)
    {
        s.SetProperty(b => b.Name, "Updated Blog Name");
    }
});

Console.WriteLine("Blogs updated.");

