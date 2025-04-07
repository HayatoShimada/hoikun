using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hoikun.Migrations
{
    /// <inheritdoc />
    public partial class _2025_04_07_02 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DelayMinutes",
                table: "PickupRecords",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PickupType",
                table: "PickupRecords",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DelayMinutes",
                table: "PickupRecords");

            migrationBuilder.DropColumn(
                name: "PickupType",
                table: "PickupRecords");
        }
    }
}
