using System.Collections.Generic;
using System.IO;
using Assignment4.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Assignment4
{
    public class KanbanContextBuilder : IDesignTimeDbContextFactory<KanbanContext>
    {
        public KanbanContext CreateDbContext(string[] args)
        {
            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddUserSecrets<Program>()
                .AddJsonFile("appsettings.json")
                .Build();

            // var connectionString = configuration.GetConnectionString("Kanban:ConnectionString"); 
            var connectionString = "Server=localhost,1433;Database=Kanban;User Id=sa;Password=yourStrong#Password;MultipleActiveResultSets=True;";


            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>()
                .UseSqlServer(connectionString);

            return new KanbanContext(optionsBuilder.Options);
        }

        public static void Seed(KanbanContext context)
        {
            context.Database.ExecuteSqlRaw("DELETE dbo.Tasks");
            context.Database.ExecuteSqlRaw("DELETE dbo.Users");
            context.Database.ExecuteSqlRaw("DELETE dbo.Tags");
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.Tasks', RESEED, 0)");
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.Users', RESEED, 0)");
            context.Database.ExecuteSqlRaw("DBCC CHECKIDENT ('dbo.Tags', RESEED, 0)");

            var merchant = new Tag { Name = "Merchant" };
            var admin = new Tag { Name = "Admin" };
            var developer = new Tag { Name = "Developer" };
            var plugin = new Tag { Name = "Plugin" };

            var user = new User { Name = "Kasper", Email = "kasper@medusa-commerce.com" };

            var rma = new Task { Title = "RMA Shipping Options", Description = "As a merchant I would like to be able to select from a selection of shipping options when handling RMA", AssignedTo = user, State = Core.State.Active, Tags = new List<Tag> {merchant, admin}};
            var shopify = new Task { Title = "Shopify Source Plugin", Description = "As a merchant/developer I would like to be able to import products/customers/orders etc. from Shopify", AssignedTo = user, State = Core.State.New, Tags = new List<Tag> {merchant, developer, plugin}};

            context.Tasks.AddRange(rma, shopify);

            context.SaveChanges();
        }
    }
}