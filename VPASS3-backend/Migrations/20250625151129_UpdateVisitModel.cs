using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPASS3_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVisitModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Visitors_VisitorId",
                table: "Visits");

            migrationBuilder.AlterColumn<int>(
                name: "VisitorId",
                table: "Visits",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "IdPerson",
                table: "Visits",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Visits_IdPerson",
                table: "Visits",
                column: "IdPerson");

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Persons_IdPerson",
                table: "Visits",
                column: "IdPerson",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Visitors_VisitorId",
                table: "Visits",
                column: "VisitorId",
                principalTable: "Visitors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Persons_IdPerson",
                table: "Visits");

            migrationBuilder.DropForeignKey(
                name: "FK_Visits_Visitors_VisitorId",
                table: "Visits");

            migrationBuilder.DropIndex(
                name: "IX_Visits_IdPerson",
                table: "Visits");

            migrationBuilder.DropColumn(
                name: "IdPerson",
                table: "Visits");

            migrationBuilder.AlterColumn<int>(
                name: "VisitorId",
                table: "Visits",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Visits_Visitors_VisitorId",
                table: "Visits",
                column: "VisitorId",
                principalTable: "Visitors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
