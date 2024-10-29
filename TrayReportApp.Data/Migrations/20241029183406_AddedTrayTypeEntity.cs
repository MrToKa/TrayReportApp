using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace TrayReportApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedTrayTypeEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trays_Supports_SupportId",
                table: "Trays");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "Trays");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "Trays");

            migrationBuilder.RenameColumn(
                name: "SupportId",
                table: "Trays",
                newName: "TrayTypeId");

            migrationBuilder.RenameIndex(
                name: "IX_Trays_SupportId",
                table: "Trays",
                newName: "IX_Trays_TrayTypeId");

            migrationBuilder.AlterColumn<double>(
                name: "Length",
                table: "Trays",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "double precision",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "TrayTypes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Width = table.Column<int>(type: "integer", nullable: false),
                    Height = table.Column<int>(type: "integer", nullable: false),
                    Length = table.Column<double>(type: "double precision", nullable: true),
                    Weight = table.Column<double>(type: "double precision", nullable: false),
                    SupportId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TrayTypes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TrayTypes_Supports_SupportId",
                        column: x => x.SupportId,
                        principalTable: "Supports",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TrayTypes_SupportId",
                table: "TrayTypes",
                column: "SupportId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trays_TrayTypes_TrayTypeId",
                table: "Trays",
                column: "TrayTypeId",
                principalTable: "TrayTypes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trays_TrayTypes_TrayTypeId",
                table: "Trays");

            migrationBuilder.DropTable(
                name: "TrayTypes");

            migrationBuilder.RenameColumn(
                name: "TrayTypeId",
                table: "Trays",
                newName: "SupportId");

            migrationBuilder.RenameIndex(
                name: "IX_Trays_TrayTypeId",
                table: "Trays",
                newName: "IX_Trays_SupportId");

            migrationBuilder.AlterColumn<double>(
                name: "Length",
                table: "Trays",
                type: "double precision",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AddColumn<int>(
                name: "Height",
                table: "Trays",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Width",
                table: "Trays",
                type: "integer",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Trays_Supports_SupportId",
                table: "Trays",
                column: "SupportId",
                principalTable: "Supports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
