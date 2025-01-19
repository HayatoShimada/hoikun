using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hoikun.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationToAppointment8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DefaultValue",
                table: "FormFields");

            migrationBuilder.AddColumn<string>(
                name: "OptionsJson",
                table: "FormFields",
                type: "json",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OptionsJson",
                table: "FormFields");

            migrationBuilder.AddColumn<string>(
                name: "DefaultValue",
                table: "FormFields",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
