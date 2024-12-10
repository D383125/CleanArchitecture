using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Domain.Entities;

namespace Domain.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    partial class ApplicationDbContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity<Product>(b =>
            {
                b.Property<int>("Id")
                    .ValueGeneratedOnAdd()
                    .HasColumnType("integer");

                NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<int>("Id"));

                b.Property<string>("Name")
                    .IsRequired()
                    .HasColumnType("text");

                b.Property<decimal>("Price")
                    .HasColumnType("numeric");

                b.HasKey("Id");

                b.ToTable("Products");
            });

            modelBuilder.Entity<Chat>(entity =>
            {
                entity.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("integer");

                NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(entity.Property<int>("Id"));

                entity.Property(e => e.CreatorId)
                    .HasColumnType("integer");

                entity.Property(e => e.Message)
                    .HasColumnType("jsonb");

                entity.Property(e => e.CreatedOn)
                    .HasColumnType("timestamptz")
                    .HasDefaultValueSql("CURRENT_TIMESTAMP");

                entity.Property(e => e.LastModifiedOn)
                    .HasColumnType("timestamptz");

                entity.ToTable("ChatHistory");
            });





            // Convert all table names to lowercase
            //foreach (var entity in modelBuilder.Model.GetEntityTypes())
            //{
            //    entity.SetTableName(entity.GetTableName()?.ToLowerInvariant());

            //    // Convert all column names to lowercase
            //    foreach (var property in entity.GetProperties())
            //    {
            //        property.SetColumnName(property.GetColumnName()?.ToLowerInvariant());
            //    }

            //    // Convert keys and foreign keys to lowercase
            //    foreach (var key in entity.GetKeys())
            //    {
            //        key.SetName(key.GetName()?.ToLowerInvariant());
            //    }

            //    foreach (var fk in entity.GetForeignKeys())
            //    {
            //        fk.SetConstraintName(fk.GetConstraintName()?.ToLowerInvariant());
            //    }

            //    foreach (var index in entity.GetIndexes())
            //    {
            //        index.SetDatabaseName(index.GetDatabaseName()?.ToLowerInvariant());
            //    }
            //}

#pragma warning restore 612, 618
        }
    }
}
