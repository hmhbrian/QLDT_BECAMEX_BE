using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableCourseAndQuestionAndUserFeedbackV2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_created_by_id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_manager_u_id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_update_by_id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_created_by_id",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_update_by_id",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "fk_lessons_user_created",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "fk_lessons_user_edited",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_AspNetUsers_created_by_id",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_AspNetUsers_update_by_id",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "fk_tests_user_created",
                table: "Tests");

            migrationBuilder.DropForeignKey(
                name: "fk_tests_user_edited",
                table: "Tests");

            migrationBuilder.RenameColumn(
                name: "user_id_edited",
                table: "Tests",
                newName: "updated_by_id");

            migrationBuilder.RenameColumn(
                name: "user_id_created",
                table: "Tests",
                newName: "created_by_id");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_user_id_edited",
                table: "Tests",
                newName: "IX_Tests_updated_by_id");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_user_id_created",
                table: "Tests",
                newName: "IX_Tests_created_by_id");

            migrationBuilder.RenameColumn(
                name: "user_id_edited",
                table: "Lessons",
                newName: "updated_by_id");

            migrationBuilder.RenameColumn(
                name: "user_id_created",
                table: "Lessons",
                newName: "created_by_id");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_user_id_edited",
                table: "Lessons",
                newName: "IX_Lessons_updated_by_id");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_user_id_created",
                table: "Lessons",
                newName: "IX_Lessons_created_by_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_created_by_id",
                table: "AspNetUsers",
                column: "created_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_manager_u_id",
                table: "AspNetUsers",
                column: "manager_u_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_update_by_id",
                table: "AspNetUsers",
                column: "update_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_created_by_id",
                table: "Courses",
                column: "created_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_update_by_id",
                table: "Courses",
                column: "update_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "fk_lessons_courses",
                table: "Lessons",
                column: "course_id",
                principalTable: "Courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_lessons_user_created",
                table: "Lessons",
                column: "created_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "fk_lessons_user_edited",
                table: "Lessons",
                column: "updated_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_AspNetUsers_created_by_id",
                table: "Questions",
                column: "created_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_AspNetUsers_update_by_id",
                table: "Questions",
                column: "update_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "fk_tests_user_created",
                table: "Tests",
                column: "created_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "fk_tests_user_edited",
                table: "Tests",
                column: "updated_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_created_by_id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_manager_u_id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_update_by_id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_created_by_id",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_update_by_id",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "fk_lessons_courses",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "fk_lessons_user_created",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "fk_lessons_user_edited",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_AspNetUsers_created_by_id",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_AspNetUsers_update_by_id",
                table: "Questions");

            migrationBuilder.DropForeignKey(
                name: "fk_tests_user_created",
                table: "Tests");

            migrationBuilder.DropForeignKey(
                name: "fk_tests_user_edited",
                table: "Tests");

            migrationBuilder.RenameColumn(
                name: "updated_by_id",
                table: "Tests",
                newName: "user_id_edited");

            migrationBuilder.RenameColumn(
                name: "created_by_id",
                table: "Tests",
                newName: "user_id_created");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_updated_by_id",
                table: "Tests",
                newName: "IX_Tests_user_id_edited");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_created_by_id",
                table: "Tests",
                newName: "IX_Tests_user_id_created");

            migrationBuilder.RenameColumn(
                name: "updated_by_id",
                table: "Lessons",
                newName: "user_id_edited");

            migrationBuilder.RenameColumn(
                name: "created_by_id",
                table: "Lessons",
                newName: "user_id_created");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_updated_by_id",
                table: "Lessons",
                newName: "IX_Lessons_user_id_edited");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_created_by_id",
                table: "Lessons",
                newName: "IX_Lessons_user_id_created");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_created_by_id",
                table: "AspNetUsers",
                column: "created_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_manager_u_id",
                table: "AspNetUsers",
                column: "manager_u_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_update_by_id",
                table: "AspNetUsers",
                column: "update_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_created_by_id",
                table: "Courses",
                column: "created_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_update_by_id",
                table: "Courses",
                column: "update_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_lessons_courses",
                table: "Lessons",
                column: "course_id",
                principalTable: "Courses",
                principalColumn: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_lessons_user_created",
                table: "Lessons",
                column: "user_id_created",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_lessons_user_edited",
                table: "Lessons",
                column: "user_id_edited",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_AspNetUsers_created_by_id",
                table: "Questions",
                column: "created_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_AspNetUsers_update_by_id",
                table: "Questions",
                column: "update_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_tests_user_created",
                table: "Tests",
                column: "user_id_created",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_tests_user_edited",
                table: "Tests",
                column: "user_id_edited",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
