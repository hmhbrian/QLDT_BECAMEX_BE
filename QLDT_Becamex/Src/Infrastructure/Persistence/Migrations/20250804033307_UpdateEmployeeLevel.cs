using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class UpdateEmployeeLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Positions_elevel_id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseELevel_Positions_elevel_id",
                table: "CourseELevel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Positions",
                table: "Positions");

            migrationBuilder.RenameTable(
                name: "Positions",
                newName: "EmployeeLevel");

            migrationBuilder.AddPrimaryKey(
                name: "PK_EmployeeLevel",
                table: "EmployeeLevel",
                column: "elevel_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_EmployeeLevel_elevel_id",
                table: "AspNetUsers",
                column: "elevel_id",
                principalTable: "EmployeeLevel",
                principalColumn: "elevel_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseELevel_EmployeeLevel_elevel_id",
                table: "CourseELevel",
                column: "elevel_id",
                principalTable: "EmployeeLevel",
                principalColumn: "elevel_id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_EmployeeLevel_elevel_id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseELevel_EmployeeLevel_elevel_id",
                table: "CourseELevel");

            migrationBuilder.DropPrimaryKey(
                name: "PK_EmployeeLevel",
                table: "EmployeeLevel");

            migrationBuilder.RenameTable(
                name: "EmployeeLevel",
                newName: "Positions");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Positions",
                table: "Positions",
                column: "elevel_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Positions_elevel_id",
                table: "AspNetUsers",
                column: "elevel_id",
                principalTable: "Positions",
                principalColumn: "elevel_id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseELevel_Positions_elevel_id",
                table: "CourseELevel",
                column: "elevel_id",
                principalTable: "Positions",
                principalColumn: "elevel_id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
