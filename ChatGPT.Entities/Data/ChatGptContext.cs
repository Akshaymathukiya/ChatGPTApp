using System;
using System.Collections.Generic;
using ChatGPT.Entities.Models;
using Microsoft.EntityFrameworkCore;

namespace ChatGPT.Entities.Data;

public partial class ChatGptContext : DbContext
{
    public ChatGptContext()
    {
    }

    public ChatGptContext(DbContextOptions<ChatGptContext> options)
        : base(options)
    {
    }

    public virtual DbSet<User> Users { get; set; }

    public virtual DbSet<UserHistory> UserHistories { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
        => optionsBuilder.UseSqlServer("Data Source=PCT230\\SQL2017;DataBase=ChatGPT;User ID=sa;Password=tatva123;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__users__3213E83FE244DCDD");

            entity.ToTable("users");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.Firstname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("firstname");
            entity.Property(e => e.Lastname)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("lastname");
            entity.Property(e => e.Mobilenumber).HasColumnName("mobilenumber");
            entity.Property(e => e.Password)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("password");
        });

        modelBuilder.Entity<UserHistory>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__user_his__3213E83FE62A25DC");

            entity.ToTable("user_history");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.Answer)
                .HasColumnType("text")
                .HasColumnName("answer");
            entity.Property(e => e.CreatedAt)
                .HasColumnType("datetime")
                .HasColumnName("createdAt");
            entity.Property(e => e.Question)
                .HasColumnType("text")
                .HasColumnName("question");
            entity.Property(e => e.UserId).HasColumnName("user_id");

            entity.HasOne(d => d.User).WithMany(p => p.UserHistories)
                .HasForeignKey(d => d.UserId)
                .HasConstraintName("FK__user_hist__user___5CD6CB2B");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
