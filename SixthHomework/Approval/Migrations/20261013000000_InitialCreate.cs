using System;
using Approval.Data;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Infrastructure;

#nullable disable

namespace Approval.Migrations;

[DbContext(typeof(ApprovalDbContext))]
[Migration("20261013000000_InitialCreate")]
public partial class InitialCreate : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.CreateTable(
            name: "approval_requests",
            columns: table => new
            {
                id = table.Column<Guid>(type: "uuid", nullable: false),
                user_id = table.Column<Guid>(type: "uuid", nullable: false),
                title = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false),
                description = table.Column<string>(type: "text", nullable: false),
                status = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: false),
                created_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                decided_at = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                decision_comment = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true)
            },
            constraints: table => table.PrimaryKey("PK_approval_requests", request => request.id));

        migrationBuilder.CreateIndex(
            name: "ix_approval_requests_user_id",
            table: "approval_requests",
            column: "user_id");
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(name: "approval_requests");
    }
}
