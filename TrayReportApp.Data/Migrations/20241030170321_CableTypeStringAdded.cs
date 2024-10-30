using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrayReportApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class CableTypeStringAdded : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cables_CableTypes_Type",
                table: "Cables");

            migrationBuilder.DropIndex(
                name: "IX_Cables_Type",
                table: "Cables");

            migrationBuilder.AlterColumn<string>(
                name: "Type",
                table: "Cables",
                type: "text",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "CableTypeId",
                table: "Cables",
                type: "integer",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cables_CableTypeId",
                table: "Cables",
                column: "CableTypeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Cables_CableTypes_CableTypeId",
                table: "Cables",
                column: "CableTypeId",
                principalTable: "CableTypes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cables_CableTypes_CableTypeId",
                table: "Cables");

            migrationBuilder.DropIndex(
                name: "IX_Cables_CableTypeId",
                table: "Cables");

            migrationBuilder.DropColumn(
                name: "CableTypeId",
                table: "Cables");

            migrationBuilder.AlterColumn<int>(
                name: "Type",
                table: "Cables",
                type: "integer",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Cables_Type",
                table: "Cables",
                column: "Type");

            migrationBuilder.AddForeignKey(
                name: "FK_Cables_CableTypes_Type",
                table: "Cables",
                column: "Type",
                principalTable: "CableTypes",
                principalColumn: "Id");
        }
    }
}
