using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hoikun.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationToAppointment2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Routs_Users_UserId",
                table: "Routs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Routs",
                table: "Routs");

            migrationBuilder.RenameTable(
                name: "Routs",
                newName: "Rout");

            migrationBuilder.RenameIndex(
                name: "IX_Routs_UserId",
                table: "Rout",
                newName: "IX_Rout_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rout",
                table: "Rout",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Rout_Users_UserId",
                table: "Rout",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Rout_Users_UserId",
                table: "Rout");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rout",
                table: "Rout");

            migrationBuilder.RenameTable(
                name: "Rout",
                newName: "Routs");

            migrationBuilder.RenameIndex(
                name: "IX_Rout_UserId",
                table: "Routs",
                newName: "IX_Routs_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Routs",
                table: "Routs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Routs_Users_UserId",
                table: "Routs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
