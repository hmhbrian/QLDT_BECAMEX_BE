using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonProgressAndTypeDoc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "url_pdf",
                table: "Lessons");

            migrationBuilder.AddColumn<string>(
                name: "file_url",
                table: "Lessons",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "total_duration_seconds",
                table: "Lessons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "total_pages",
                table: "Lessons",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "type_doc_id",
                table: "Lessons",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "LessonProgress",
                columns: table => new
                {
                    user_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    lesson_id = table.Column<int>(type: "int", nullable: false),
                    is_completed = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    current_time_seconds = table.Column<int>(type: "int", nullable: true),
                    current_page = table.Column<int>(type: "int", nullable: true),
                    last_accessed = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonProgress", x => new { x.user_id, x.lesson_id });
                    table.CheckConstraint("CK_Progress_Type", "(current_time_seconds IS NOT NULL AND current_page IS NULL) OR (current_time_seconds IS NULL AND current_page IS NOT NULL) OR (current_time_seconds IS NULL AND current_page IS NULL)");
                    table.ForeignKey(
                        name: "fk_student_lesson_progress_lesson",
                        column: x => x.lesson_id,
                        principalTable: "Lessons",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_student_lesson_progress_user",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TypeDocument",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name_type = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETDATE()")
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TypeDocument", x => x.id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_type_doc_id",
                table: "Lessons",
                column: "type_doc_id");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Lesson_Metadata",
                table: "Lessons",
                sql: "(type_doc_id = 2 AND total_duration_seconds IS NOT NULL AND total_pages IS NULL) OR (type_doc_id = 1 AND total_pages IS NOT NULL AND total_duration_seconds IS NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgress_lesson_id",
                table: "LessonProgress",
                column: "lesson_id");

            migrationBuilder.AddForeignKey(
                name: "fk_lessons_type_document",
                table: "Lessons",
                column: "type_doc_id",
                principalTable: "TypeDocument",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_lessons_type_document",
                table: "Lessons");

            migrationBuilder.DropTable(
                name: "LessonProgress");

            migrationBuilder.DropTable(
                name: "TypeDocument");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_type_doc_id",
                table: "Lessons");

            migrationBuilder.DropCheckConstraint(
                name: "CK_Lesson_Metadata",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "file_url",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "total_duration_seconds",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "total_pages",
                table: "Lessons");

            migrationBuilder.DropColumn(
                name: "type_doc_id",
                table: "Lessons");

            migrationBuilder.AddColumn<string>(
                name: "url_pdf",
                table: "Lessons",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");
        }
    }
}
