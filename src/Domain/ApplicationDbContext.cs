using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        //TODO: Convert to Postgres snake-case
        public DbSet<Product> Products { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

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
        }

    }
}
