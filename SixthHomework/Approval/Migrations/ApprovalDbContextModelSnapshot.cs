using Approval.Data;
using Approval.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;

#nullable disable

namespace Approval.Migrations;

[DbContext(typeof(ApprovalDbContext))]
partial class ApprovalDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.0");

        modelBuilder.Entity("Approval.Models.ApprovalRequest", entity =>
        {
            entity.Property<Guid>("Id").HasColumnName("id").HasColumnType("uuid");
            entity.Property<Guid>("UserId").HasColumnName("user_id").HasColumnType("uuid");
            entity.Property<string>("Title").IsRequired().HasMaxLength(200)
                .HasColumnName("title").HasColumnType("character varying(200)");
            entity.Property<string>("Description").IsRequired()
                .HasColumnName("description").HasColumnType("text");
            entity.Property<ApprovalStatus>("Status").HasConversion<string>().HasMaxLength(20)
                .HasColumnName("status").HasColumnType("character varying(20)");
            entity.Property<DateTime>("CreatedAt").HasColumnName("created_at")
                .HasColumnType("timestamp with time zone");
            entity.Property<DateTime?>("DecidedAt").HasColumnName("decided_at")
                .HasColumnType("timestamp with time zone");
            entity.Property<string>("DecisionComment").HasMaxLength(1000)
                .HasColumnName("decision_comment").HasColumnType("character varying(1000)");
            entity.HasKey("Id");
            entity.HasIndex("UserId").HasDatabaseName("ix_approval_requests_user_id");
            entity.ToTable("approval_requests");
        });
    }
}
