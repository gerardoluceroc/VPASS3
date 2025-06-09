using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPASS3_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCommonAreasRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommonAreaReservations_Person_IdPersonReservedBy",
                table: "CommonAreaReservations");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationCommonAreaGuests_Person_IdPerson",
                table: "ReservationCommonAreaGuests");

            migrationBuilder.DropForeignKey(
                name: "FK_UtilizationUsableCommonAreaLogs_Person_IdPerson",
                table: "UtilizationUsableCommonAreaLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Person",
                table: "Person");

            migrationBuilder.RenameTable(
                name: "Person",
                newName: "Persons");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Persons",
                table: "Persons",
                column: "Id");

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

            migrationBuilder.CreateIndex(
                name: "IX_ReservationCommonAreaGuestPerson_InvitedCommonAreaReservationsId",
                table: "ReservationCommonAreaGuestPerson",
                column: "InvitedCommonAreaReservationsId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommonAreaReservations_Persons_IdPersonReservedBy",
                table: "CommonAreaReservations",
                column: "IdPersonReservedBy",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ReservationCommonAreaGuests_Persons_IdPerson",
                table: "ReservationCommonAreaGuests",
                column: "IdPerson",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UtilizationUsableCommonAreaLogs_Persons_IdPerson",
                table: "UtilizationUsableCommonAreaLogs",
                column: "IdPerson",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommonAreaReservations_Persons_IdPersonReservedBy",
                table: "CommonAreaReservations");

            migrationBuilder.DropForeignKey(
                name: "FK_ReservationCommonAreaGuests_Persons_IdPerson",
                table: "ReservationCommonAreaGuests");

            migrationBuilder.DropForeignKey(
                name: "FK_UtilizationUsableCommonAreaLogs_Persons_IdPerson",
                table: "UtilizationUsableCommonAreaLogs");

            migrationBuilder.DropTable(
                name: "ReservationCommonAreaGuestPerson");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Persons",
                table: "Persons");

            migrationBuilder.RenameTable(
                name: "Persons",
                newName: "Person");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Person",
                table: "Person",
                column: "Id");

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
                name: "FK_UtilizationUsableCommonAreaLogs_Person_IdPerson",
                table: "UtilizationUsableCommonAreaLogs",
                column: "IdPerson",
                principalTable: "Person",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
