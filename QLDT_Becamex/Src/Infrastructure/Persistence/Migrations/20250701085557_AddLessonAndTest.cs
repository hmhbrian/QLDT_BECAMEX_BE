using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class AddLessonAndTest : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    content_pdf = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    order = table.Column<int>(type: "int", nullable: false),
                    userId_created = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    userId_edited = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lessons_AspNetUsers_userId_created",
                        column: x => x.userId_created,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Lessons_AspNetUsers_userId_edited",
                        column: x => x.userId_edited,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Lessons_Course_course_id",
                        column: x => x.course_id,
                        principalTable: "Course",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    pass_threshold = table.Column<float>(type: "real", nullable: false),
                    time_test = table.Column<int>(type: "int", nullable: false),
                    userId_created = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    userId_edited = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tests_AspNetUsers_userId_created",
                        column: x => x.userId_created,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tests_AspNetUsers_userId_edited",
                        column: x => x.userId_edited,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tests_Course_course_id",
                        column: x => x.course_id,
                        principalTable: "Course",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    test_id = table.Column<int>(type: "int", nullable: true),
                    question_text = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    correct_option = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    question_type = table.Column<int>(type: "int", nullable: false),
                    explanation = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    A = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    B = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    C = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    D = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_Tests_test_id",
                        column: x => x.test_id,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_course_id",
                table: "Lessons",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_userId_created",
                table: "Lessons",
                column: "userId_created");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_userId_edited",
                table: "Lessons",
                column: "userId_edited");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_test_id",
                table: "Questions",
                column: "test_id");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_course_id",
                table: "Tests",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_userId_created",
                table: "Tests",
                column: "userId_created");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_userId_edited",
                table: "Tests",
                column: "userId_edited");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "Tests");
        }
    }
}
