using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TrayReportApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class FixedCablesRouting : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CableTray");

            migrationBuilder.DropTable(
                name: "TraysCables");

            migrationBuilder.AddColumn<string>(
                name: "Routing",
                table: "Cables",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Routing",
                table: "Cables");

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
                    CableId = table.Column<int>(type: "integer", nullable: false),
                    TrayId = table.Column<int>(type: "integer", nullable: false)
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
                name: "IX_CableTray_RoutingId",
                table: "CableTray",
                column: "RoutingId");

            migrationBuilder.CreateIndex(
                name: "IX_TraysCables_TrayId",
                table: "TraysCables",
                column: "TrayId");
        }
    }
}
