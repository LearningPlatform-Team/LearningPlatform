using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddEventManagementSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Events_SubjectOfferingId",
                table: "Events");

            migrationBuilder.RenameColumn(
                name: "StartsAt",
                table: "Events",
                newName: "StartsAtUtc");

            migrationBuilder.RenameIndex(
                name: "IX_Events_StartsAt",
                table: "Events",
                newName: "IX_Events_StartsAtUtc");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndsAtUtc",
                table: "Events",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Location",
                table: "Events",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAtUtc",
                table: "EventReminders",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Events_SubjectOfferingId_StartsAtUtc",
                table: "Events",
                columns: new[] { "SubjectOfferingId", "StartsAtUtc" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Events_SubjectOfferingId_StartsAtUtc",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EndsAtUtc",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "Location",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "SentAtUtc",
                table: "EventReminders");

            migrationBuilder.RenameColumn(
                name: "StartsAtUtc",
                table: "Events",
                newName: "StartsAt");

            migrationBuilder.RenameIndex(
                name: "IX_Events_StartsAtUtc",
                table: "Events",
                newName: "IX_Events_StartsAt");

            migrationBuilder.CreateIndex(
                name: "IX_Events_SubjectOfferingId",
                table: "Events",
                column: "SubjectOfferingId");
        }
    }
}
