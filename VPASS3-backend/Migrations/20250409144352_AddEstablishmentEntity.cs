using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPASS3_backend.Migrations
{
    /// <inheritdoc />
    public partial class AddEstablishmentEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "EstablishmentId",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Establishments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Establishments", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_EstablishmentId",
                table: "AspNetUsers",
                column: "EstablishmentId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Establishments_EstablishmentId",
                table: "AspNetUsers",
                column: "EstablishmentId",
                principalTable: "Establishments",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Establishments_EstablishmentId",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "Establishments");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_EstablishmentId",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EstablishmentId",
                table: "AspNetUsers");
        }
    }
}
