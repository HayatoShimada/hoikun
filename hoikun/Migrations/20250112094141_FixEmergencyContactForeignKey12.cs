using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hoikun.Migrations
{
    /// <inheritdoc />
    public partial class FixEmergencyContactForeignKey12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyContacts_Users_UserId1",
                table: "EmergencyContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageRecipients_Users_UserId",
                table: "MessageRecipients");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyContacts_UserId1",
                table: "EmergencyContacts");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "EmergencyContacts");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageRecipients_Users_UserId",
                table: "MessageRecipients",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MessageRecipients_Users_UserId",
                table: "MessageRecipients");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "EmergencyContacts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContacts_UserId1",
                table: "EmergencyContacts",
                column: "UserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyContacts_Users_UserId1",
                table: "EmergencyContacts",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageRecipients_Users_UserId",
                table: "MessageRecipients",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
