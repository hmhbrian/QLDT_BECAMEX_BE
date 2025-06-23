using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSpellColumnOfCourseStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_CourseSatus_StatusId",
                table: "Course");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseSatus",
                table: "CourseSatus");

            migrationBuilder.RenameTable(
                name: "CourseSatus",
                newName: "CourseStatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseStatus",
                table: "CourseStatus",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_CourseStatus_StatusId",
                table: "Course",
                column: "StatusId",
                principalTable: "CourseStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Course_CourseStatus_StatusId",
                table: "Course");

            migrationBuilder.DropPrimaryKey(
                name: "PK_CourseStatus",
                table: "CourseStatus");

            migrationBuilder.RenameTable(
                name: "CourseStatus",
                newName: "CourseSatus");

            migrationBuilder.AddPrimaryKey(
                name: "PK_CourseSatus",
                table: "CourseSatus",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Course_CourseSatus_StatusId",
                table: "Course",
                column: "StatusId",
                principalTable: "CourseSatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
