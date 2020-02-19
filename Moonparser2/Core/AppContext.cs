using System;
using System.Data.Entity;
using System.Linq;
using System.Collections.Generic;

namespace Moonparser.Core
{
    public class AppContext : DbContext
    {

        public AppContext()
            : base("name=AppContext")
        {
        }

        public DbSet<Article> Articles { get; set; }
    }
}