using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Moonparser.Core;


namespace Moonparser
{
    class Program
    {
        static async Task Main(string[] args)
        {

            await ParserManager.Run();

            ParserManager.Push();

            Console.ReadKey();
        }
    }

    class AppContext : DbContext
    { 
        public DbSet<Article> Articles { get; set; }

        public AppContext()
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-UP2HC2B\\SQLEXPRESS; Database=MoonpaperDb; Trusted_Connection=True");
        }
    }
}
