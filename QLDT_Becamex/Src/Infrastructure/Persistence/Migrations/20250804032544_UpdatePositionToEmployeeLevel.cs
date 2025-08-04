using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePositionToEmployeeLevel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Positions_position_id",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "CoursePosition");

            migrationBuilder.RenameColumn(
                name: "position_name",
                table: "Positions",
                newName: "elevel_name");

            migrationBuilder.RenameColumn(
                name: "position_id",
                table: "Positions",
                newName: "elevel_id");

            migrationBuilder.RenameColumn(
                name: "position_id",
                table: "AspNetUsers",
                newName: "elevel_id");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_position_id",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_elevel_id");

            migrationBuilder.CreateTable(
                name: "CourseELevel",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    elevel_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CourseELevel", x => x.id);
                    table.ForeignKey(
                        name: "FK_CourseELevel_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CourseELevel_Positions_elevel_id",
                        column: x => x.elevel_id,
                        principalTable: "Positions",
                        principalColumn: "elevel_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CourseELevel_course_id",
                table: "CourseELevel",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_CourseELevel_elevel_id",
                table: "CourseELevel",
                column: "elevel_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Positions_elevel_id",
                table: "AspNetUsers",
                column: "elevel_id",
                principalTable: "Positions",
                principalColumn: "elevel_id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Positions_elevel_id",
                table: "AspNetUsers");

            migrationBuilder.DropTable(
                name: "CourseELevel");

            migrationBuilder.RenameColumn(
                name: "elevel_name",
                table: "Positions",
                newName: "position_name");

            migrationBuilder.RenameColumn(
                name: "elevel_id",
                table: "Positions",
                newName: "position_id");

            migrationBuilder.RenameColumn(
                name: "elevel_id",
                table: "AspNetUsers",
                newName: "position_id");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_elevel_id",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_position_id");

            migrationBuilder.CreateTable(
                name: "CoursePosition",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    position_id = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoursePosition", x => x.id);
                    table.ForeignKey(
                        name: "FK_CoursePosition_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CoursePosition_Positions_position_id",
                        column: x => x.position_id,
                        principalTable: "Positions",
                        principalColumn: "position_id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CoursePosition_course_id",
                table: "CoursePosition",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_CoursePosition_position_id",
                table: "CoursePosition",
                column: "position_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Positions_position_id",
                table: "AspNetUsers",
                column: "position_id",
                principalTable: "Positions",
                principalColumn: "position_id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
