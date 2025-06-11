using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Src.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleIdToPosition : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
                name: "FK_Positions_AspNetRoles_RoleId",
                table: "Positions",
                column: "RoleId",
                principalTable: "AspNetRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Positions_AspNetRoles_RoleId",
                table: "Positions");

            migrationBuilder.DropIndex(
                name: "IX_Positions_RoleId",
                table: "Positions");

            migrationBuilder.DropColumn(
                name: "RoleId",
                table: "Positions");
        }
    }
}
