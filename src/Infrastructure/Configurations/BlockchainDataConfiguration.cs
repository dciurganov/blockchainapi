using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Configurations
{
    public class BlockchainDataConfiguration : IEntityTypeConfiguration<BlockchainData>
    {
        public void Configure(EntityTypeBuilder<BlockchainData> builder)
        {
            // Table name
            builder.ToTable("BlockchainData");

            // Primary Key
            builder.HasKey(e => e.Id);
            builder.Property(e => e.Id)
                .ValueGeneratedOnAdd();

            // BlockchainType
            builder.Property(e => e.BlockchainType)
                .IsRequired()
                .HasMaxLength(50);

            builder.HasIndex(e => e.BlockchainType)
                .HasDatabaseName("IX_BlockchainData_BlockchainType");

            // Name
            builder.Property(e => e.Name)
                .IsRequired()
                .HasMaxLength(100);

            // Height
            builder.Property(e => e.Height)
                .IsRequired();

            // Hash
            builder.Property(e => e.Hash)
                .IsRequired()
                .HasMaxLength(256);

            // Time
            builder.Property(e => e.Time)
                .IsRequired();

            // LatestUrl
            builder.Property(e => e.LatestUrl)
                .IsRequired()
                .HasMaxLength(500);

            // PreviousHash
            builder.Property(e => e.PreviousHash)
                .IsRequired()
                .HasMaxLength(256);

            // PreviousUrl
            builder.Property(e => e.PreviousUrl)
                .HasMaxLength(500);

            // Fee properties
            builder.Property(e => e.HighFeePerKb)
                .IsRequired();

            builder.Property(e => e.MediumFeePerKb)
                .IsRequired();

            builder.Property(e => e.LowFeePerKb)
                .IsRequired();

            // Fork properties
            builder.Property(e => e.LastForkHeight)
                .IsRequired();

            builder.Property(e => e.LastForkHash)
                .HasMaxLength(256);

            // CreatedAt - with default value and index
            builder.Property(e => e.CreatedAt)
                .IsRequired()
                .HasDefaultValueSql("datetime('now')");

            builder.HasIndex(e => e.CreatedAt)
                .HasDatabaseName("IX_BlockchainData_CreatedAt")
                .IsDescending(); // For efficient DESC queries

            // RawJsonData - store complete JSON response
            builder.Property(e => e.RawJsonData)
                .IsRequired()
                .HasColumnType("TEXT"); // SQLite text type for large JSON

            // Composite index for common queries
            builder.HasIndex(e => new { e.BlockchainType, e.CreatedAt })
                .HasDatabaseName("IX_BlockchainData_Type_Created")
                .IsDescending(); // For efficient filtering and sorting
        }
    }
}
