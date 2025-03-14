using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace hoikun.Migrations
{
    /// <inheritdoc />
    public partial class _2025_03_14_2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "ShiftTypes",
                keyColumn: "ShiftTypeId",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "ShiftTypes",
                keyColumn: "ShiftTypeId",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "ShiftTypes",
                keyColumn: "ShiftTypeId",
                keyValue: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "ShiftTypes",
                columns: new[] { "ShiftTypeId", "CreatedAt", "EndTime", "Name", "StartTime", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 3, 14, 10, 5, 57, 364, DateTimeKind.Local).AddTicks(3628), new TimeSpan(0, 15, 0, 0, 0), "早番", new TimeSpan(0, 7, 0, 0, 0), new DateTime(2025, 3, 14, 10, 5, 57, 364, DateTimeKind.Local).AddTicks(3641) },
                    { 2, new DateTime(2025, 3, 14, 10, 5, 57, 364, DateTimeKind.Local).AddTicks(3648), new TimeSpan(0, 23, 0, 0, 0), "遅番", new TimeSpan(0, 15, 0, 0, 0), new DateTime(2025, 3, 14, 10, 5, 57, 364, DateTimeKind.Local).AddTicks(3648) },
                    { 3, new DateTime(2025, 3, 14, 10, 5, 57, 364, DateTimeKind.Local).AddTicks(3649), new TimeSpan(0, 0, 0, 0, 0), "代休", new TimeSpan(0, 0, 0, 0, 0), new DateTime(2025, 3, 14, 10, 5, 57, 364, DateTimeKind.Local).AddTicks(3650) }
                });
        }
    }
}
