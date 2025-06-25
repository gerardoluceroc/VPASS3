using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPASS3_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBlacklistModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blacklists_Visitors_IdVisitor",
                table: "Blacklists");

            migrationBuilder.RenameColumn(
                name: "IdVisitor",
                table: "Blacklists",
                newName: "IdPerson");

            migrationBuilder.RenameIndex(
                name: "IX_Blacklists_IdVisitor",
                table: "Blacklists",
                newName: "IX_Blacklists_IdPerson");

            migrationBuilder.AddColumn<int>(
                name: "VisitorId",
                table: "Blacklists",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Blacklists_VisitorId",
                table: "Blacklists",
                column: "VisitorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Blacklists_Persons_IdPerson",
                table: "Blacklists",
                column: "IdPerson",
                principalTable: "Persons",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Blacklists_Visitors_VisitorId",
                table: "Blacklists",
                column: "VisitorId",
                principalTable: "Visitors",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blacklists_Persons_IdPerson",
                table: "Blacklists");

            migrationBuilder.DropForeignKey(
                name: "FK_Blacklists_Visitors_VisitorId",
                table: "Blacklists");

            migrationBuilder.DropIndex(
                name: "IX_Blacklists_VisitorId",
                table: "Blacklists");

            migrationBuilder.DropColumn(
                name: "VisitorId",
                table: "Blacklists");

            migrationBuilder.RenameColumn(
                name: "IdPerson",
                table: "Blacklists",
                newName: "IdVisitor");

            migrationBuilder.RenameIndex(
                name: "IX_Blacklists_IdPerson",
                table: "Blacklists",
                newName: "IX_Blacklists_IdVisitor");

            migrationBuilder.AddForeignKey(
                name: "FK_Blacklists_Visitors_IdVisitor",
                table: "Blacklists",
                column: "IdVisitor",
                principalTable: "Visitors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
