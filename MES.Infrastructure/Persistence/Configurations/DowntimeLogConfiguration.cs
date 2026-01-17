using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MES.Domain.Entities;
using MES.Domain.Enums;

namespace MES.Infrastructure.Persistence.Configurations;

public class DowntimeLogConfiguration : IEntityTypeConfiguration<DowntimeLog>
{
    public void Configure(EntityTypeBuilder<DowntimeLog> builder)
    {
        builder.ToTable("DowntimeLogs");

        builder.HasKey(x => x.Id);

        builder.Property(x => x.Reason)
            .HasConversion<int>()
            .IsRequired();

        builder.Property(x => x.Comment)
            .HasMaxLength(500);

        builder.HasOne<ProductionLine>()
            .WithMany()
            .HasForeignKey(x => x.ProductionLineId);

        builder.HasOne<WorkOrder>()
            .WithMany()
            .HasForeignKey(x => x.WorkOrderId);
    }
}
