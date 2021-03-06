﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Novel.Entity.Models
{
    public partial class BookContext : DbContext
    {
        public BookContext()
        {
        }

        public BookContext(DbContextOptions<BookContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Book> Book { get; set; }
        public virtual DbSet<BookCategory> BookCategory { get; set; }
        public virtual DbSet<BookGroupCategroyRelation> BookGroupCategroyRelation { get; set; }
        public virtual DbSet<BookIndex> BookIndex { get; set; }
        public virtual DbSet<BookItem> BookItem { get; set; }
        public virtual DbSet<BookLinks> BookLinks { get; set; }
        public virtual DbSet<BookRecommend> BookRecommend { get; set; }
        public virtual DbSet<BookReptileTask> BookReptileTask { get; set; }
        public virtual DbSet<BookShelf> BookShelf { get; set; }
        public virtual DbSet<BookThumbsup> BookThumbsup { get; set; }
        public virtual DbSet<Linksubmit> Linksubmit { get; set; }
        public virtual DbSet<TaskToDo> TaskToDo { get; set; }
        public virtual DbSet<UserInfo> UserInfo { get; set; }
        public virtual DbSet<UserRead> UserRead { get; set; }
        public virtual DbSet<UserReadBookHistory> UserReadBookHistory { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. See http://go.microsoft.com/fwlink/?LinkId=723263 for guidance on storing connection strings.
                optionsBuilder.UseSqlServer("Data Source=118.25.74.102;Initial Catalog=Book;Persist Security Info=True;User ID=zx;Password=1qaz@WSX;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("ProductVersion", "2.2.4-servicing-10062");

            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(e => e.BookAuthor)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.BookImage).HasMaxLength(200);

                entity.Property(e => e.BookName)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.BookReleaseTime).HasColumnType("datetime");

                entity.Property(e => e.BookSummary).HasMaxLength(2000);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<BookCategory>(entity =>
            {
                entity.HasKey(e => e.CategoryId);

                entity.Property(e => e.CategoryName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<BookGroupCategroyRelation>(entity =>
            {
                entity.ToTable("Book_GroupCategroy_Relation");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<BookIndex>(entity =>
            {
                entity.HasKey(e => e.Id)
                    .ForSqlServerIsClustered(false);

                entity.Property(e => e.BookName)
                    .IsRequired()
                    .HasMaxLength(50);

                entity.Property(e => e.DataYm).HasColumnName("DataYM");

                entity.Property(e => e.Date).HasColumnType("datetime");
            });

            modelBuilder.Entity<BookItem>(entity =>
            {
                entity.HasKey(e => e.ItemId);

                entity.HasIndex(e => new { e.ItemId, e.ItemName, e.BookId })
                    .HasName("IX_BookItem");

                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.ItemName)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Pri).HasColumnName("PRI");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<BookLinks>(entity =>
            {
                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(300);
            });

            modelBuilder.Entity<BookRecommend>(entity =>
            {
                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.DataYm).HasColumnName("DataYM");
            });

            modelBuilder.Entity<BookReptileTask>(entity =>
            {
                entity.Property(e => e.BookName).HasMaxLength(50);

                entity.Property(e => e.Created)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.CurrentRecod)
                    .IsRequired()
                    .HasMaxLength(100);

                entity.Property(e => e.Remark).HasColumnType("text");

                entity.Property(e => e.Updated).HasColumnType("datetime");

                entity.Property(e => e.Url)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<BookShelf>(entity =>
            {
                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });

            modelBuilder.Entity<BookThumbsup>(entity =>
            {
                entity.Property(e => e.Date).HasColumnType("datetime");

                entity.Property(e => e.Ip)
                    .IsRequired()
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Linksubmit>(entity =>
            {
                entity.Property(e => e.Result).HasMaxLength(500);

                entity.Property(e => e.UpdatedTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.WebUrl)
                    .IsRequired()
                    .HasMaxLength(500);
            });

            modelBuilder.Entity<TaskToDo>(entity =>
            {
                entity.Property(e => e.EndTime).HasColumnType("datetime");

                entity.Property(e => e.Remark).HasMaxLength(50);
            });

            modelBuilder.Entity<UserInfo>(entity =>
            {
                entity.HasKey(e => e.UserId);

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.Uesrpwd)
                    .IsRequired()
                    .HasMaxLength(30);

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");

                entity.Property(e => e.UserEmail).HasMaxLength(20);

                entity.Property(e => e.UserMoblie).HasMaxLength(11);

                entity.Property(e => e.UserName)
                    .IsRequired()
                    .HasMaxLength(30);
            });

            modelBuilder.Entity<UserRead>(entity =>
            {
                entity.Property(e => e.Id).ValueGeneratedNever();

                entity.Property(e => e.CreateTime).HasColumnType("datetime");

                entity.Property(e => e.UserId).ValueGeneratedOnAdd();
            });

            modelBuilder.Entity<UserReadBookHistory>(entity =>
            {
                entity.Property(e => e.CreateTime)
                    .HasColumnType("datetime")
                    .HasDefaultValueSql("(getdate())");

                entity.Property(e => e.UpdateTime).HasColumnType("datetime");
            });
        }
    }
}
