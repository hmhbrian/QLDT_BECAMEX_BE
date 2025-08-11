using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class AddNormalizedFullNameAndIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "normalized_full_name",
                table: "AspNetUsers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Course_Code",
                table: "Courses",
                column: "code");

            migrationBuilder.CreateIndex(
                name: "IX_Course_NormalizedCourseName",
                table: "Courses",
                column: "NormalizeCourseName");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_Email",
                table: "AspNetUsers",
                column: "Email");

            migrationBuilder.CreateIndex(
                name: "IX_ApplicationUser_NormalizedFullName",
                table: "AspNetUsers",
                column: "normalized_full_name");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Course_Code",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Course_NormalizedCourseName",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUser_Email",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_ApplicationUser_NormalizedFullName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "normalized_full_name",
                table: "AspNetUsers");
        }
    }
}
