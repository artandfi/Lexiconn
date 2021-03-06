﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Lexiconn
{
    public partial class DBDictionaryContext : DbContext
    {
        public DBDictionaryContext()
        {
        }

        public DBDictionaryContext(DbContextOptions<DBDictionaryContext> options)
            : base(options)
        {
        }

        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<CategorizedWord> CategorizedWords { get; set; }
        public virtual DbSet<Language> Languages { get; set; }
        public virtual DbSet<Translation> Translations { get; set; }
        public virtual DbSet<Word> Words { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer("Server=ROOFLER\\SQLEXPRESS;Database=DBDictionary; Trusted_Connection=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50);
                entity.Property(e => e.UserName).HasColumnName("userName");
            });

            modelBuilder.Entity<CategorizedWord>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategoryId).HasColumnName("categoryId");

                entity.Property(e => e.WordId).HasColumnName("wordId");

                entity.Property(e => e.UserName).HasColumnName("userName");

                entity.HasOne(d => d.Category)
                    .WithMany(p => p.CategorizedWords)
                    .HasForeignKey(d => d.CategoryId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CategorizedWords_Categories");

                entity.HasOne(d => d.Word)
                    .WithMany(p => p.CategorizedWords)
                    .HasForeignKey(d => d.WordId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_CategorizedWords_Words");
            });

            modelBuilder.Entity<Language>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.UserName).HasColumnName("userName");

                entity.Property(e => e.Name)
                    .IsRequired()
                    .HasColumnName("name")
                    .HasMaxLength(50);
            });

            modelBuilder.Entity<Translation>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.CategorizedWordId).HasColumnName("categorizedWordId");

                entity.Property(e => e.ThisTranslation)
                    .IsRequired()
                    .HasColumnName("thisTranslation")
                    .HasMaxLength(50);

                entity.HasOne(d => d.CategorizedWord)
                    .WithMany(p => p.Translations)
                    .HasForeignKey(d => d.CategorizedWordId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Translations_CategorizedWords");
            });

            modelBuilder.Entity<Word>(entity =>
            {
                entity.Property(e => e.Id).HasColumnName("id");

                entity.Property(e => e.LanguageId).HasColumnName("languageId");

                entity.Property(e => e.ThisWord)
                    .IsRequired()
                    .HasColumnName("thisWord")
                    .HasMaxLength(50);

                entity.HasOne(d => d.Language)
                    .WithMany(p => p.Words)
                    .HasForeignKey(d => d.LanguageId)
                    .OnDelete(DeleteBehavior.Cascade)
                    .HasConstraintName("FK_Words_Languages");
            });

            OnModelCreatingPartial(modelBuilder);
        }

        partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
    }
}
