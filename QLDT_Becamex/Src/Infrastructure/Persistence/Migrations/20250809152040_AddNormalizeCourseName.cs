using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class AddNormalizeCourseName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "NormalizeCourseName",
                table: "Courses",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "NormalizeCourseName",
                table: "Courses");
        }
    }
}
