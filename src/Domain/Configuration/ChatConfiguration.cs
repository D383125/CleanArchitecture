﻿using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Configuration
{
    internal class ChatConfiguration : IEntityTypeConfiguration<Chat>
    {
        public void Configure(EntityTypeBuilder<Chat> builder)
        {
            builder.HasKey(x => x.Id);
            builder.Property(p => p.CreatorId)
                .IsRequired();

            builder.Property(p => p.CreatedOn)
                .IsRequired();

            builder.Property(p => p.LastModifiedOn)
                .IsRequired();

            builder.Property(x => x.Message)
                 .HasColumnType("jsonb")
                 .IsRequired();
        }
    }
}
