using Approval.Models;
using Microsoft.EntityFrameworkCore;

namespace Approval.Data;

public sealed class ApprovalDbContext(DbContextOptions<ApprovalDbContext> options) : DbContext(options)
{
    public DbSet<ApprovalRequest> Requests => Set<ApprovalRequest>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<ApprovalRequest>(entity =>
        {
            entity.ToTable("approval_requests");
            entity.HasKey(request => request.Id);
            entity.Property(request => request.Id).HasColumnName("id");
            entity.Property(request => request.UserId).HasColumnName("user_id");
            entity.Property(request => request.Title).HasColumnName("title").HasMaxLength(200).IsRequired();
            entity.Property(request => request.Description).HasColumnName("description").IsRequired();
            entity.Property(request => request.Status).HasColumnName("status").HasConversion<string>().HasMaxLength(20);
            entity.Property(request => request.CreatedAt).HasColumnName("created_at");
            entity.Property(request => request.DecidedAt).HasColumnName("decided_at");
            entity.Property(request => request.DecisionComment).HasColumnName("decision_comment").HasMaxLength(1000);
            entity.HasIndex(request => request.UserId).HasDatabaseName("ix_approval_requests_user_id");
        });
    }
}
