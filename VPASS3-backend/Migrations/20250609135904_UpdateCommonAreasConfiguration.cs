using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPASS3_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCommonAreasConfiguration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommonAreaReservations_Visitors_ReservedById",
                table: "CommonAreaReservations");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationCommonAreaGuests_Visitors_IdVisitor",
                table: "ReservationCommonAreaGuests");

            migrationBuilder.DropForeignKey(
                name: "FK_UtilizationUsableCommonAreaLogs_CommonAreas_IdUtilizationUsableCommonAreaLog",
                table: "UtilizationUsableCommonAreaLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_UtilizationUsableCommonAreaLogs_Visitors_IdVisitor",
                table: "UtilizationUsableCommonAreaLogs");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "UtilizationUsableCommonAreaLogs");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "CommonAreas");

            migrationBuilder.DropColumn(
                name: "IdVisitorReservedBy",
                table: "CommonAreaReservations");

            migrationBuilder.RenameColumn(
                name: "IdVisitor",
                table: "UtilizationUsableCommonAreaLogs",
                newName: "IdUsableCommonArea");

            migrationBuilder.RenameColumn(
                name: "IdUtilizationUsableCommonAreaLog",
                table: "UtilizationUsableCommonAreaLogs",
                newName: "IdPerson");

            migrationBuilder.RenameIndex(
                name: "IX_UtilizationUsableCommonAreaLogs_IdVisitor",
                table: "UtilizationUsableCommonAreaLogs",
                newName: "IX_UtilizationUsableCommonAreaLogs_IdUsableCommonArea");

            migrationBuilder.RenameIndex(
                name: "IX_UtilizationUsableCommonAreaLogs_IdUtilizationUsableCommonAreaLog",
                table: "UtilizationUsableCommonAreaLogs",
                newName: "IX_UtilizationUsableCommonAreaLogs_IdPerson");

            migrationBuilder.RenameColumn(
                name: "IdVisitor",
                table: "ReservationCommonAreaGuests",
                newName: "IdPerson");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationCommonAreaGuests_IdVisitor",
                table: "ReservationCommonAreaGuests",
                newName: "IX_ReservationCommonAreaGuests_IdPerson");

            migrationBuilder.RenameColumn(
                name: "MaxReservationTime",
                table: "CommonAreas",
                newName: "ReservationTime");

            migrationBuilder.RenameColumn(
                name: "ReservedById",
                table: "CommonAreaReservations",
                newName: "IdPersonReservedBy");

            migrationBuilder.RenameIndex(
                name: "IX_CommonAreaReservations_ReservedById",
                table: "CommonAreaReservations",
                newName: "IX_CommonAreaReservations_IdPersonReservedBy");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "UsageTime",
                table: "UtilizationUsableCommonAreaLogs",
                type: "time",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Person",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Names = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    LastNames = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdentificationNumber = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Person", x => x.Id);
                });

            migrationBuilder.AddForeignKey(
                name: "FK_CommonAreaReservations_Person_IdPersonReservedBy",
                table: "CommonAreaReservations",
                column: "IdPersonReservedBy",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationCommonAreaGuests_Person_IdPerson",
                table: "ReservationCommonAreaGuests",
                column: "IdPerson",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UtilizationUsableCommonAreaLogs_CommonAreas_IdUsableCommonArea",
                table: "UtilizationUsableCommonAreaLogs",
                column: "IdUsableCommonArea",
                principalTable: "CommonAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UtilizationUsableCommonAreaLogs_Person_IdPerson",
                table: "UtilizationUsableCommonAreaLogs",
                column: "IdPerson",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommonAreaReservations_Person_IdPersonReservedBy",
                table: "CommonAreaReservations");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationCommonAreaGuests_Person_IdPerson",
                table: "ReservationCommonAreaGuests");

            migrationBuilder.DropForeignKey(
                name: "FK_UtilizationUsableCommonAreaLogs_CommonAreas_IdUsableCommonArea",
                table: "UtilizationUsableCommonAreaLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_UtilizationUsableCommonAreaLogs_Person_IdPerson",
                table: "UtilizationUsableCommonAreaLogs");

            migrationBuilder.DropTable(
                name: "Person");

            migrationBuilder.DropColumn(
                name: "UsageTime",
                table: "UtilizationUsableCommonAreaLogs");

            migrationBuilder.RenameColumn(
                name: "IdUsableCommonArea",
                table: "UtilizationUsableCommonAreaLogs",
                newName: "IdVisitor");

            migrationBuilder.RenameColumn(
                name: "IdPerson",
                table: "UtilizationUsableCommonAreaLogs",
                newName: "IdUtilizationUsableCommonAreaLog");

            migrationBuilder.RenameIndex(
                name: "IX_UtilizationUsableCommonAreaLogs_IdUsableCommonArea",
                table: "UtilizationUsableCommonAreaLogs",
                newName: "IX_UtilizationUsableCommonAreaLogs_IdVisitor");

            migrationBuilder.RenameIndex(
                name: "IX_UtilizationUsableCommonAreaLogs_IdPerson",
                table: "UtilizationUsableCommonAreaLogs",
                newName: "IX_UtilizationUsableCommonAreaLogs_IdUtilizationUsableCommonAreaLog");

            migrationBuilder.RenameColumn(
                name: "IdPerson",
                table: "ReservationCommonAreaGuests",
                newName: "IdVisitor");

            migrationBuilder.RenameIndex(
                name: "IX_ReservationCommonAreaGuests_IdPerson",
                table: "ReservationCommonAreaGuests",
                newName: "IX_ReservationCommonAreaGuests_IdVisitor");

            migrationBuilder.RenameColumn(
                name: "ReservationTime",
                table: "CommonAreas",
                newName: "MaxReservationTime");

            migrationBuilder.RenameColumn(
                name: "IdPersonReservedBy",
                table: "CommonAreaReservations",
                newName: "ReservedById");

            migrationBuilder.RenameIndex(
                name: "IX_CommonAreaReservations_IdPersonReservedBy",
                table: "CommonAreaReservations",
                newName: "IX_CommonAreaReservations_ReservedById");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "UtilizationUsableCommonAreaLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "CommonAreas",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdVisitorReservedBy",
                table: "CommonAreaReservations",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddForeignKey(
                name: "FK_CommonAreaReservations_Visitors_ReservedById",
                table: "CommonAreaReservations",
                column: "ReservedById",
                principalTable: "Visitors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationCommonAreaGuests_Visitors_IdVisitor",
                table: "ReservationCommonAreaGuests",
                column: "IdVisitor",
                principalTable: "Visitors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UtilizationUsableCommonAreaLogs_CommonAreas_IdUtilizationUsableCommonAreaLog",
                table: "UtilizationUsableCommonAreaLogs",
                column: "IdUtilizationUsableCommonAreaLog",
                principalTable: "CommonAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UtilizationUsableCommonAreaLogs_Visitors_IdVisitor",
                table: "UtilizationUsableCommonAreaLogs",
                column: "IdVisitor",
                principalTable: "Visitors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
