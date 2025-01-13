using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hoikun.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationToAppointment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeacherId",
                table: "Classes");

            migrationBuilder.AddColumn<int>(
                name: "UserId1",
                table: "EmergencyContacts",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ClassTeachers",
                columns: table => new
                {
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassTeachers", x => new { x.ClassId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ClassTeachers_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassTeachers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_EmergencyContacts_UserId1",
                table: "EmergencyContacts",
                column: "UserId1");

            migrationBuilder.CreateIndex(
                name: "IX_ClassTeachers_UserId",
                table: "ClassTeachers",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_EmergencyContacts_Users_UserId1",
                table: "EmergencyContacts",
                column: "UserId1",
                principalTable: "Users",
                principalColumn: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_EmergencyContacts_Users_UserId1",
                table: "EmergencyContacts");

            migrationBuilder.DropTable(
                name: "ClassTeachers");

            migrationBuilder.DropIndex(
                name: "IX_EmergencyContacts_UserId1",
                table: "EmergencyContacts");

            migrationBuilder.DropColumn(
                name: "UserId1",
                table: "EmergencyContacts");

            migrationBuilder.AddColumn<int>(
                name: "TeacherId",
                table: "Classes",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
