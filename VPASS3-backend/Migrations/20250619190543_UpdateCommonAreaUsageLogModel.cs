using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPASS3_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCommonAreaUsageLogModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CommonAreaUsageLogGuests");

            migrationBuilder.AddColumn<int>(
                name: "GuestsNumber",
                table: "CommonAreaUsageLogs",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "GuestsNumber",
                table: "CommonAreaUsageLogs");

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
                name: "IX_CommonAreaUsageLogGuests_PersonId",
                table: "CommonAreaUsageLogGuests",
                column: "PersonId");
        }
    }
}
