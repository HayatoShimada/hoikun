using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hoikun.Migrations
{
    /// <inheritdoc />
    public partial class _2025_03_14_0 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shift_Employees_EmployeeId",
                table: "Shift");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shift",
                table: "Shift");

            migrationBuilder.RenameTable(
                name: "Shift",
                newName: "Shifts");

            migrationBuilder.RenameIndex(
                name: "IX_Shift_EmployeeId",
                table: "Shifts",
                newName: "IX_Shifts_EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shifts",
                table: "Shifts",
                column: "ShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Employees_EmployeeId",
                table: "Shifts",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Employees_EmployeeId",
                table: "Shifts");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Shifts",
                table: "Shifts");

            migrationBuilder.RenameTable(
                name: "Shifts",
                newName: "Shift");

            migrationBuilder.RenameIndex(
                name: "IX_Shifts_EmployeeId",
                table: "Shift",
                newName: "IX_Shift_EmployeeId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Shift",
                table: "Shift",
                column: "ShiftId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shift_Employees_EmployeeId",
                table: "Shift",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
