using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Src.Migrations
{
    /// <inheritdoc />
    public partial class RemoveManagerIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ManagerId",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Positions_AspNetRoles_RoleId",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Positions_RoleId",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Positions");

            migrationBuilder.RenameColumn(
                name: "ManagerId",
                table: "AspNetUsers",
                newName: "ApplicationUserId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_ManagerId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_ApplicationUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ApplicationUserId",
                table: "AspNetUsers",
                column: "ApplicationUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ApplicationUserId",
                table: "AspNetUsers");

            migrationBuilder.RenameColumn(
                name: "ApplicationUserId",
                table: "AspNetUsers",
                newName: "ManagerId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_ApplicationUserId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_ManagerId");

            migrationBuilder.AddColumn<string>(
                name: "RoleId",
                table: "Positions",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Positions_RoleId",
                table: "Positions",
                column: "RoleId");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ManagerId",
                table: "AspNetUsers",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Positions_AspNetRoles_RoleId",
                table: "Positions",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
