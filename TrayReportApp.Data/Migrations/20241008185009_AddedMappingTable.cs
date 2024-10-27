using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrayReportApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddedMappingTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Supports_Trays_TrayId",
                table: "Supports");

            migrationBuilder.DropForeignKey(
                name: "FK_Trays_Cables_CableId",
                table: "Trays");

            migrationBuilder.DropIndex(
                name: "IX_Trays_CableId",
                table: "Trays");

            migrationBuilder.DropIndex(
                name: "IX_Supports_TrayId",
                table: "Supports");

            migrationBuilder.DropColumn(
                name: "CableId",
                table: "Trays");

            migrationBuilder.DropColumn(
                name: "TrayId",
                table: "Supports");

            migrationBuilder.AddColumn<int>(
                name: "SupportId",
                table: "Trays",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<double>(
                name: "Weight",
                table: "CableTypes",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<double>(
                name: "Diameter",
                table: "CableTypes",
                type: "double precision",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.CreateTable(
                name: "CableTray",
                columns: table => new
                {
                    CablesId = table.Column<int>(type: "integer", nullable: false),
                    RoutingId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CableTray", x => new { x.CablesId, x.RoutingId });
                    table.ForeignKey(
                        name: "FK_CableTray_Cables_CablesId",
                        column: x => x.CablesId,
                        principalTable: "Cables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CableTray_Trays_RoutingId",
                        column: x => x.RoutingId,
                        principalTable: "Trays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TraysCables",
                columns: table => new
                {
                    TrayId = table.Column<int>(type: "integer", nullable: false),
                    CableId = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TraysCables", x => new { x.CableId, x.TrayId });
                    table.ForeignKey(
                        name: "FK_TraysCables_Cables_CableId",
                        column: x => x.CableId,
                        principalTable: "Cables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TraysCables_Trays_TrayId",
                        column: x => x.TrayId,
                        principalTable: "Trays",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Trays_SupportId",
                table: "Trays",
                column: "SupportId");

            migrationBuilder.CreateIndex(
                name: "IX_CableTray_RoutingId",
                table: "CableTray",
                column: "RoutingId");

            migrationBuilder.CreateIndex(
                name: "IX_TraysCables_TrayId",
                table: "TraysCables",
                column: "TrayId");

            migrationBuilder.AddForeignKey(
                name: "FK_Trays_Supports_SupportId",
                table: "Trays",
                column: "SupportId",
                principalTable: "Supports",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Trays_Supports_SupportId",
                table: "Trays");

            migrationBuilder.DropTable(
                name: "CableTray");

            migrationBuilder.DropTable(
                name: "TraysCables");

            migrationBuilder.DropIndex(
                name: "IX_Trays_SupportId",
                table: "Trays");

            migrationBuilder.DropColumn(
                name: "SupportId",
                table: "Trays");

            migrationBuilder.AddColumn<int>(
                name: "CableId",
                table: "Trays",
                type: "integer",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TrayId",
                table: "Supports",
                type: "integer",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "Weight",
                table: "CableTypes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.AlterColumn<int>(
                name: "Diameter",
                table: "CableTypes",
                type: "integer",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "double precision");

            migrationBuilder.CreateIndex(
                name: "IX_Trays_CableId",
                table: "Trays",
                column: "CableId");

            migrationBuilder.CreateIndex(
                name: "IX_Supports_TrayId",
                table: "Supports",
                column: "TrayId");

            migrationBuilder.AddForeignKey(
                name: "FK_Supports_Trays_TrayId",
                table: "Supports",
                column: "TrayId",
                principalTable: "Trays",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Trays_Cables_CableId",
                table: "Trays",
                column: "CableId",
                principalTable: "Cables",
                principalColumn: "Id");
        }
    }
}
