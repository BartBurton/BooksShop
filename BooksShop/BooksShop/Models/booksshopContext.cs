using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace BooksShop.Models
{
    public partial class booksshopContext : DbContext
    {
        public booksshopContext(DbContextOptions<booksshopContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Autor> Autors { get; set; }
        public virtual DbSet<Book> Books { get; set; }
        public virtual DbSet<BooksAutor> BooksAutors { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasAnnotation("Relational:Collation", "Cyrillic_General_CI_AS");

            modelBuilder.Entity<Autor>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Dob)
                    .HasColumnType("date")
                    .HasColumnName("dob");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasMaxLength(256)
                    .HasColumnName("name");
            });

            modelBuilder.Entity<Book>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Description)
                    .HasMaxLength(2048)
                    .HasColumnName("description")
                    .HasDefaultValueSql("('No description!')");

                entity.Property(e => e.Edition)
                    .HasMaxLength(256)
                    .HasColumnName("edition");

                entity.Property(e => e.PublishedAt)
                    .HasMaxLength(256)
                    .HasColumnName("published_at");

                entity.Property(e => e.Title)
                    .IsRequired()
                    .HasMaxLength(128)
                    .HasColumnName("title");
            });

            modelBuilder.Entity<BooksAutor>(entity =>
            {
                entity.HasKey(e => new { e.IdBook, e.IdAutor })
                    .HasName("PK__Books_Au__1F1B2A9AC00E0C5E");

                entity.ToTable("Books_Autors");

                entity.Property(e => e.IdBook).HasColumnName("id_book");

                entity.Property(e => e.IdAutor).HasColumnName("id_autor");

                entity.HasOne(d => d.IdAutorNavigation)
                    .WithMany(p => p.BooksAutors)
                    .HasForeignKey(d => d.IdAutor)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK__Books_Aut__id_au__60A75C0F");

                entity.HasOne(d => d.IdBookNavigation)
                    .WithMany(p => p.BooksAutors)
                    .HasForeignKey(d => d.IdBook)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("fk_books_to_autors");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
