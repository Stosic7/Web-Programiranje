using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebTemplate.Migrations
{
    /// <inheritdoc />
    public partial class v1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Aerodromi",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Kod = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    Lokacija = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KapacitetLetelica = table.Column<int>(type: "int", nullable: false),
                    KapacitetPutnika = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Aerodromi", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Letelice",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Naziv = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    KapacitetPutnika = table.Column<int>(type: "int", nullable: false),
                    BrojOsoblja = table.Column<int>(type: "int", nullable: false),
                    BrojMotora = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Letelice", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Letovi",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BrojPutnika = table.Column<int>(type: "int", nullable: false),
                    VremePoletanja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    VremeSletanja = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PolazniAerodromID = table.Column<int>(type: "int", nullable: true),
                    DolazniAerodromID = table.Column<int>(type: "int", nullable: true),
                    LetelicaID = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Letovi", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Letovi_Aerodromi_DolazniAerodromID",
                        column: x => x.DolazniAerodromID,
                        principalTable: "Aerodromi",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Letovi_Aerodromi_PolazniAerodromID",
                        column: x => x.PolazniAerodromID,
                        principalTable: "Aerodromi",
                        principalColumn: "ID");
                    table.ForeignKey(
                        name: "FK_Letovi_Letelice_LetelicaID",
                        column: x => x.LetelicaID,
                        principalTable: "Letelice",
                        principalColumn: "ID");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Letovi_DolazniAerodromID",
                table: "Letovi",
                column: "DolazniAerodromID");

            migrationBuilder.CreateIndex(
                name: "IX_Letovi_LetelicaID",
                table: "Letovi",
                column: "LetelicaID");

            migrationBuilder.CreateIndex(
                name: "IX_Letovi_PolazniAerodromID",
                table: "Letovi",
                column: "PolazniAerodromID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Letovi");

            migrationBuilder.DropTable(
                name: "Aerodromi");

            migrationBuilder.DropTable(
                name: "Letelice");
        }
    }
}
