using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LearningPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAutomaticAttendanceSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_AspNetUsers_MarkedByUserId",
                table: "AttendanceRecords");

            migrationBuilder.AddColumn<DateTime>(
                name: "AttendanceFinalizedAt",
                table: "ClassSessions",
                type: "timestamp with time zone",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "MarkedByUserId",
                table: "AttendanceRecords",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.AddColumn<int>(
                name: "Source",
                table: "AttendanceRecords",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_AspNetUsers_MarkedByUserId",
                table: "AttendanceRecords",
                column: "MarkedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AttendanceRecords_AspNetUsers_MarkedByUserId",
                table: "AttendanceRecords");

            migrationBuilder.DropColumn(
                name: "AttendanceFinalizedAt",
                table: "ClassSessions");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "AttendanceRecords");

            migrationBuilder.AlterColumn<Guid>(
                name: "MarkedByUserId",
                table: "AttendanceRecords",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_AttendanceRecords_AspNetUsers_MarkedByUserId",
                table: "AttendanceRecords",
                column: "MarkedByUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
