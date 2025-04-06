using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hoikun.Migrations
{
    /// <inheritdoc />
    public partial class AddPickup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PickupType",
                table: "Childrens",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "PickupRecords",
                columns: table => new
                {
                    PickupRecordId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ChildrenId = table.Column<int>(type: "int", nullable: false),
                    PickupTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickupRecords", x => x.PickupRecordId);
                    table.ForeignKey(
                        name: "FK_PickupRecords_Childrens_ChildrenId",
                        column: x => x.ChildrenId,
                        principalTable: "Childrens",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PickupTimeSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PickupType = table.Column<int>(type: "int", nullable: false),
                    PickupTime = table.Column<TimeSpan>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PickupTimeSettings", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PickupRecords_ChildrenId",
                table: "PickupRecords",
                column: "ChildrenId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PickupRecords");

            migrationBuilder.DropTable(
                name: "PickupTimeSettings");

            migrationBuilder.DropColumn(
                name: "PickupType",
                table: "Childrens");
        }
    }
}
