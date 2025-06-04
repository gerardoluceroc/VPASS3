using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPASS3_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateParkingSpotUsageLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParkingSpotUsageLogs_ParkingSpots_IdParkingSpot",
                table: "ParkingSpotUsageLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ParkingSpotUsageLogs_Visitors_IdVisitor",
                table: "ParkingSpotUsageLogs");

            migrationBuilder.DropIndex(
                name: "IX_ParkingSpotUsageLogs_IdParkingSpot_IdVisitor_StartTime",
                table: "ParkingSpotUsageLogs");

            migrationBuilder.DropIndex(
                name: "IX_ParkingSpotUsageLogs_IdVisitor",
                table: "ParkingSpotUsageLogs");

            migrationBuilder.DropColumn(
                name: "IdParkingSpot",
                table: "ParkingSpotUsageLogs");

            migrationBuilder.DropColumn(
                name: "IdVisitor",
                table: "ParkingSpotUsageLogs");

            migrationBuilder.AddColumn<int>(
                name: "ParkingSpotId",
                table: "ParkingSpotUsageLogs",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "VisitorId",
                table: "ParkingSpotUsageLogs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSpotUsageLogs_ParkingSpotId",
                table: "ParkingSpotUsageLogs",
                column: "ParkingSpotId");

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSpotUsageLogs_VisitorId",
                table: "ParkingSpotUsageLogs",
                column: "VisitorId");

            migrationBuilder.AddForeignKey(
                name: "FK_ParkingSpotUsageLogs_ParkingSpots_ParkingSpotId",
                table: "ParkingSpotUsageLogs",
                column: "ParkingSpotId",
                principalTable: "ParkingSpots",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ParkingSpotUsageLogs_Visitors_VisitorId",
                table: "ParkingSpotUsageLogs",
                column: "VisitorId",
                principalTable: "Visitors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ParkingSpotUsageLogs_ParkingSpots_ParkingSpotId",
                table: "ParkingSpotUsageLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ParkingSpotUsageLogs_Visitors_VisitorId",
                table: "ParkingSpotUsageLogs");

            migrationBuilder.DropIndex(
                name: "IX_ParkingSpotUsageLogs_ParkingSpotId",
                table: "ParkingSpotUsageLogs");

            migrationBuilder.DropIndex(
                name: "IX_ParkingSpotUsageLogs_VisitorId",
                table: "ParkingSpotUsageLogs");

            migrationBuilder.DropColumn(
                name: "ParkingSpotId",
                table: "ParkingSpotUsageLogs");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "ParkingSpotUsageLogs");

            migrationBuilder.AddColumn<int>(
                name: "IdParkingSpot",
                table: "ParkingSpotUsageLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "IdVisitor",
                table: "ParkingSpotUsageLogs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSpotUsageLogs_IdParkingSpot_IdVisitor_StartTime",
                table: "ParkingSpotUsageLogs",
                columns: new[] { "IdParkingSpot", "IdVisitor", "StartTime" });

            migrationBuilder.CreateIndex(
                name: "IX_ParkingSpotUsageLogs_IdVisitor",
                table: "ParkingSpotUsageLogs",
                column: "IdVisitor");

            migrationBuilder.AddForeignKey(
                name: "FK_ParkingSpotUsageLogs_ParkingSpots_IdParkingSpot",
                table: "ParkingSpotUsageLogs",
                column: "IdParkingSpot",
                principalTable: "ParkingSpots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ParkingSpotUsageLogs_Visitors_IdVisitor",
                table: "ParkingSpotUsageLogs",
                column: "IdVisitor",
                principalTable: "Visitors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
