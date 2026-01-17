using MES.Domain.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

public class ProductionLogConfiguration : IEntityTypeConfiguration<ProductionLog>
{
    public void Configure(EntityTypeBuilder<ProductionLog> builder)
    {
        builder.ToTable("ProductionLogs");

        builder.HasKey(x => x.Id);

        builder.HasOne<ProductionLine>()
            .WithMany()
            .HasForeignKey(x => x.ProductionLineId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<WorkOrder>()
            .WithMany()
            .HasForeignKey(x => x.WorkOrderId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Station>()
            .WithMany()
            .HasForeignKey(x => x.StationId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne<Operator>()
            .WithMany()
            .HasForeignKey(x => x.OperatorId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
