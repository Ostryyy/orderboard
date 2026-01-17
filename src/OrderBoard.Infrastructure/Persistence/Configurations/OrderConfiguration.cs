using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderBoard.Core.Domain.Orders;

namespace OrderBoard.Infrastructure.Persistence.Configurations;

public sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");

        builder.HasKey(o => o.Id);

        builder.Property(o => o.CustomerName)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(o => o.BoardId)
            .HasMaxLength(100)
            .IsRequired();

        builder.Property(o => o.CreatedAt)
            .IsRequired();

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .HasMaxLength(50)
            .IsRequired();

        builder.OwnsMany<OrderItem>("_items", items =>
        {
            items.ToTable("OrderItems");
            items.WithOwner().HasForeignKey("OrderId");

            items.Property<Guid>("Id");
            items.HasKey("Id");

            items.Property(i => i.Name)
                .HasColumnName("Name")
                .HasMaxLength(200)
                .IsRequired();

            items.Property(i => i.Quantity)
                .HasColumnName("Quantity")
                .IsRequired();
        });

        builder.Navigation("_items")
            .UsePropertyAccessMode(PropertyAccessMode.Field);
    }
}
