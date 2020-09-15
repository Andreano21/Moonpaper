using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using MoonpaperLinux.Models;
using Pomelo.EntityFrameworkCore.MySql;

namespace MoonpaperLinux.Data
{
    public class ApplicationDbContext : IdentityDbContext<User>
    {
        public DbSet<Article> Articles { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Source> Sources { get; set; }
        public DbSet<ArticleTag> ArticleTag { get; set; }
        public DbSet<UserTag> UserTags { get; set; }
        public DbSet<UserSource> UserSource { get; set; }
        public DbSet<Star> Stars { get; set; }

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
            Database.EnsureCreated();
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //optionsBuilder.UseMySql("server=localhost;UserId=root;Password=1234;database=MoonpaperDb2;");
            optionsBuilder.UseMySql("server=172.17.0.2;UserId=root;Password=25097346098758653486438978943856;database=MoonpaperDb2;");
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ArticleTag>()
                .HasKey(t => new { t.ArticleId, t.TagId });

            modelBuilder.Entity<ArticleTag>()
                .HasOne(sc => sc.Article)
                .WithMany(s => s.ArticleTags)
                .HasForeignKey(sc => sc.ArticleId);

            modelBuilder.Entity<ArticleTag>()
                .HasOne(sc => sc.Tag)
                .WithMany(c => c.ArticleTags)
                .HasForeignKey(sc => sc.TagId);


            modelBuilder.Entity<UserTag>()
                .HasKey(t => new { t.UserId, t.TagId });

            modelBuilder.Entity<UserTag>()
                .HasOne(sc => sc.User)
                .WithMany(s => s.UserTags)
                .HasForeignKey(sc => sc.UserId);

            modelBuilder.Entity<UserTag>()
                .HasOne(sc => sc.Tag)
                .WithMany(c => c.UserTags)
                .HasForeignKey(sc => sc.TagId);

            modelBuilder.Entity<Article>()
                .HasOne(s => s.Source)
                .WithMany(a => a.Articles);

            modelBuilder.Entity<Source>().HasIndex(s => s.Name).IsUnique();
            //modelBuilder.Entity<Source>().HasIndex(s => s.Url).IsUnique();

            //modelBuilder.Entity<UserSource>()
            //    .HasKey(us => us.Id);
        }
    }
}
