using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hoikun.Migrations
{
    /// <inheritdoc />
    public partial class _20250407_1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Childrens_Classes_ClassId",
                table: "Childrens");

            migrationBuilder.AlterColumn<int>(
                name: "ClassId",
                table: "Childrens",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Childrens_Classes_ClassId",
                table: "Childrens",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Childrens_Classes_ClassId",
                table: "Childrens");

            migrationBuilder.AlterColumn<int>(
                name: "ClassId",
                table: "Childrens",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Childrens_Classes_ClassId",
                table: "Childrens",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
