using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MES.Domain.Entities;

namespace MES.Infrastructure.Persistence.Configurations;

public class StationConfiguration : IEntityTypeConfiguration<Station>
{
    public void Configure(EntityTypeBuilder<Station> builder)
    {
        builder.ToTable("Stations");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Code)
            .IsRequired()
            .HasMaxLength(20);

        builder.HasIndex(x => x.Code).IsUnique();

        // ✅ inverse navigation aniq ko‘rsatildi
        builder.HasOne(x => x.ProductionLine)
            .WithMany(x => x.Stations)
            .HasForeignKey(x => x.ProductionLineId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
