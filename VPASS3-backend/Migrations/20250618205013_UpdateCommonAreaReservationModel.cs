using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPASS3_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCommonAreaReservationModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommonAreaReservationGuests");

            migrationBuilder.AddColumn<int>(
                name: "GuestsNumber",
                table: "CommonAreaReservations",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PersonId",
                table: "CommonAreaReservations",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_CommonAreaReservations_PersonId",
                table: "CommonAreaReservations",
                column: "PersonId");

            migrationBuilder.AddForeignKey(
                name: "FK_CommonAreaReservations_Persons_PersonId",
                table: "CommonAreaReservations",
                column: "PersonId",
                principalTable: "Persons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CommonAreaReservations_Persons_PersonId",
                table: "CommonAreaReservations");

            migrationBuilder.DropIndex(
                name: "IX_CommonAreaReservations_PersonId",
                table: "CommonAreaReservations");

            migrationBuilder.DropColumn(
                name: "GuestsNumber",
                table: "CommonAreaReservations");

            migrationBuilder.DropColumn(
                name: "PersonId",
                table: "CommonAreaReservations");

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

            migrationBuilder.CreateIndex(
                name: "IX_CommonAreaReservationGuests_PersonId",
                table: "CommonAreaReservationGuests",
                column: "PersonId");
        }
    }
}
