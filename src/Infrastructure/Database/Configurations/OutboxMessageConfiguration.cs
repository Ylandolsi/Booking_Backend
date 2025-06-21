using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SharedKernel;

namespace Infrastructure.Database.Configurations
{
    internal sealed class OutboxMessageConfiguration : IEntityTypeConfiguration<OutboxMessage>
    {
        public void Configure(EntityTypeBuilder<OutboxMessage> builder)
        {
            builder.HasKey(builder => builder.Id);
            builder.HasIndex(builder => builder.ProcessedOnUtc);
            builder.Property(builder => builder.Id)
                .ValueGeneratedOnAdd()
                .IsRequired();
        }
    }
}