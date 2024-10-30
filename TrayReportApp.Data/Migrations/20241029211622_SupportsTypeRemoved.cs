using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrayReportApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class SupportsTypeRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Type",
                table: "Supports");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "Supports",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
