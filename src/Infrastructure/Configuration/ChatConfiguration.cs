using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configuration
{
    //TODO: Remove as Domain shopuld not have any external references like EFCore. Should be Entreprise Business items only
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
