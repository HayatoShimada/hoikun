using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hoikun.Migrations
{
    /// <inheritdoc />
    public partial class FixMessageUserDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyContacts_Users_UserId",
                table: "EmergencyContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageRecipients_Messages_MessageId",
                table: "MessageRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageRecipients_Users_UserId",
                table: "MessageRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Photo_PhotoId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Photo_Users_UserId",
                table: "Photo");

            migrationBuilder.DropForeignKey(
                name: "FK_Rout_Users_UserId",
                table: "Rout");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Rout",
                table: "Rout");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Photo",
                table: "Photo");

            migrationBuilder.DropColumn(
                name: "MenssageCategoryId",
                table: "Messages");

            migrationBuilder.RenameTable(
                name: "Rout",
                newName: "Routs");

            migrationBuilder.RenameTable(
                name: "Photo",
                newName: "Photos");

            migrationBuilder.RenameIndex(
                name: "IX_Rout_UserId",
                table: "Routs",
                newName: "IX_Routs_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Photo_UserId",
                table: "Photos",
                newName: "IX_Photos_UserId");

            migrationBuilder.AddColumn<int>(
                name: "MessageCategoryId",
                table: "Messages",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Routs",
                table: "Routs",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Photos",
                table: "Photos",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "MessageCategoryOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageCategoryId = table.Column<int>(type: "int", nullable: false),
                    OptionKey = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageCategoryOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageCategoryOptions_MessageCategories_MessageCategoryId",
                        column: x => x.MessageCategoryId,
                        principalTable: "MessageCategories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MessageOptions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MessageId = table.Column<int>(type: "int", nullable: false),
                    OptionKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionValue = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageOptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MessageOptions_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserAppointments_UserId",
                table: "UserAppointments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MessageCategoryId",
                table: "Messages",
                column: "MessageCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageCategoryOptions_MessageCategoryId",
                table: "MessageCategoryOptions",
                column: "MessageCategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_MessageOptions_MessageId",
                table: "MessageOptions",
                column: "MessageId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyContacts_Users_UserId",
                table: "EmergencyContacts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageRecipients_Messages_MessageId",
                table: "MessageRecipients",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_MessageRecipients_Users_UserId",
                table: "MessageRecipients",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_MessageCategories_MessageCategoryId",
                table: "Messages",
                column: "MessageCategoryId",
                principalTable: "MessageCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Photos_PhotoId",
                table: "Messages",
                column: "PhotoId",
                principalTable: "Photos",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Photos_Users_UserId",
                table: "Photos",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Routs_Users_UserId",
                table: "Routs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAppointments_Users_UserId",
                table: "UserAppointments",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyContacts_Users_UserId",
                table: "EmergencyContacts");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageRecipients_Messages_MessageId",
                table: "MessageRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_MessageRecipients_Users_UserId",
                table: "MessageRecipients");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_MessageCategories_MessageCategoryId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Photos_PhotoId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Photos_Users_UserId",
                table: "Photos");

            migrationBuilder.DropForeignKey(
                name: "FK_Routs_Users_UserId",
                table: "Routs");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAppointments_Users_UserId",
                table: "UserAppointments");

            migrationBuilder.DropTable(
                name: "MessageCategoryOptions");

            migrationBuilder.DropTable(
                name: "MessageOptions");

            migrationBuilder.DropIndex(
                name: "IX_UserAppointments_UserId",
                table: "UserAppointments");

            migrationBuilder.DropIndex(
                name: "IX_Messages_MessageCategoryId",
                table: "Messages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Routs",
                table: "Routs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Photos",
                table: "Photos");

            migrationBuilder.DropColumn(
                name: "MessageCategoryId",
                table: "Messages");

            migrationBuilder.RenameTable(
                name: "Routs",
                newName: "Rout");

            migrationBuilder.RenameTable(
                name: "Photos",
                newName: "Photo");

            migrationBuilder.RenameIndex(
                name: "IX_Routs_UserId",
                table: "Rout",
                newName: "IX_Rout_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Photos_UserId",
                table: "Photo",
                newName: "IX_Photo_UserId");

            migrationBuilder.AddColumn<string>(
                name: "MenssageCategoryId",
                table: "Messages",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Rout",
                table: "Rout",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Photo",
                table: "Photo",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyContacts_Users_UserId",
                table: "EmergencyContacts",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageRecipients_Messages_MessageId",
                table: "MessageRecipients",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MessageRecipients_Users_UserId",
                table: "MessageRecipients",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Photo_PhotoId",
                table: "Messages",
                column: "PhotoId",
                principalTable: "Photo",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Photo_Users_UserId",
                table: "Photo",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Rout_Users_UserId",
                table: "Rout",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "UserId");
        }
    }
}
