using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace UBahnApi.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Linien",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Linien", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Stationen",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "longtext", nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    LinieId = table.Column<int>(type: "int", nullable: false),
                    Position = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Stationen", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Stationen_Linien_LinieId",
                        column: x => x.LinieId,
                        principalTable: "Linien",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Fahrzeiten",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    VonStationId = table.Column<int>(type: "int", nullable: false),
                    NachStationId = table.Column<int>(type: "int", nullable: false),
                    DauerMinuten = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Fahrzeiten", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Fahrzeiten_Stationen_NachStationId",
                        column: x => x.NachStationId,
                        principalTable: "Stationen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Fahrzeiten_Stationen_VonStationId",
                        column: x => x.VonStationId,
                        principalTable: "Stationen",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Fahrzeiten_NachStationId",
                table: "Fahrzeiten",
                column: "NachStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Fahrzeiten_VonStationId",
                table: "Fahrzeiten",
                column: "VonStationId");

            migrationBuilder.CreateIndex(
                name: "IX_Stationen_LinieId",
                table: "Stationen",
                column: "LinieId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Fahrzeiten");

            migrationBuilder.DropTable(
                name: "Stationen");

            migrationBuilder.DropTable(
                name: "Linien");
        }
    }
}
