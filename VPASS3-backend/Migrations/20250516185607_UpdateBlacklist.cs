using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VPASS3_backend.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBlacklist : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blacklist_Establishments_IdEstablishment",
                table: "Blacklist");

            migrationBuilder.DropForeignKey(
                name: "FK_Blacklist_Visitors_IdVisitor",
                table: "Blacklist");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Blacklist",
                table: "Blacklist");

            migrationBuilder.RenameTable(
                name: "Blacklist",
                newName: "Blacklists");

            migrationBuilder.RenameIndex(
                name: "IX_Blacklist_IdVisitor",
                table: "Blacklists",
                newName: "IX_Blacklists_IdVisitor");

            migrationBuilder.RenameIndex(
                name: "IX_Blacklist_IdEstablishment",
                table: "Blacklists",
                newName: "IX_Blacklists_IdEstablishment");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Blacklists",
                table: "Blacklists",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Blacklists_Establishments_IdEstablishment",
                table: "Blacklists",
                column: "IdEstablishment",
                principalTable: "Establishments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Blacklists_Visitors_IdVisitor",
                table: "Blacklists",
                column: "IdVisitor",
                principalTable: "Visitors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Blacklists_Establishments_IdEstablishment",
                table: "Blacklists");

            migrationBuilder.DropForeignKey(
                name: "FK_Blacklists_Visitors_IdVisitor",
                table: "Blacklists");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Blacklists",
                table: "Blacklists");

            migrationBuilder.RenameTable(
                name: "Blacklists",
                newName: "Blacklist");

            migrationBuilder.RenameIndex(
                name: "IX_Blacklists_IdVisitor",
                table: "Blacklist",
                newName: "IX_Blacklist_IdVisitor");

            migrationBuilder.RenameIndex(
                name: "IX_Blacklists_IdEstablishment",
                table: "Blacklist",
                newName: "IX_Blacklist_IdEstablishment");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Blacklist",
                table: "Blacklist",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Blacklist_Establishments_IdEstablishment",
                table: "Blacklist",
                column: "IdEstablishment",
                principalTable: "Establishments",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Blacklist_Visitors_IdVisitor",
                table: "Blacklist",
                column: "IdVisitor",
                principalTable: "Visitors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
