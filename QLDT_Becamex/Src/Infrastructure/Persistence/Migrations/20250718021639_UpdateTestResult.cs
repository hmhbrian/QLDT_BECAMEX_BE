using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTestResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "correct_answer_count",
                table: "test_results",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "incorrect_answer_count",
                table: "test_results",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "correct_answer_count",
                table: "test_results");

            migrationBuilder.DropColumn(
                name: "incorrect_answer_count",
                table: "test_results");
        }
    }
}
