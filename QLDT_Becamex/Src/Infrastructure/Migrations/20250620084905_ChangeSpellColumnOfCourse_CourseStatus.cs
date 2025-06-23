using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class ChangeSpellColumnOfCourse_CourseStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sesstions",
                table: "Course",
                newName: "Sessions");

            migrationBuilder.RenameColumn(
                name: "RegistrationSlosingDate",
                table: "Course",
                newName: "RegistrationClosingDate");

            migrationBuilder.RenameColumn(
                name: "Objecttives",
                table: "Course",
                newName: "Objectives");

            migrationBuilder.RenameColumn(
                name: "HoursPerSesstions",
                table: "Course",
                newName: "HoursPerSessions");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Sessions",
                table: "Course",
                newName: "Sesstions");

            migrationBuilder.RenameColumn(
                name: "RegistrationClosingDate",
                table: "Course",
                newName: "RegistrationSlosingDate");

            migrationBuilder.RenameColumn(
                name: "Objectives",
                table: "Course",
                newName: "Objecttives");

            migrationBuilder.RenameColumn(
                name: "HoursPerSessions",
                table: "Course",
                newName: "HoursPerSesstions");
        }
    }
}
