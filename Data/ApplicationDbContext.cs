using System;
using System.Collections.Generic;
using System.Text;
using Blog.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Blog.Models;

namespace Blog.Data
{
    public class ApplicationDbContext : IdentityDbContext<IdentityUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public virtual DbSet<Article> Article { get; set; }
        public virtual DbSet<ArticleTag> ArticleTag { get; set; }
        public virtual DbSet<Tag> Tag { get; set; }
        public virtual DbSet<Worker> Worker { get; set; }
        public virtual DbSet<Photo> Photo { get; set; }
        //public DbSet<Blog.Models.WorkerVM> WorkerVM { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Article>(entity =>
            {
                entity.Property(e => e.Body)
                    .IsRequired();

                entity.Property(e => e.DateCreated).HasColumnType("datetime");

                entity.Property(e => e.DateModifed).HasColumnType("datetime");

                entity.Property(e => e.DatePublished).HasColumnType("datetime");

                entity.Property(e => e.Meta)
                    .IsRequired()
                    .HasMaxLength(250);

                entity.Property(e => e.Serp)
                    .IsRequired()
                    .HasColumnName("SERP")
                    .HasMaxLength(500);

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(150);

                entity.HasOne(d => d.Worker)
                    .WithMany(p => p.Article)
                    .HasForeignKey(d => d.WorkerId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_Article_Worker");
            });

            modelBuilder.Entity<ArticleTag>(entity =>
            {
                entity.HasKey(e => new { e.TagId, e.ArticleId });

                entity.HasOne(d => d.Article)
                    .WithMany(p => p.ArticleTag)
                    .HasForeignKey(d => d.ArticleId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ArticleTag_Article");

                entity.HasOne(d => d.Tag)
                    .WithMany(p => p.ArticleTag)
                    .HasForeignKey(d => d.TagId)
                    .OnDelete(DeleteBehavior.ClientSetNull)
                    .HasConstraintName("FK_ArticleTag_Tag");
            });

            modelBuilder.Entity<Tag>(entity =>
            {
                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Worker>(entity =>
            {
                entity.Property(e => e.FirstName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.Joined).HasColumnType("datetime");

                entity.Property(e => e.LastName)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Photo>(entity =>
            {
                entity.HasOne(mt => mt.Article)
                .WithMany(ft => ft.Photo)
                .HasForeignKey(fk => fk.ArticleId)
                .OnDelete(DeleteBehavior.Cascade);
            });
        }
        //public DbSet<Blog.Models.WorkerVM> WorkerVM { get; set; }

        public DbSet<Blog.Models.WorkerVM> WorkerVM { get; set; }

        
    }
}
