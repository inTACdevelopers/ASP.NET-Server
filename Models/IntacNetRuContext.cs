using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using Server.Backend;
using Server.Backend.Config;

namespace Server.Models;

public partial class IntacNetRuContext : DbContext
{
    public IntacNetRuContext()
    {
    }

    public IntacNetRuContext(DbContextOptions<IntacNetRuContext> options)
        : base(options)
    {

    }

    public virtual DbSet<Post> Posts { get; set; }

    public virtual DbSet<User> Users { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var _config = new ConfigManager();
      

        optionsBuilder.UseNpgsql(_config.GetConnetion());
    }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Post>(entity =>
        {
            entity.HasKey(e => new { e.Id, e.Description, e.Url }).HasName("posts_pkey");

            entity.ToTable("posts");

            entity.Property(e => e.Id)
                .ValueGeneratedOnAdd()
                .HasColumnName("id");
            entity.Property(e => e.Description)
                .HasMaxLength(255)
                .HasColumnName("description");
            entity.Property(e => e.Url)
                .HasMaxLength(255)
                .HasColumnName("url");
            entity.Property(e => e.CreationDate).HasColumnName("creation_date");
            entity.Property(e => e.Owner).HasColumnName("owner");
            entity.Property(e => e.Title)
                .HasMaxLength(255)
                .HasColumnName("title");
            entity.Property(e => e.Weight).HasColumnName("weight");

            entity.HasOne(d => d.OwnerNavigation).WithMany(p => p.Posts)
                .HasForeignKey(d => d.Owner)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("owner");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("users_pkey");

            entity.ToTable("users");

            entity.HasIndex(e => e.Login, "users_login_key").IsUnique();

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.About)
                .HasMaxLength(128)
                .HasColumnName("about");
            entity.Property(e => e.BirthDate).HasColumnName("birth_date");
            entity.Property(e => e.Company)
                .HasMaxLength(255)
                .HasColumnName("company");
            entity.Property(e => e.Login)
                .HasMaxLength(255)
                .HasColumnName("login");
            entity.Property(e => e.Name)
                .HasMaxLength(255)
                .HasColumnName("name");
            entity.Property(e => e.Password)
                .HasMaxLength(255)
                .HasColumnName("password");
            entity.Property(e => e.Surname)
                .HasMaxLength(255)
                .HasColumnName("surname");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
