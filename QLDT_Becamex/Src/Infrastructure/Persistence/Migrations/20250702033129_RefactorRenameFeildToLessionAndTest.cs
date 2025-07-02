using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class RefactorRenameFeildToLessionAndTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tests",
                table: "Tests");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_Course_id",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "Course_id",
                table: "Lessons");

            migrationBuilder.RenameTable(
                name: "Tests",
                newName: "tests");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "tests",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Time_test",
                table: "tests",
                newName: "time_test");

            migrationBuilder.RenameColumn(
                name: "Pass_threshold",
                table: "tests",
                newName: "pass_threshold");

            migrationBuilder.RenameColumn(
                name: "Course_id",
                table: "tests",
                newName: "course_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tests",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "tests",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "tests",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "UserId_edited",
                table: "tests",
                newName: "user_id_edited");

            migrationBuilder.RenameColumn(
                name: "UserId_created",
                table: "tests",
                newName: "user_id_created");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_Course_id",
                table: "tests",
                newName: "IX_tests_course_id");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_UserId_edited",
                table: "tests",
                newName: "IX_tests_user_id_edited");

            migrationBuilder.RenameIndex(
                name: "IX_Tests_UserId_created",
                table: "tests",
                newName: "IX_tests_user_id_created");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Lessons",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Order",
                table: "Lessons",
                newName: "order");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Lessons",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Lessons",
                newName: "updated_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Lessons",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "UserId_edited",
                table: "Lessons",
                newName: "UserIdEdited");

            migrationBuilder.RenameColumn(
                name: "UserId_created",
                table: "Lessons",
                newName: "UserIdCreated");

            migrationBuilder.RenameColumn(
                name: "Content_pdf",
                table: "Lessons",
                newName: "url_pdf");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_UserId_edited",
                table: "Lessons",
                newName: "IX_Lessons_UserIdEdited");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_UserId_created",
                table: "Lessons",
                newName: "IX_Lessons_UserIdCreated");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "Lessons",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Lessons",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "CourseId",
                table: "Lessons",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_tests",
                table: "tests",
                column: "id");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_CourseId",
                table: "Lessons",
                column: "CourseId");

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

            migrationBuilder.AddForeignKey(
                name: "fk_tests_courses",
                table: "tests",
                column: "course_id",
                principalTable: "Course",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "fk_tests_user_created",
                table: "tests",
                column: "user_id_created",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "fk_tests_user_edited",
                table: "tests",
                column: "user_id_edited",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.DropForeignKey(
                name: "fk_tests_courses",
                table: "tests");

            migrationBuilder.DropForeignKey(
                name: "fk_tests_user_created",
                table: "tests");

            migrationBuilder.DropForeignKey(
                name: "fk_tests_user_edited",
                table: "tests");

            migrationBuilder.DropPrimaryKey(
                name: "PK_tests",
                table: "tests");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_CourseId",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "CourseId",
                table: "Lessons");

            migrationBuilder.RenameTable(
                name: "tests",
                newName: "Tests");

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

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Tests",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Tests",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Tests",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "user_id_edited",
                table: "Tests",
                newName: "UserId_edited");

            migrationBuilder.RenameColumn(
                name: "user_id_created",
                table: "Tests",
                newName: "UserId_created");

            migrationBuilder.RenameIndex(
                name: "IX_tests_course_id",
                table: "Tests",
                newName: "IX_Tests_Course_id");

            migrationBuilder.RenameIndex(
                name: "IX_tests_user_id_edited",
                table: "Tests",
                newName: "IX_Tests_UserId_edited");

            migrationBuilder.RenameIndex(
                name: "IX_tests_user_id_created",
                table: "Tests",
                newName: "IX_Tests_UserId_created");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "Lessons",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "order",
                table: "Lessons",
                newName: "Order");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Lessons",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "updated_at",
                table: "Lessons",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Lessons",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "url_pdf",
                table: "Lessons",
                newName: "Content_pdf");

            migrationBuilder.RenameColumn(
                name: "UserIdEdited",
                table: "Lessons",
                newName: "UserId_edited");

            migrationBuilder.RenameColumn(
                name: "UserIdCreated",
                table: "Lessons",
                newName: "UserId_created");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_UserIdEdited",
                table: "Lessons",
                newName: "IX_Lessons_UserId_edited");

            migrationBuilder.RenameIndex(
                name: "IX_Lessons_UserIdCreated",
                table: "Lessons",
                newName: "IX_Lessons_UserId_created");

            migrationBuilder.AlterColumn<DateTime>(
                name: "UpdatedAt",
                table: "Lessons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Lessons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Course_id",
                table: "Lessons",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tests",
                table: "Tests",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_Course_id",
                table: "Lessons",
                column: "Course_id");

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
    }
}
