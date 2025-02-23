using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hoikun.Migrations
{
    /// <inheritdoc />
    public partial class AddLocationToAppointment25 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Value",
                table: "FormSubmissionFields");

            migrationBuilder.AddColumn<DateTime>(
                name: "DateValue",
                table: "FormSubmissionFields",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IntValue",
                table: "FormSubmissionFields",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "StringValue",
                table: "FormSubmissionFields",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DateValue",
                table: "FormSubmissionFields");

            migrationBuilder.DropColumn(
                name: "IntValue",
                table: "FormSubmissionFields");

            migrationBuilder.DropColumn(
                name: "StringValue",
                table: "FormSubmissionFields");

            migrationBuilder.AddColumn<string>(
                name: "Value",
                table: "FormSubmissionFields",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
