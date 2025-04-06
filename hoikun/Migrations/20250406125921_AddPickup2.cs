using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hoikun.Migrations
{
    /// <inheritdoc />
    public partial class AddPickup2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<TimeOnly>(
                name: "PickupExpectedTime",
                table: "Childrens",
                type: "time",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PickupExpectedTime",
                table: "Childrens");
        }
    }
}
