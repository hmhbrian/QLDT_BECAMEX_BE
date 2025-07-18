using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class UpdateApplicationDbContext : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_AspNetUsers_user_id",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Tests_test_id",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_Questions_question_id",
                table: "UserAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_TestResults_test_result_id",
                table: "UserAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAnswers",
                table: "UserAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestResults",
                table: "TestResults");

            migrationBuilder.RenameTable(
                name: "UserAnswers",
                newName: "user_answers");

            migrationBuilder.RenameTable(
                name: "TestResults",
                newName: "test_results");

            migrationBuilder.RenameIndex(
                name: "IX_UserAnswers_test_result_id",
                table: "user_answers",
                newName: "IX_user_answers_test_result_id");

            migrationBuilder.RenameIndex(
                name: "IX_UserAnswers_question_id",
                table: "user_answers",
                newName: "IX_user_answers_question_id");

            migrationBuilder.RenameIndex(
                name: "IX_TestResults_user_id",
                table: "test_results",
                newName: "IX_test_results_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_TestResults_test_id",
                table: "test_results",
                newName: "IX_test_results_test_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_user_answers",
                table: "user_answers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_test_results",
                table: "test_results",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_test_results_AspNetUsers_user_id",
                table: "test_results",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_test_results_Tests_test_id",
                table: "test_results",
                column: "test_id",
                principalTable: "Tests",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_answers_Questions_question_id",
                table: "user_answers",
                column: "question_id",
                principalTable: "Questions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_user_answers_test_results_test_result_id",
                table: "user_answers",
                column: "test_result_id",
                principalTable: "test_results",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_test_results_AspNetUsers_user_id",
                table: "test_results");

            migrationBuilder.DropForeignKey(
                name: "FK_test_results_Tests_test_id",
                table: "test_results");

            migrationBuilder.DropForeignKey(
                name: "FK_user_answers_Questions_question_id",
                table: "user_answers");

            migrationBuilder.DropForeignKey(
                name: "FK_user_answers_test_results_test_result_id",
                table: "user_answers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_user_answers",
                table: "user_answers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_test_results",
                table: "test_results");

            migrationBuilder.RenameTable(
                name: "user_answers",
                newName: "UserAnswers");

            migrationBuilder.RenameTable(
                name: "test_results",
                newName: "TestResults");

            migrationBuilder.RenameIndex(
                name: "IX_user_answers_test_result_id",
                table: "UserAnswers",
                newName: "IX_UserAnswers_test_result_id");

            migrationBuilder.RenameIndex(
                name: "IX_user_answers_question_id",
                table: "UserAnswers",
                newName: "IX_UserAnswers_question_id");

            migrationBuilder.RenameIndex(
                name: "IX_test_results_user_id",
                table: "TestResults",
                newName: "IX_TestResults_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_test_results_test_id",
                table: "TestResults",
                newName: "IX_TestResults_test_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAnswers",
                table: "UserAnswers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestResults",
                table: "TestResults",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_AspNetUsers_user_id",
                table: "TestResults",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_TestResults_Tests_test_id",
                table: "TestResults",
                column: "test_id",
                principalTable: "Tests",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_Questions_question_id",
                table: "UserAnswers",
                column: "question_id",
                principalTable: "Questions",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_UserAnswers_TestResults_test_result_id",
                table: "UserAnswers",
                column: "test_result_id",
                principalTable: "TestResults",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
