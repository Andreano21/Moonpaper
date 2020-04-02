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
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Source> Sources { get; set; }


        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            modelBuilder.Entity<Article>()
                .HasMany<Tag>(s => s.Tags)
                .WithMany(c => c.Articles)
                .Map(cs =>
                {
                    cs.MapLeftKey("ArticleId");
                    cs.MapRightKey("TagId");
                    cs.ToTable("ArticleTag");
                });

            modelBuilder.Entity<Article>()
                .HasRequired(s => s.Source)
                .WithMany(a => a.Articles)
                .Map(sa => sa.MapKey("SourceId"));

            //modelBuilder.Entity<Source>().HasIndex(s => s.Name).IsUnique();

        }
    }
}