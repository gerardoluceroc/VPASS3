using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPASS3_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCommonAreasModels : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CommonAreas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    IdEstablishment = table.Column<int>(type: "int", nullable: false),
                    Type = table.Column<int>(type: "int", nullable: false),
                    MaxReservationTime = table.Column<TimeSpan>(type: "time", nullable: true),
                    MaxCapacity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonAreas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommonAreas_Establishments_IdEstablishment",
                        column: x => x.IdEstablishment,
                        principalTable: "Establishments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CommonAreaReservations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdReservableCommonArea = table.Column<int>(type: "int", nullable: false),
                    ReservationStart = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReservationEnd = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IdVisitorReservedBy = table.Column<int>(type: "int", nullable: false),
                    ReservedById = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CommonAreaReservations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CommonAreaReservations_CommonAreas_IdReservableCommonArea",
                        column: x => x.IdReservableCommonArea,
                        principalTable: "CommonAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CommonAreaReservations_Visitors_ReservedById",
                        column: x => x.ReservedById,
                        principalTable: "Visitors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UtilizationUsableCommonAreaLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdUtilizationUsableCommonAreaLog = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IdVisitor = table.Column<int>(type: "int", nullable: false),
                    GuestsNumber = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UtilizationUsableCommonAreaLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UtilizationUsableCommonAreaLogs_CommonAreas_IdUtilizationUsableCommonAreaLog",
                        column: x => x.IdUtilizationUsableCommonAreaLog,
                        principalTable: "CommonAreas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UtilizationUsableCommonAreaLogs_Visitors_IdVisitor",
                        column: x => x.IdVisitor,
                        principalTable: "Visitors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ReservationCommonAreaGuests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdVisitor = table.Column<int>(type: "int", nullable: false),
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
                        name: "FK_ReservationCommonAreaGuests_Visitors_IdVisitor",
                        column: x => x.IdVisitor,
                        principalTable: "Visitors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CommonAreaReservations_IdReservableCommonArea",
                table: "CommonAreaReservations",
                column: "IdReservableCommonArea");

            migrationBuilder.CreateIndex(
                name: "IX_CommonAreaReservations_ReservedById",
                table: "CommonAreaReservations",
                column: "ReservedById");

            migrationBuilder.CreateIndex(
                name: "IX_CommonAreas_IdEstablishment",
                table: "CommonAreas",
                column: "IdEstablishment");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationCommonAreaGuests_IdReservation",
                table: "ReservationCommonAreaGuests",
                column: "IdReservation");

            migrationBuilder.CreateIndex(
                name: "IX_ReservationCommonAreaGuests_IdVisitor",
                table: "ReservationCommonAreaGuests",
                column: "IdVisitor");

            migrationBuilder.CreateIndex(
                name: "IX_UtilizationUsableCommonAreaLogs_IdUtilizationUsableCommonAreaLog",
                table: "UtilizationUsableCommonAreaLogs",
                column: "IdUtilizationUsableCommonAreaLog");

            migrationBuilder.CreateIndex(
                name: "IX_UtilizationUsableCommonAreaLogs_IdVisitor",
                table: "UtilizationUsableCommonAreaLogs",
                column: "IdVisitor");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ReservationCommonAreaGuests");

            migrationBuilder.DropTable(
                name: "UtilizationUsableCommonAreaLogs");

            migrationBuilder.DropTable(
                name: "CommonAreaReservations");

            migrationBuilder.DropTable(
                name: "CommonAreas");
        }
    }
}
