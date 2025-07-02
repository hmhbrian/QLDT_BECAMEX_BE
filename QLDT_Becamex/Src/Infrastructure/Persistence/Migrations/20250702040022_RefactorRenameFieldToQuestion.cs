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
                name: "FK_Lessons_Courses",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_UserCreated",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Lessons_UserEdited",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "FK_Questions_tests_Test_id",
                table: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tests",
                table: "tests");

            migrationBuilder.RenameTable(
                name: "tests",
                newName: "Tests");

            migrationBuilder.RenameIndex(
                name: "IX_tests_user_id_edited",
                table: "Tests",
                newName: "IX_Tests_user_id_edited");

            migrationBuilder.RenameIndex(
                name: "IX_tests_user_id_created",
                table: "Tests",
                newName: "IX_Tests_user_id_created");

            migrationBuilder.RenameIndex(
                name: "IX_tests_course_id",
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
                name: "D",
                table: "Questions",
                newName: "d");

            migrationBuilder.RenameColumn(
                name: "Correct_option",
                table: "Questions",
                newName: "correct_option");

            migrationBuilder.RenameColumn(
                name: "C",
                table: "Questions",
                newName: "c");

            migrationBuilder.RenameColumn(
                name: "B",
                table: "Questions",
                newName: "b");

            migrationBuilder.RenameColumn(
                name: "A",
                table: "Questions",
                newName: "a");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Questions",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Questions",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Questions",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_Questions_Test_id",
                table: "Questions",
                newName: "IX_Questions_test_id");

            migrationBuilder.RenameColumn(
                name: "UserIdEdited",
                table: "Lessons",
                newName: "user_id_edited");

            migrationBuilder.RenameColumn(
                name: "UserIdCreated",
                table: "Lessons",
                newName: "user_id_created");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "Lessons",
                newName: "course_id");

            migrationBuilder.RenameColumn(
                name: "order",
                table: "Lessons",
                newName: "position");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_UserIdEdited",
                table: "Lessons",
                newName: "IX_Lessons_user_id_edited");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_UserIdCreated",
                table: "Lessons",
                newName: "IX_Lessons_user_id_created");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_CourseId",
                table: "Lessons",
                newName: "IX_Lessons_course_id");

            migrationBuilder.AlterColumn<int>(
                name: "test_id",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "question_type",
                table: "Questions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "question_text",
                table: "Questions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "explanation",
                table: "Questions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "d",
                table: "Questions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "correct_option",
                table: "Questions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "c",
                table: "Questions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "b",
                table: "Questions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<string>(
                name: "a",
                table: "Questions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tests",
                table: "Tests",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_lessons_courses",
                table: "Lessons",
                column: "course_id",
                principalTable: "Course",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "fk_lessons_user_created",
                table: "Lessons",
                column: "user_id_created",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "fk_lessons_user_edited",
                table: "Lessons",
                column: "user_id_edited",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "fk_questions_tests",
                table: "Questions",
                column: "test_id",
                principalTable: "Tests",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
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
                name: "fk_questions_tests",
                table: "Questions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tests",
                table: "Tests");

            migrationBuilder.RenameTable(
                name: "Tests",
                newName: "tests");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_user_id_edited",
                table: "tests",
                newName: "IX_tests_user_id_edited");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_user_id_created",
                table: "tests",
                newName: "IX_tests_user_id_created");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_course_id",
                table: "tests",
                newName: "IX_tests_course_id");

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
                name: "correct_option",
                table: "Questions",
                newName: "Correct_option");

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

            migrationBuilder.RenameIndex(
                name: "IX_Questions_test_id",
                table: "Questions",
                newName: "IX_Questions_Test_id");

            migrationBuilder.RenameColumn(
                name: "user_id_edited",
                table: "Lessons",
                newName: "UserIdEdited");

            migrationBuilder.RenameColumn(
                name: "user_id_created",
                table: "Lessons",
                newName: "UserIdCreated");

            migrationBuilder.RenameColumn(
                name: "course_id",
                table: "Lessons",
                newName: "CourseId");

            migrationBuilder.RenameColumn(
                name: "position",
                table: "Lessons",
                newName: "order");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_user_id_edited",
                table: "Lessons",
                newName: "IX_Lessons_UserIdEdited");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_user_id_created",
                table: "Lessons",
                newName: "IX_Lessons_UserIdCreated");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_course_id",
                table: "Lessons",
                newName: "IX_Lessons_CourseId");

            migrationBuilder.AlterColumn<int>(
                name: "Test_id",
                table: "Questions",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "Question_type",
                table: "Questions",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Question_text",
                table: "Questions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Explanation",
                table: "Questions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "D",
                table: "Questions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Correct_option",
                table: "Questions",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "C",
                table: "Questions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "B",
                table: "Questions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "A",
                table: "Questions",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_tests",
                table: "tests",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_Courses",
                table: "Lessons",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_UserCreated",
                table: "Lessons",
                column: "UserIdCreated",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Lessons_UserEdited",
                table: "Lessons",
                column: "UserIdEdited",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

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
