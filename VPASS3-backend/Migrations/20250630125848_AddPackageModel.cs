using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPASS3_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPackageModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Packages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdApartment = table.Column<int>(type: "int", nullable: false),
                    IdApartmentOwnership = table.Column<int>(type: "int", nullable: false),
                    Code = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ReceivedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeliveredAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdPersonWhoReceived = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Packages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Packages_ApartmentOwnerships_IdApartmentOwnership",
                        column: x => x.IdApartmentOwnership,
                        principalTable: "ApartmentOwnerships",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Packages_Apartments_IdApartment",
                        column: x => x.IdApartment,
                        principalTable: "Apartments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Packages_Persons_IdPersonWhoReceived",
                        column: x => x.IdPersonWhoReceived,
                        principalTable: "Persons",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Packages_IdApartment",
                table: "Packages",
                column: "IdApartment");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_IdApartmentOwnership",
                table: "Packages",
                column: "IdApartmentOwnership");

            migrationBuilder.CreateIndex(
                name: "IX_Packages_IdPersonWhoReceived",
                table: "Packages",
                column: "IdPersonWhoReceived");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Packages");
        }
    }
}
