using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class RefactorToCourse : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Duration",
                table: "Course",
                newName: "Optional");

            migrationBuilder.AddColumn<int>(
                name: "HoursPerSesstions",
                table: "Course",
                type: "int",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "MaxParticipant",
                table: "Course",
                type: "int",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Sesstions",
                table: "Course",
                type: "int",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ThumbUrl",
                table: "Course",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HoursPerSesstions",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "MaxParticipant",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "Sesstions",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "ThumbUrl",
                table: "Course");

            migrationBuilder.RenameColumn(
                name: "Optional",
                table: "Course",
                newName: "Duration");
        }
    }
}
