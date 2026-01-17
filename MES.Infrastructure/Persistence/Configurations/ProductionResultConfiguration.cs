using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MES.Domain.Entities;

namespace MES.Infrastructure.Persistence.Configurations;

public class ProductionResultConfiguration : IEntityTypeConfiguration<ProductionResult>
{
    public void Configure(EntityTypeBuilder<ProductionResult> builder)
    {
        builder.ToTable("ProductionResults");

        builder.HasKey(x => x.Id);

        builder.HasOne<ProductionLine>()
            .WithMany()
            .HasForeignKey(x => x.ProductionLineId);

        builder.HasOne<WorkOrder>()
            .WithMany()
            .HasForeignKey(x => x.WorkOrderId);
    }
}
