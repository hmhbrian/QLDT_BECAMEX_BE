using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class CreateTableCertifiatesAndAddColumnIsDoneToTableTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_test_results_AspNetUsers_user_id",
                table: "test_results");

            migrationBuilder.DropForeignKey(
                name: "FK_test_results_Tests_test_id",
                table: "test_results");

            migrationBuilder.DropForeignKey(
                name: "fk_tests_user_created",
                table: "Tests");

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

            migrationBuilder.AddColumn<bool>(
                name: "is_done",
                table: "TestResults",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserAnswers",
                table: "UserAnswers",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_TestResults",
                table: "TestResults",
                column: "id");

            migrationBuilder.CreateTable(
                name: "Certificates",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    course_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    certificate_url = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Certificates", x => x.id);
                    table.ForeignKey(
                        name: "FK_Certificates_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Certificates_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_course_id",
                table: "Certificates",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Certificates_user_id",
                table: "Certificates",
                column: "user_id");

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
                name: "fk_tests_user_updated",
                table: "Tests",
                column: "created_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_AspNetUsers_user_id",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "FK_TestResults_Tests_test_id",
                table: "TestResults");

            migrationBuilder.DropForeignKey(
                name: "fk_tests_user_updated",
                table: "Tests");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_Questions_question_id",
                table: "UserAnswers");

            migrationBuilder.DropForeignKey(
                name: "FK_UserAnswers_TestResults_test_result_id",
                table: "UserAnswers");

            migrationBuilder.DropTable(
                name: "Certificates");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserAnswers",
                table: "UserAnswers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_TestResults",
                table: "TestResults");

            migrationBuilder.DropColumn(
                name: "is_done",
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
                name: "fk_tests_user_created",
                table: "Tests",
                column: "created_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

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
    }
}
