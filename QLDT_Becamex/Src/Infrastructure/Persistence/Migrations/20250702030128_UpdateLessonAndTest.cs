using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class UpdateLessonAndTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_AspNetUsers_userId_created",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_AspNetUsers_userId_edited",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Course_course_id",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Tests_test_id",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_AspNetUsers_userId_created",
                table: "Tests");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_AspNetUsers_userId_edited",
                table: "Tests");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Course_course_id",
                table: "Tests");

            migrationBuilder.RenameColumn(
                name: "userId_edited",
                table: "Tests",
                newName: "UserId_edited");

            migrationBuilder.RenameColumn(
                name: "userId_created",
                table: "Tests",
                newName: "UserId_created");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Tests",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "time_test",
                table: "Tests",
                newName: "Time_test");

            migrationBuilder.RenameColumn(
                name: "pass_threshold",
                table: "Tests",
                newName: "Pass_threshold");

            migrationBuilder.RenameColumn(
                name: "course_id",
                table: "Tests",
                newName: "Course_id");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_userId_edited",
                table: "Tests",
                newName: "IX_Tests_UserId_edited");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_userId_created",
                table: "Tests",
                newName: "IX_Tests_UserId_created");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_course_id",
                table: "Tests",
                newName: "IX_Tests_Course_id");

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
                name: "correct_option",
                table: "Questions",
                newName: "Correct_option");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_test_id",
                table: "Questions",
                newName: "IX_Questions_Test_id");

            migrationBuilder.RenameColumn(
                name: "userId_edited",
                table: "Lessons",
                newName: "UserId_edited");

            migrationBuilder.RenameColumn(
                name: "userId_created",
                table: "Lessons",
                newName: "UserId_created");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Lessons",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "course_id",
                table: "Lessons",
                newName: "Course_id");

            migrationBuilder.RenameColumn(
                name: "content_pdf",
                table: "Lessons",
                newName: "Content_pdf");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_userId_edited",
                table: "Lessons",
                newName: "IX_Lessons_UserId_edited");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_userId_created",
                table: "Lessons",
                newName: "IX_Lessons_UserId_created");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_course_id",
                table: "Lessons",
                newName: "IX_Lessons_Course_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_AspNetUsers_UserId_created",
                table: "Lessons",
                column: "UserId_created",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_AspNetUsers_UserId_edited",
                table: "Lessons",
                column: "UserId_edited",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Course_Course_id",
                table: "Lessons",
                column: "Course_id",
                principalTable: "Course",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Tests_Test_id",
                table: "Questions",
                column: "Test_id",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_AspNetUsers_UserId_created",
                table: "Tests",
                column: "UserId_created",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_AspNetUsers_UserId_edited",
                table: "Tests",
                column: "UserId_edited",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Course_Course_id",
                table: "Tests",
                column: "Course_id",
                principalTable: "Course",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_AspNetUsers_UserId_created",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_AspNetUsers_UserId_edited",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_Course_Course_id",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_Tests_Test_id",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_AspNetUsers_UserId_created",
                table: "Tests");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_AspNetUsers_UserId_edited",
                table: "Tests");

            migrationBuilder.DropForeignKey(
                name: "FK_Tests_Course_Course_id",
                table: "Tests");

            migrationBuilder.RenameColumn(
                name: "UserId_edited",
                table: "Tests",
                newName: "userId_edited");

            migrationBuilder.RenameColumn(
                name: "UserId_created",
                table: "Tests",
                newName: "userId_created");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Tests",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Time_test",
                table: "Tests",
                newName: "time_test");

            migrationBuilder.RenameColumn(
                name: "Pass_threshold",
                table: "Tests",
                newName: "pass_threshold");

            migrationBuilder.RenameColumn(
                name: "Course_id",
                table: "Tests",
                newName: "course_id");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_UserId_edited",
                table: "Tests",
                newName: "IX_Tests_userId_edited");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_UserId_created",
                table: "Tests",
                newName: "IX_Tests_userId_created");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_Course_id",
                table: "Tests",
                newName: "IX_Tests_course_id");

            migrationBuilder.RenameColumn(
                name: "Test_id",
                table: "Questions",
                newName: "test_id");

            migrationBuilder.RenameColumn(
                name: "Question_type",
                table: "Questions",
                newName: "question_type");

            migrationBuilder.RenameColumn(
                name: "Question_text",
                table: "Questions",
                newName: "question_text");

            migrationBuilder.RenameColumn(
                name: "Explanation",
                table: "Questions",
                newName: "explanation");

            migrationBuilder.RenameColumn(
                name: "Correct_option",
                table: "Questions",
                newName: "correct_option");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_Test_id",
                table: "Questions",
                newName: "IX_Questions_test_id");

            migrationBuilder.RenameColumn(
                name: "UserId_edited",
                table: "Lessons",
                newName: "userId_edited");

            migrationBuilder.RenameColumn(
                name: "UserId_created",
                table: "Lessons",
                newName: "userId_created");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Lessons",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Course_id",
                table: "Lessons",
                newName: "course_id");

            migrationBuilder.RenameColumn(
                name: "Content_pdf",
                table: "Lessons",
                newName: "content_pdf");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_UserId_edited",
                table: "Lessons",
                newName: "IX_Lessons_userId_edited");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_UserId_created",
                table: "Lessons",
                newName: "IX_Lessons_userId_created");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_Course_id",
                table: "Lessons",
                newName: "IX_Lessons_course_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_AspNetUsers_userId_created",
                table: "Lessons",
                column: "userId_created",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_AspNetUsers_userId_edited",
                table: "Lessons",
                column: "userId_edited",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Course_course_id",
                table: "Lessons",
                column: "course_id",
                principalTable: "Course",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_Tests_test_id",
                table: "Questions",
                column: "test_id",
                principalTable: "Tests",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_AspNetUsers_userId_created",
                table: "Tests",
                column: "userId_created",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_AspNetUsers_userId_edited",
                table: "Tests",
                column: "userId_edited",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tests_Course_course_id",
                table: "Tests",
                column: "course_id",
                principalTable: "Course",
                principalColumn: "Id");
        }
    }
}
