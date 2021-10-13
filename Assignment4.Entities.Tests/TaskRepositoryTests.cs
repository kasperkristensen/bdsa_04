using Xunit;
using System.Collections.Generic;
using System;
using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace Assignment4.Entities.Tests
{
    public class TaskRepositoryTests
    {
        static string getConnectionString()
        {
            return "Server=localhost,1433;Database=Kanban;User Id=sa;Password=yourStrong#Password;MultipleActiveResultSets=True;";
        }

        [Fact]
        public void mail_is_correct() {
            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>().UseSqlServer(getConnectionString());
            using var context = new KanbanContext(optionsBuilder.Options);

            var email = from u in context.Users
                        where u.Name.Equals("Kasper")
                        select u.Email;
            var actual = email.First();
            var expected = "kasper@medusa-commerce.com";

            Assert.Equal(expected, actual);
        }

        [Fact]
        public void has_three_tags() {
            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>().UseSqlServer(getConnectionString());
            using var context = new KanbanContext(optionsBuilder.Options);

            var tags = context.Tasks.Where(t => t.Title == "Shopify Source Plugin").Select(t => t.Tags).First();
                       
            var actual = tags.Count();
            var expected = 3;

            Assert.Equal(expected, actual);
        }

         [Fact]
        public void is_new() {
            var optionsBuilder = new DbContextOptionsBuilder<KanbanContext>().UseSqlServer(getConnectionString());
            using var context = new KanbanContext(optionsBuilder.Options);

            var actual = context.Tasks.Where(t => t.Title == "Shopify Source Plugin").Select(t => t.State).First();
            var expected = Core.State.New;

            Assert.Equal(expected, actual);
        }
    }
}
