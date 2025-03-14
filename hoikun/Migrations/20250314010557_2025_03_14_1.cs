using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace hoikun.Migrations
{
    /// <inheritdoc />
    public partial class _2025_03_14_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ShiftType",
                table: "Shifts");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "Shifts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "ShiftTypeId",
                table: "Shifts",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartTime",
                table: "Shifts",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "ShiftTypes",
                columns: table => new
                {
                    ShiftTypeId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ShiftTypes", x => x.ShiftTypeId);
                });

            migrationBuilder.InsertData(
                table: "ShiftTypes",
                columns: new[] { "ShiftTypeId", "CreatedAt", "EndTime", "Name", "StartTime", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 3, 14, 10, 5, 57, 364, DateTimeKind.Local).AddTicks(3628), new TimeSpan(0, 15, 0, 0, 0), "早番", new TimeSpan(0, 7, 0, 0, 0), new DateTime(2025, 3, 14, 10, 5, 57, 364, DateTimeKind.Local).AddTicks(3641) },
                    { 2, new DateTime(2025, 3, 14, 10, 5, 57, 364, DateTimeKind.Local).AddTicks(3648), new TimeSpan(0, 23, 0, 0, 0), "遅番", new TimeSpan(0, 15, 0, 0, 0), new DateTime(2025, 3, 14, 10, 5, 57, 364, DateTimeKind.Local).AddTicks(3648) },
                    { 3, new DateTime(2025, 3, 14, 10, 5, 57, 364, DateTimeKind.Local).AddTicks(3649), new TimeSpan(0, 0, 0, 0, 0), "代休", new TimeSpan(0, 0, 0, 0, 0), new DateTime(2025, 3, 14, 10, 5, 57, 364, DateTimeKind.Local).AddTicks(3650) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Shifts_ShiftTypeId",
                table: "Shifts",
                column: "ShiftTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_ShiftTypes_ShiftTypeId",
                table: "Shifts",
                column: "ShiftTypeId",
                principalTable: "ShiftTypes",
                principalColumn: "ShiftTypeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_ShiftTypes_ShiftTypeId",
                table: "Shifts");

            migrationBuilder.DropTable(
                name: "ShiftTypes");

            migrationBuilder.DropIndex(
                name: "IX_Shifts_ShiftTypeId",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "ShiftTypeId",
                table: "Shifts");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Shifts");

            migrationBuilder.AddColumn<string>(
                name: "ShiftType",
                table: "Shifts",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
