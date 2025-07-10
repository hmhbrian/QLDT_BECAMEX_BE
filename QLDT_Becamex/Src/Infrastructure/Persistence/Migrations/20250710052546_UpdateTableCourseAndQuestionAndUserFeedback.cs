using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTableCourseAndQuestionAndUserFeedback : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
           

            migrationBuilder.DropForeignKey(
                name: "FK_Course_CourseCategories_CategoryId",
                table: "Course");

            migrationBuilder.DropForeignKey(
                name: "FK_Course_CourseStatus_StatusId",
                table: "Course");

            migrationBuilder.DropForeignKey(
                name: "FK_Course_Lecturers_LecturerId",
                table: "Course");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseAttachedFile_Course_CourseId",
                table: "CourseAttachedFile");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseDepartment_Course_CourseId",
                table: "CourseDepartment");

            migrationBuilder.DropForeignKey(
                name: "FK_CoursePosition_Course_CourseId",
                table: "CoursePosition");

            migrationBuilder.DropForeignKey(
                name: "fk_lessons_user_created",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "fk_lessons_user_edited",
                table: "Lessons");

            migrationBuilder.DropForeignKey(
                name: "fk_tests_user_created",
                table: "Tests");

            migrationBuilder.DropForeignKey(
                name: "fk_tests_user_edited",
                table: "Tests");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCourse_AspNetUsers_UserId",
                table: "UserCourse");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCourse_Course_CourseId",
                table: "UserCourse");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Course",
                table: "Course");

            migrationBuilder.DropColumn(
                name: "url_pdf",
                table: "Lessons");

            migrationBuilder.RenameTable(
                name: "Course",
                newName: "Courses");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "UserCourse",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UserCourse",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "UserCourse",
                newName: "user_id");

            migrationBuilder.RenameColumn(
                name: "IsMandatory",
                table: "UserCourse",
                newName: "is_mandatory");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "UserCourse",
                newName: "course_id");

            migrationBuilder.RenameColumn(
                name: "AssignedAt",
                table: "UserCourse",
                newName: "assigned_at");

            migrationBuilder.RenameIndex(
                name: "IX_UserCourse_UserId",
                table: "UserCourse",
                newName: "IX_UserCourse_user_id");

            migrationBuilder.RenameIndex(
                name: "IX_UserCourse_CourseId",
                table: "UserCourse",
                newName: "IX_UserCourse_course_id");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "AspNetUsers",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "UrlAvatar",
                table: "AspNetUsers",
                newName: "url_avatar");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "AspNetUsers",
                newName: "status_id");

            migrationBuilder.RenameColumn(
                name: "StartWork",
                table: "AspNetUsers",
                newName: "start_work");

            migrationBuilder.RenameColumn(
                name: "PositionId",
                table: "AspNetUsers",
                newName: "position_id");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "AspNetUsers",
                newName: "modified_at");

            migrationBuilder.RenameColumn(
                name: "ManagerUId",
                table: "AspNetUsers",
                newName: "manager_u_id");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "AspNetUsers",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "IdCard",
                table: "AspNetUsers",
                newName: "id_card");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "AspNetUsers",
                newName: "full_name");

            migrationBuilder.RenameColumn(
                name: "EndWork",
                table: "AspNetUsers",
                newName: "end_work");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "AspNetUsers",
                newName: "department_id");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "AspNetUsers",
                newName: "created_at");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_StatusId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_status_id");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_PositionId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_position_id");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_ManagerUId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_manager_u_id");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_DepartmentId",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_department_id");

            migrationBuilder.RenameColumn(
                name: "Sessions",
                table: "Courses",
                newName: "sessions");

            migrationBuilder.RenameColumn(
                name: "Optional",
                table: "Courses",
                newName: "optional");

            migrationBuilder.RenameColumn(
                name: "Objectives",
                table: "Courses",
                newName: "objectives");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Courses",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Location",
                table: "Courses",
                newName: "location");

            migrationBuilder.RenameColumn(
                name: "Format",
                table: "Courses",
                newName: "format");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Courses",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Code",
                table: "Courses",
                newName: "code");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Courses",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ThumbUrl",
                table: "Courses",
                newName: "thumb_url");

            migrationBuilder.RenameColumn(
                name: "StatusId",
                table: "Courses",
                newName: "status_id");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "Courses",
                newName: "start_date");

            migrationBuilder.RenameColumn(
                name: "RegistrationStartDate",
                table: "Courses",
                newName: "registration_start_date");

            migrationBuilder.RenameColumn(
                name: "RegistrationClosingDate",
                table: "Courses",
                newName: "registration_closing_date");

            migrationBuilder.RenameColumn(
                name: "ModifiedAt",
                table: "Courses",
                newName: "modified_at");

            migrationBuilder.RenameColumn(
                name: "MaxParticipant",
                table: "Courses",
                newName: "max_participant");

            migrationBuilder.RenameColumn(
                name: "LecturerId",
                table: "Courses",
                newName: "lecturer_id");

            migrationBuilder.RenameColumn(
                name: "IsDeleted",
                table: "Courses",
                newName: "is_deleted");

            migrationBuilder.RenameColumn(
                name: "HoursPerSessions",
                table: "Courses",
                newName: "hours_per_sessions");

            migrationBuilder.RenameColumn(
                name: "EndDate",
                table: "Courses",
                newName: "end_date");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Courses",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "Courses",
                newName: "category_id");

            migrationBuilder.RenameIndex(
                name: "IX_Course_StatusId",
                table: "Courses",
                newName: "IX_Courses_status_id");

            migrationBuilder.RenameIndex(
                name: "IX_Course_LecturerId",
                table: "Courses",
                newName: "IX_Courses_lecturer_id");

            migrationBuilder.RenameIndex(
                name: "IX_Course_CategoryId",
                table: "Courses",
                newName: "IX_Courses_category_id");

            migrationBuilder.AddColumn<DateTime>(
                name: "created_at",
                table: "UserCourse",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "modified_at",
                table: "UserCourse",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "created_by_id",
                table: "Questions",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "update_by_id",
                table: "Questions",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "Lessons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "public_id_url_pdf",
                table: "Lessons",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Lessons",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "course_id",
                table: "Lessons",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

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

            migrationBuilder.AlterColumn<string>(
                name: "PublicIdUrlPdf",
                table: "CourseAttachedFile",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Link",
                table: "CourseAttachedFile",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by_id",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "update_by_id",
                table: "AspNetUsers",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "optional",
                table: "Courses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "objectives",
                table: "Courses",
                type: "nvarchar(1000)",
                maxLength: 1000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "format",
                table: "Courses",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "code",
                table: "Courses",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "thumb_url",
                table: "Courses",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "start_date",
                table: "Courses",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "registration_start_date",
                table: "Courses",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "registration_closing_date",
                table: "Courses",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "modified_at",
                table: "Courses",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "end_date",
                table: "Courses",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Courses",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "created_by_id",
                table: "Courses",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "update_by_id",
                table: "Courses",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Courses",
                table: "Courses",
                column: "id");

            migrationBuilder.CreateTable(
                name: "Feedbacks",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    course_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    user_id = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    q1_revelance = table.Column<int>(type: "int", nullable: false),
                    q2_clarity = table.Column<int>(type: "int", nullable: false),
                    q3_structure = table.Column<int>(type: "int", nullable: false),
                    q4_duration = table.Column<int>(type: "int", nullable: false),
                    q5_material = table.Column<int>(type: "int", nullable: false),
                    comment = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    feedback_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Feedbacks", x => x.id);
                    table.ForeignKey(
                        name: "FK_Feedbacks_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Feedbacks_Courses_course_id",
                        column: x => x.course_id,
                        principalTable: "Courses",
                        principalColumn: "id");
                });

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
                name: "IX_Questions_created_by_id",
                table: "Questions",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_update_by_id",
                table: "Questions",
                column: "update_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_type_doc_id",
                table: "Lessons",
                column: "type_doc_id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_created_by_id",
                table: "AspNetUsers",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_update_by_id",
                table: "AspNetUsers",
                column: "update_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_created_by_id",
                table: "Courses",
                column: "created_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_Courses_update_by_id",
                table: "Courses",
                column: "update_by_id");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_course_id",
                table: "Feedbacks",
                column: "course_id");

            migrationBuilder.CreateIndex(
                name: "IX_Feedbacks_user_id",
                table: "Feedbacks",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "IX_LessonProgress_lesson_id",
                table: "LessonProgress",
                column: "lesson_id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_created_by_id",
                table: "AspNetUsers",
                column: "created_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_manager_u_id",
                table: "AspNetUsers",
                column: "manager_u_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_update_by_id",
                table: "AspNetUsers",
                column: "update_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Departments_department_id",
                table: "AspNetUsers",
                column: "department_id",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Positions_position_id",
                table: "AspNetUsers",
                column: "position_id",
                principalTable: "Positions",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserStatus_status_id",
                table: "AspNetUsers",
                column: "status_id",
                principalTable: "UserStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAttachedFile_Courses_CourseId",
                table: "CourseAttachedFile",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseDepartment_Courses_CourseId",
                table: "CourseDepartment",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoursePosition_Courses_CourseId",
                table: "CoursePosition",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_created_by_id",
                table: "Courses",
                column: "created_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_AspNetUsers_update_by_id",
                table: "Courses",
                column: "update_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CourseCategories_category_id",
                table: "Courses",
                column: "category_id",
                principalTable: "CourseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_CourseStatus_status_id",
                table: "Courses",
                column: "status_id",
                principalTable: "CourseStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Courses_Lecturers_lecturer_id",
                table: "Courses",
                column: "lecturer_id",
                principalTable: "Lecturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "fk_lessons_type_document",
                table: "Lessons",
                column: "type_doc_id",
                principalTable: "TypeDocument",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_lessons_user_created",
                table: "Lessons",
                column: "user_id_created",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "fk_lessons_user_edited",
                table: "Lessons",
                column: "user_id_edited",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_AspNetUsers_created_by_id",
                table: "Questions",
                column: "created_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_Questions_AspNetUsers_update_by_id",
                table: "Questions",
                column: "update_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "fk_tests_user_created",
                table: "Tests",
                column: "user_id_created",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "fk_tests_user_edited",
                table: "Tests",
                column: "user_id_edited",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourse_AspNetUsers_user_id",
                table: "UserCourse",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourse_Courses_course_id",
                table: "UserCourse",
                column: "course_id",
                principalTable: "Courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
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
                name: "FK_AspNetUsers_Departments_department_id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_Positions_position_id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_UserStatus_status_id",
                table: "AspNetUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseAttachedFile_Courses_CourseId",
                table: "CourseAttachedFile");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseDepartment_Courses_CourseId",
                table: "CourseDepartment");

            migrationBuilder.DropForeignKey(
                name: "FK_CoursePosition_Courses_CourseId",
                table: "CoursePosition");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_created_by_id",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_AspNetUsers_update_by_id",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CourseCategories_category_id",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_CourseStatus_status_id",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Courses_Lecturers_lecturer_id",
                table: "Courses");

            migrationBuilder.DropForeignKey(
                name: "fk_lessons_type_document",
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

            migrationBuilder.DropForeignKey(
                name: "FK_UserCourse_AspNetUsers_user_id",
                table: "UserCourse");

            migrationBuilder.DropForeignKey(
                name: "FK_UserCourse_Courses_course_id",
                table: "UserCourse");

            migrationBuilder.DropTable(
                name: "Feedbacks");

            migrationBuilder.DropTable(
                name: "LessonProgress");

            migrationBuilder.DropTable(
                name: "TypeDocument");

            migrationBuilder.DropIndex(
                name: "IX_Questions_created_by_id",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Questions_update_by_id",
                table: "Questions");

            migrationBuilder.DropIndex(
                name: "IX_Lessons_type_doc_id",
                table: "Lessons");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_created_by_id",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_update_by_id",
                table: "AspNetUsers");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Courses",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_created_by_id",
                table: "Courses");

            migrationBuilder.DropIndex(
                name: "IX_Courses_update_by_id",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "created_at",
                table: "UserCourse");

            migrationBuilder.DropColumn(
                name: "modified_at",
                table: "UserCourse");

            migrationBuilder.DropColumn(
                name: "created_by_id",
                table: "Questions");

            migrationBuilder.DropColumn(
                name: "update_by_id",
                table: "Questions");

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

            migrationBuilder.DropColumn(
                name: "created_by_id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "update_by_id",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "created_by_id",
                table: "Courses");

            migrationBuilder.DropColumn(
                name: "update_by_id",
                table: "Courses");

            migrationBuilder.RenameTable(
                name: "Courses",
                newName: "Course");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "UserCourse",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UserCourse",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "user_id",
                table: "UserCourse",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "is_mandatory",
                table: "UserCourse",
                newName: "IsMandatory");

            migrationBuilder.RenameColumn(
                name: "course_id",
                table: "UserCourse",
                newName: "CourseId");

            migrationBuilder.RenameColumn(
                name: "assigned_at",
                table: "UserCourse",
                newName: "AssignedAt");

            migrationBuilder.RenameIndex(
                name: "IX_UserCourse_user_id",
                table: "UserCourse",
                newName: "IX_UserCourse_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_UserCourse_course_id",
                table: "UserCourse",
                newName: "IX_UserCourse_CourseId");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "AspNetUsers",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "url_avatar",
                table: "AspNetUsers",
                newName: "UrlAvatar");

            migrationBuilder.RenameColumn(
                name: "status_id",
                table: "AspNetUsers",
                newName: "StatusId");

            migrationBuilder.RenameColumn(
                name: "start_work",
                table: "AspNetUsers",
                newName: "StartWork");

            migrationBuilder.RenameColumn(
                name: "position_id",
                table: "AspNetUsers",
                newName: "PositionId");

            migrationBuilder.RenameColumn(
                name: "modified_at",
                table: "AspNetUsers",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "manager_u_id",
                table: "AspNetUsers",
                newName: "ManagerUId");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "AspNetUsers",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "id_card",
                table: "AspNetUsers",
                newName: "IdCard");

            migrationBuilder.RenameColumn(
                name: "full_name",
                table: "AspNetUsers",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "end_work",
                table: "AspNetUsers",
                newName: "EndWork");

            migrationBuilder.RenameColumn(
                name: "department_id",
                table: "AspNetUsers",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "AspNetUsers",
                newName: "CreatedAt");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_status_id",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_position_id",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_manager_u_id",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_ManagerUId");

            migrationBuilder.RenameIndex(
                name: "IX_AspNetUsers_department_id",
                table: "AspNetUsers",
                newName: "IX_AspNetUsers_DepartmentId");

            migrationBuilder.RenameColumn(
                name: "sessions",
                table: "Course",
                newName: "Sessions");

            migrationBuilder.RenameColumn(
                name: "optional",
                table: "Course",
                newName: "Optional");

            migrationBuilder.RenameColumn(
                name: "objectives",
                table: "Course",
                newName: "Objectives");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "Course",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "location",
                table: "Course",
                newName: "Location");

            migrationBuilder.RenameColumn(
                name: "format",
                table: "Course",
                newName: "Format");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Course",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "code",
                table: "Course",
                newName: "Code");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Course",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "thumb_url",
                table: "Course",
                newName: "ThumbUrl");

            migrationBuilder.RenameColumn(
                name: "status_id",
                table: "Course",
                newName: "StatusId");

            migrationBuilder.RenameColumn(
                name: "start_date",
                table: "Course",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "registration_start_date",
                table: "Course",
                newName: "RegistrationStartDate");

            migrationBuilder.RenameColumn(
                name: "registration_closing_date",
                table: "Course",
                newName: "RegistrationClosingDate");

            migrationBuilder.RenameColumn(
                name: "modified_at",
                table: "Course",
                newName: "ModifiedAt");

            migrationBuilder.RenameColumn(
                name: "max_participant",
                table: "Course",
                newName: "MaxParticipant");

            migrationBuilder.RenameColumn(
                name: "lecturer_id",
                table: "Course",
                newName: "LecturerId");

            migrationBuilder.RenameColumn(
                name: "is_deleted",
                table: "Course",
                newName: "IsDeleted");

            migrationBuilder.RenameColumn(
                name: "hours_per_sessions",
                table: "Course",
                newName: "HoursPerSessions");

            migrationBuilder.RenameColumn(
                name: "end_date",
                table: "Course",
                newName: "EndDate");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "Course",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "category_id",
                table: "Course",
                newName: "CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_status_id",
                table: "Course",
                newName: "IX_Course_StatusId");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_lecturer_id",
                table: "Course",
                newName: "IX_Course_LecturerId");

            migrationBuilder.RenameIndex(
                name: "IX_Courses_category_id",
                table: "Course",
                newName: "IX_Course_CategoryId");

            migrationBuilder.AlterColumn<DateTime>(
                name: "updated_at",
                table: "Lessons",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "public_id_url_pdf",
                table: "Lessons",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "created_at",
                table: "Lessons",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "course_id",
                table: "Lessons",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "url_pdf",
                table: "Lessons",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "PublicIdUrlPdf",
                table: "CourseAttachedFile",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Link",
                table: "CourseAttachedFile",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldMaxLength: 450,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Optional",
                table: "Course",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Objectives",
                table: "Course",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(1000)",
                oldMaxLength: 1000,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Format",
                table: "Course",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Code",
                table: "Course",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);

            migrationBuilder.AlterColumn<string>(
                name: "ThumbUrl",
                table: "Course",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "Course",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RegistrationStartDate",
                table: "Course",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "RegistrationClosingDate",
                table: "Course",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "ModifiedAt",
                table: "Course",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "EndDate",
                table: "Course",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "CreatedAt",
                table: "Course",
                type: "datetime",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Course",
                table: "Course",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_AspNetUsers_ManagerUId",
                table: "AspNetUsers",
                column: "ManagerUId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Departments_DepartmentId",
                table: "AspNetUsers",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_Positions_PositionId",
                table: "AspNetUsers",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_UserStatus_StatusId",
                table: "AspNetUsers",
                column: "StatusId",
                principalTable: "UserStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Course_CourseCategories_CategoryId",
                table: "Course",
                column: "CategoryId",
                principalTable: "CourseCategories",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Course_CourseStatus_StatusId",
                table: "Course",
                column: "StatusId",
                principalTable: "CourseStatus",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Course_Lecturers_LecturerId",
                table: "Course",
                column: "LecturerId",
                principalTable: "Lecturers",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAttachedFile_Course_CourseId",
                table: "CourseAttachedFile",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseDepartment_Course_CourseId",
                table: "CourseDepartment",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoursePosition_Course_CourseId",
                table: "CoursePosition",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "fk_tests_user_created",
                table: "Tests",
                column: "user_id_created",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "fk_tests_user_edited",
                table: "Tests",
                column: "user_id_edited",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourse_AspNetUsers_UserId",
                table: "UserCourse",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserCourse_Course_CourseId",
                table: "UserCourse",
                column: "CourseId",
                principalTable: "Course",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
