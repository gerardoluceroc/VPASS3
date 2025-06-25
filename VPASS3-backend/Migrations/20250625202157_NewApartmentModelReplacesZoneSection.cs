using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPASS3_backend.Migrations
{
    /// <inheritdoc />
    public partial class NewApartmentModelReplacesZoneSection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blacklists_Visitors_VisitorId",
                table: "Blacklists");

            migrationBuilder.DropForeignKey(
                name: "FK_ParkingSpotUsageLogs_Visitors_VisitorId",
                table: "ParkingSpotUsageLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Visitors_VisitorId",
                table: "Visits");

            migrationBuilder.DropForeignKey(
                name: "FK_Visits_ZoneSections_IdZoneSection",
                table: "Visits");

            migrationBuilder.DropTable(
                name: "Visitors");

            migrationBuilder.DropTable(
                name: "ZoneSections");

            migrationBuilder.DropIndex(
                name: "IX_Visits_IdZoneSection",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_ParkingSpotUsageLogs_VisitorId",
                table: "ParkingSpotUsageLogs");

            migrationBuilder.DropIndex(
                name: "IX_Blacklists_VisitorId",
                table: "Blacklists");

            migrationBuilder.DropColumn(
                name: "IdZoneSection",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "ParkingSpotUsageLogs");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "Blacklists");

            migrationBuilder.RenameColumn(
                name: "VisitorId",
                table: "Visits",
                newName: "IdApartment");

            migrationBuilder.RenameIndex(
                name: "IX_Visits_VisitorId",
                table: "Visits",
                newName: "IX_Visits_IdApartment");

            migrationBuilder.CreateTable(
                name: "Apartments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdZone = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apartments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Apartments_Zones_IdZone",
                        column: x => x.IdZone,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Apartments_IdZone",
                table: "Apartments",
                column: "IdZone");

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Apartments_IdApartment",
                table: "Visits",
                column: "IdApartment",
                principalTable: "Apartments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Apartments_IdApartment",
                table: "Visits");

            migrationBuilder.DropTable(
                name: "Apartments");

            migrationBuilder.RenameColumn(
                name: "IdApartment",
                table: "Visits",
                newName: "VisitorId");

            migrationBuilder.RenameIndex(
                name: "IX_Visits_IdApartment",
                table: "Visits",
                newName: "IX_Visits_VisitorId");

            migrationBuilder.AddColumn<int>(
                name: "IdZoneSection",
                table: "Visits",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VisitorId",
                table: "ParkingSpotUsageLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VisitorId",
                table: "Blacklists",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Visitors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdentificationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastNames = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Names = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Visitors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ZoneSections",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdZone = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ZoneSections", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ZoneSections_Zones_IdZone",
                        column: x => x.IdZone,
                        principalTable: "Zones",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Visits_IdZoneSection",
                table: "Visits",
                column: "IdZoneSection");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSpotUsageLogs_VisitorId",
                table: "ParkingSpotUsageLogs",
                column: "VisitorId");

            migrationBuilder.CreateIndex(
                name: "IX_Blacklists_VisitorId",
                table: "Blacklists",
                column: "VisitorId");

            migrationBuilder.CreateIndex(
                name: "IX_ZoneSections_IdZone",
                table: "ZoneSections",
                column: "IdZone");

            migrationBuilder.AddForeignKey(
                name: "FK_Blacklists_Visitors_VisitorId",
                table: "Blacklists",
                column: "VisitorId",
                principalTable: "Visitors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ParkingSpotUsageLogs_Visitors_VisitorId",
                table: "ParkingSpotUsageLogs",
                column: "VisitorId",
                principalTable: "Visitors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Visitors_VisitorId",
                table: "Visits",
                column: "VisitorId",
                principalTable: "Visitors",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_ZoneSections_IdZoneSection",
                table: "Visits",
                column: "IdZoneSection",
                principalTable: "ZoneSections",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
