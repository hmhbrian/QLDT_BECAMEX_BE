using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class RefactorChangeFieldIdInttToStringTestResult : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_audit_logs_AspNetUsers_user_id",
                table: "audit_logs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_audit_logs",
                table: "audit_logs");

            migrationBuilder.RenameTable(
                name: "audit_logs",
                newName: "AuditLogs");

            migrationBuilder.RenameIndex(
                name: "IX_audit_logs_user_id",
                table: "AuditLogs",
                newName: "IX_AuditLogs_user_id");

            //migrationBuilder.AddColumn<int>(
            //    name: "key",
            //    table: "CourseStatus",
            //    type: "int",
            //    maxLength: 255,
            //    nullable: false,
            //    defaultValue: 0);

            //migrationBuilder.AddColumn<bool>(
            //    name: "is_private",
            //    table: "Courses",
            //    type: "bit",
            //    nullable: false,
            //    defaultValue: false);

            migrationBuilder.AddPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs",
                column: "id");

            migrationBuilder.CreateTable(
                name: "TestResults",
                columns: table => new
                {
                    id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    test_id = table.Column<int>(type: "int", nullable: false),
                    user_id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    score = table.Column<float>(type: "real", nullable: false),
                    is_passed = table.Column<bool>(type: "bit", nullable: false),
                    started_at = table.Column<DateTime>(type: "datetime2", nullable: true),
                    submitted_at = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestResults", x => x.id);
                    table.ForeignKey(
                        name: "FK_TestResults_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestResults_Tests_test_id",
                        column: x => x.test_id,
                        principalTable: "Tests",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "UserAnswers",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    test_result_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    question_id = table.Column<int>(type: "int", nullable: false),
                    selected_options = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    is_correct = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserAnswers", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserAnswers_Questions_question_id",
                        column: x => x.question_id,
                        principalTable: "Questions",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserAnswers_TestResults_test_result_id",
                        column: x => x.test_result_id,
                        principalTable: "TestResults",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_test_id",
                table: "TestResults",
                column: "test_id");

            migrationBuilder.CreateIndex(
                name: "IX_TestResults_user_id",
                table: "TestResults",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_question_id",
                table: "UserAnswers",
                column: "question_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserAnswers_test_result_id",
                table: "UserAnswers",
                column: "test_result_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AuditLogs_AspNetUsers_user_id",
                table: "AuditLogs",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AuditLogs_AspNetUsers_user_id",
                table: "AuditLogs");

            migrationBuilder.DropTable(
                name: "UserAnswers");

            migrationBuilder.DropTable(
                name: "TestResults");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AuditLogs",
                table: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "key",
                table: "CourseStatus");

            migrationBuilder.DropColumn(
                name: "is_private",
                table: "Courses");

            migrationBuilder.RenameTable(
                name: "AuditLogs",
                newName: "audit_logs");

            migrationBuilder.RenameIndex(
                name: "IX_AuditLogs_user_id",
                table: "audit_logs",
                newName: "IX_audit_logs_user_id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_audit_logs",
                table: "audit_logs",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "FK_audit_logs_AspNetUsers_user_id",
                table: "audit_logs",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
