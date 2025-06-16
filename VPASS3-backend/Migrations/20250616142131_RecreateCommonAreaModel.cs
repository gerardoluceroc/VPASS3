using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPASS3_backend.Migrations
{
    /// <inheritdoc />
    public partial class RecreateCommonAreaModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommonAreaReservations_CommonAreas_IdReservableCommonArea",
                table: "CommonAreaReservations");

            migrationBuilder.DropTable(
                name: "ReservationCommonAreaGuestPerson");

            migrationBuilder.DropTable(
                name: "UtilizationUsableCommonAreaLogs");

            migrationBuilder.DropColumn(
                name: "ReservationEnd",
                table: "CommonAreaReservations");

            migrationBuilder.RenameColumn(
                name: "Type",
                table: "CommonAreas",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "IdReservableCommonArea",
                table: "CommonAreaReservations",
                newName: "IdCommonArea");

            migrationBuilder.RenameIndex(
                name: "IX_CommonAreaReservations_IdReservableCommonArea",
                table: "CommonAreaReservations",
                newName: "IX_CommonAreaReservations_IdCommonArea");

            migrationBuilder.AddColumn<int>(
                name: "Mode",
                table: "CommonAreas",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "CommonAreaReservationGuests",
                columns: table => new
                {
                    ReservationId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonAreaReservationGuests", x => new { x.ReservationId, x.PersonId });
                    table.ForeignKey(
                        name: "FK_CommonAreaReservationGuests_CommonAreaReservations_ReservationId",
                        column: x => x.ReservationId,
                        principalTable: "CommonAreaReservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommonAreaReservationGuests_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommonAreaUsageLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsageTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    IdPerson = table.Column<int>(type: "int", nullable: false),
                    IdCommonArea = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonAreaUsageLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommonAreaUsageLogs_CommonAreas_IdCommonArea",
                        column: x => x.IdCommonArea,
                        principalTable: "CommonAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommonAreaUsageLogs_Persons_IdPerson",
                        column: x => x.IdPerson,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "CommonAreaUsageLogGuests",
                columns: table => new
                {
                    UsageLogId = table.Column<int>(type: "int", nullable: false),
                    PersonId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonAreaUsageLogGuests", x => new { x.UsageLogId, x.PersonId });
                    table.ForeignKey(
                        name: "FK_CommonAreaUsageLogGuests_CommonAreaUsageLogs_UsageLogId",
                        column: x => x.UsageLogId,
                        principalTable: "CommonAreaUsageLogs",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommonAreaUsageLogGuests_Persons_PersonId",
                        column: x => x.PersonId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommonAreaReservationGuests_PersonId",
                table: "CommonAreaReservationGuests",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonAreaUsageLogGuests_PersonId",
                table: "CommonAreaUsageLogGuests",
                column: "PersonId");

            migrationBuilder.CreateIndex(
                name: "IX_CommonAreaUsageLogs_IdCommonArea",
                table: "CommonAreaUsageLogs",
                column: "IdCommonArea");

            migrationBuilder.CreateIndex(
                name: "IX_CommonAreaUsageLogs_IdPerson",
                table: "CommonAreaUsageLogs",
                column: "IdPerson");

            migrationBuilder.AddForeignKey(
                name: "FK_CommonAreaReservations_CommonAreas_IdCommonArea",
                table: "CommonAreaReservations",
                column: "IdCommonArea",
                principalTable: "CommonAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommonAreaReservations_CommonAreas_IdCommonArea",
                table: "CommonAreaReservations");

            migrationBuilder.DropTable(
                name: "CommonAreaReservationGuests");

            migrationBuilder.DropTable(
                name: "CommonAreaUsageLogGuests");

            migrationBuilder.DropTable(
                name: "CommonAreaUsageLogs");

            migrationBuilder.DropColumn(
                name: "Mode",
                table: "CommonAreas");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "CommonAreas",
                newName: "Type");

            migrationBuilder.RenameColumn(
                name: "IdCommonArea",
                table: "CommonAreaReservations",
                newName: "IdReservableCommonArea");

            migrationBuilder.RenameIndex(
                name: "IX_CommonAreaReservations_IdCommonArea",
                table: "CommonAreaReservations",
                newName: "IX_CommonAreaReservations_IdReservableCommonArea");

            migrationBuilder.AddColumn<DateTime>(
                name: "ReservationEnd",
                table: "CommonAreaReservations",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ReservationCommonAreaGuestPerson",
                columns: table => new
                {
                    GuestsId = table.Column<int>(type: "int", nullable: false),
                    InvitedCommonAreaReservationsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationCommonAreaGuestPerson", x => new { x.GuestsId, x.InvitedCommonAreaReservationsId });
                    table.ForeignKey(
                        name: "FK_ReservationCommonAreaGuestPerson_CommonAreaReservations_InvitedCommonAreaReservationsId",
                        column: x => x.InvitedCommonAreaReservationsId,
                        principalTable: "CommonAreaReservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationCommonAreaGuestPerson_Persons_GuestsId",
                        column: x => x.GuestsId,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UtilizationUsableCommonAreaLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPerson = table.Column<int>(type: "int", nullable: false),
                    IdUsableCommonArea = table.Column<int>(type: "int", nullable: false),
                    GuestsNumber = table.Column<int>(type: "int", nullable: true),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsageTime = table.Column<TimeSpan>(type: "time", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilizationUsableCommonAreaLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UtilizationUsableCommonAreaLogs_CommonAreas_IdUsableCommonArea",
                        column: x => x.IdUsableCommonArea,
                        principalTable: "CommonAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UtilizationUsableCommonAreaLogs_Persons_IdPerson",
                        column: x => x.IdPerson,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationCommonAreaGuestPerson_InvitedCommonAreaReservationsId",
                table: "ReservationCommonAreaGuestPerson",
                column: "InvitedCommonAreaReservationsId");

            migrationBuilder.CreateIndex(
                name: "IX_UtilizationUsableCommonAreaLogs_IdPerson",
                table: "UtilizationUsableCommonAreaLogs",
                column: "IdPerson");

            migrationBuilder.CreateIndex(
                name: "IX_UtilizationUsableCommonAreaLogs_IdUsableCommonArea",
                table: "UtilizationUsableCommonAreaLogs",
                column: "IdUsableCommonArea");

            migrationBuilder.AddForeignKey(
                name: "FK_CommonAreaReservations_CommonAreas_IdReservableCommonArea",
                table: "CommonAreaReservations",
                column: "IdReservableCommonArea",
                principalTable: "CommonAreas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
