using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPASS3_backend.Migrations
{
    /// <inheritdoc />
    public partial class VisitTypeUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdEstablishment",
                table: "VisitTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VisitTypes_IdEstablishment",
                table: "VisitTypes",
                column: "IdEstablishment");

            migrationBuilder.AddForeignKey(
                name: "FK_VisitTypes_Establishments_IdEstablishment",
                table: "VisitTypes",
                column: "IdEstablishment",
                principalTable: "Establishments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VisitTypes_Establishments_IdEstablishment",
                table: "VisitTypes");

            migrationBuilder.DropIndex(
                name: "IX_VisitTypes_IdEstablishment",
                table: "VisitTypes");

            migrationBuilder.DropColumn(
                name: "IdEstablishment",
                table: "VisitTypes");
        }
    }
}
