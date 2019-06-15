using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Crumbs.EFCore.Migrations.MySqlMigrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "Framework");

            migrationBuilder.CreateTable(
                name: "Event",
                schema: "Framework",
                columns: table => new
                {
                    EventId = table.Column<long>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    AggregateId = table.Column<Guid>(nullable: false),
                    AppliedByUserId = table.Column<Guid>(nullable: false),
                    SessionId = table.Column<Guid>(nullable: false),
                    AggregateVersion = table.Column<int>(nullable: false),
                    Timestamp = table.Column<DateTimeOffset>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    Data = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Event", x => x.EventId);
                });

            migrationBuilder.CreateTable(
                name: "EventHandlerState",
                schema: "Framework",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    ProcessedEventId = table.Column<long>(nullable: false),
                    LastUpdated = table.Column<DateTimeOffset>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Data = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventHandlerState", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Session",
                schema: "Framework",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    CompletedDate = table.Column<DateTimeOffset>(nullable: false),
                    ComittedByUserId = table.Column<Guid>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    Data = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Snapshot",
                schema: "Framework",
                columns: table => new
                {
                    AggregateId = table.Column<Guid>(nullable: false),
                    Version = table.Column<int>(nullable: false),
                    Type = table.Column<string>(nullable: false),
                    Content = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Snapshot", x => x.AggregateId);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Event_AggregateId",
                schema: "Framework",
                table: "Event",
                column: "AggregateId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Event",
                schema: "Framework");

            migrationBuilder.DropTable(
                name: "EventHandlerState",
                schema: "Framework");

            migrationBuilder.DropTable(
                name: "Session",
                schema: "Framework");

            migrationBuilder.DropTable(
                name: "Snapshot",
                schema: "Framework");
        }
    }
}
