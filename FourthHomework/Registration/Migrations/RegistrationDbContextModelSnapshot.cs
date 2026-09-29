using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Registration.Data;

#nullable disable

namespace Registration.Migrations;

[DbContext(typeof(RegistrationDbContext))]
partial class RegistrationDbContextModelSnapshot : ModelSnapshot
{
    protected override void BuildModel(ModelBuilder modelBuilder)
    {
        modelBuilder.HasAnnotation("ProductVersion", "10.0.0");

        modelBuilder.Entity("Registration.Models.User", entity =>
        {
            entity.Property<Guid>("Id").HasColumnType("uuid");
            entity.Property<DateTime>("CreatedDate").HasColumnType("timestamp with time zone");
            entity.Property<string>("Mail").IsRequired().HasMaxLength(256).HasColumnType("character varying(256)");
            entity.Property<string>("Name").IsRequired().HasMaxLength(100).HasColumnType("character varying(100)");
            entity.Property<string>("PasswordHash").IsRequired().HasColumnType("text");
            entity.Property<DateTime>("UpdatedDate").HasColumnType("timestamp with time zone");
            entity.HasKey("Id");
            entity.HasIndex("Mail").IsUnique();
            entity.ToTable("users");
        });
    }
}
