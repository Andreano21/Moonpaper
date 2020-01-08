using System;
using Microsoft.EntityFrameworkCore;
using Moonparser.Core;


namespace Moonparser
{
    class Program
    {
        static void Main(string[] args)
        {
            ParserManager.Run();

            Console.ReadKey();
        }
    }

    class AppContext : DbContext
    { 
        public DbSet<Article> Articles { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=DESKTOP-UP2HC2B\\SQLEXPRESS; Database=MoonpaperDb; Trusted_Connection=True");
        }
    }
}
