using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class RefactorRenameFieldToQuestion : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Questions_tests_Test_id",
                table: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Questions",
                table: "Questions");

            migrationBuilder.RenameTable(
                name: "Questions",
                newName: "questions");

            migrationBuilder.RenameColumn(
                name: "Test_id",
                table: "questions",
                newName: "test_id");

            migrationBuilder.RenameColumn(
                name: "Question_type",
                table: "questions",
                newName: "question_type");

            migrationBuilder.RenameColumn(
                name: "Question_text",
                table: "questions",
                newName: "question_text");

            migrationBuilder.RenameColumn(
                name: "Explanation",
                table: "questions",
                newName: "explanation");

            migrationBuilder.RenameColumn(
                name: "D",
                table: "questions",
                newName: "d");

            migrationBuilder.RenameColumn(
                name: "C",
                table: "questions",
                newName: "c");

            migrationBuilder.RenameColumn(
                name: "B",
                table: "questions",
                newName: "b");

            migrationBuilder.RenameColumn(
                name: "A",
                table: "questions",
                newName: "a");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "questions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "questions",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "questions",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "Correct_option",
                table: "questions",
                newName: "CorrectOption");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_Test_id",
                table: "questions",
                newName: "IX_questions_test_id");

            migrationBuilder.AlterColumn<int>(
                name: "question_type",
                table: "questions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddPrimaryKey(
                name: "PK_questions",
                table: "questions",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_questions_tests",
                table: "questions",
                column: "test_id",
                principalTable: "tests",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_questions_tests",
                table: "questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_questions",
                table: "questions");

            migrationBuilder.RenameTable(
                name: "questions",
                newName: "Questions");

            migrationBuilder.RenameColumn(
                name: "test_id",
                table: "Questions",
                newName: "Test_id");

            migrationBuilder.RenameColumn(
                name: "question_type",
                table: "Questions",
                newName: "Question_type");

            migrationBuilder.RenameColumn(
                name: "question_text",
                table: "Questions",
                newName: "Question_text");

            migrationBuilder.RenameColumn(
                name: "explanation",
                table: "Questions",
                newName: "Explanation");

            migrationBuilder.RenameColumn(
                name: "d",
                table: "Questions",
                newName: "D");

            migrationBuilder.RenameColumn(
                name: "c",
                table: "Questions",
                newName: "C");

            migrationBuilder.RenameColumn(
                name: "b",
                table: "Questions",
                newName: "B");

            migrationBuilder.RenameColumn(
                name: "a",
                table: "Questions",
                newName: "A");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Questions",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Questions",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Questions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "CorrectOption",
                table: "Questions",
                newName: "Correct_option");

            migrationBuilder.RenameIndex(
                name: "IX_questions_test_id",
                table: "Questions",
                newName: "IX_Questions_Test_id");

            migrationBuilder.AlterColumn<int>(
                name: "Question_type",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Questions",
                table: "Questions",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_tests_Test_id",
                table: "Questions",
                column: "Test_id",
                principalTable: "tests",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
