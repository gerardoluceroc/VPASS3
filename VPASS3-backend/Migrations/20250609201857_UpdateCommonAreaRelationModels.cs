using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPASS3_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCommonAreaRelationModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservationCommonAreaGuests");

            migrationBuilder.DropColumn(
                name: "ReservationTime",
                table: "CommonAreas");

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReservationEnd",
                table: "CommonAreaReservations",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ReservationTime",
                table: "CommonAreaReservations",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ReservationTime",
                table: "CommonAreaReservations");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "ReservationTime",
                table: "CommonAreas",
                type: "time",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ReservationEnd",
                table: "CommonAreaReservations",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.CreateTable(
                name: "ReservationCommonAreaGuests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdPerson = table.Column<int>(type: "int", nullable: false),
                    IdReservation = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ReservationCommonAreaGuests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ReservationCommonAreaGuests_CommonAreaReservations_IdReservation",
                        column: x => x.IdReservation,
                        principalTable: "CommonAreaReservations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ReservationCommonAreaGuests_Persons_IdPerson",
                        column: x => x.IdPerson,
                        principalTable: "Persons",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ReservationCommonAreaGuests_IdPerson",
                table: "ReservationCommonAreaGuests",
                column: "IdPerson");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationCommonAreaGuests_IdReservation",
                table: "ReservationCommonAreaGuests",
                column: "IdReservation");
        }
    }
}
