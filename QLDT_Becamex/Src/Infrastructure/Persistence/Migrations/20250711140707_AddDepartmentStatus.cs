using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class AddDepartmentStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "status",
                table: "Departments",
                newName: "status_id");

            migrationBuilder.CreateTable(
                name: "DepartmentStatus",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentStatus", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Departments_status_id",
                table: "Departments",
                column: "status_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_DepartmentStatus_status_id",
                table: "Departments",
                column: "status_id",
                principalTable: "DepartmentStatus",
                principalColumn: "id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Departments_DepartmentStatus_status_id",
                table: "Departments");

            migrationBuilder.DropTable(
                name: "DepartmentStatus");

            migrationBuilder.DropIndex(
                name: "IX_Departments_status_id",
                table: "Departments");

            migrationBuilder.RenameColumn(
                name: "status_id",
                table: "Departments",
                newName: "status");
        }
    }
}
