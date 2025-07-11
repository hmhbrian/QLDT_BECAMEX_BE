using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseAttachedFile_AspNetUsers_UserId",
                table: "CourseAttachedFile");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseAttachedFile_Courses_CourseId",
                table: "CourseAttachedFile");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseDepartment_Courses_CourseId",
                table: "CourseDepartment");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseDepartment_Departments_DepartmentId",
                table: "CourseDepartment");

            migrationBuilder.DropForeignKey(
                name: "FK_CoursePosition_Courses_CourseId",
                table: "CoursePosition");

            migrationBuilder.DropForeignKey(
                name: "FK_CoursePosition_Positions_PositionId",
                table: "CoursePosition");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_AspNetUsers_ManagerId",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_ManagerId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "CourseAttachedFile");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "UserStatus",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "UserStatus",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PositionName",
                table: "Positions",
                newName: "position_name");

            migrationBuilder.RenameColumn(
                name: "PositionId",
                table: "Positions",
                newName: "position_id");

            migrationBuilder.RenameColumn(
                name: "Email",
                table: "Lecturers",
                newName: "email");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Lecturers",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "ProfileImageUrl",
                table: "Lecturers",
                newName: "profile_image_url");

            migrationBuilder.RenameColumn(
                name: "PhoneNumber",
                table: "Lecturers",
                newName: "phone_number");

            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "Lecturers",
                newName: "full_name");

            migrationBuilder.RenameColumn(
                name: "q1_revelance",
                table: "Feedbacks",
                newName: "q1_relevance");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Departments",
                newName: "status");

            migrationBuilder.RenameColumn(
                name: "Level",
                table: "Departments",
                newName: "level");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Departments",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Departments",
                newName: "update_at");

            migrationBuilder.RenameColumn(
                name: "ManagerId",
                table: "Departments",
                newName: "manager_id");

            migrationBuilder.RenameColumn(
                name: "DepartmentName",
                table: "Departments",
                newName: "department_name");

            migrationBuilder.RenameColumn(
                name: "DepartmentCode",
                table: "Departments",
                newName: "department_code");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Departments",
                newName: "create_at");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "Departments",
                newName: "department_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CourseStatus",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CourseStatus",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CoursePosition",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "PositionId",
                table: "CoursePosition",
                newName: "position_id");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "CoursePosition",
                newName: "course_id");

            migrationBuilder.RenameIndex(
                name: "IX_CoursePosition_PositionId",
                table: "CoursePosition",
                newName: "IX_CoursePosition_position_id");

            migrationBuilder.RenameIndex(
                name: "IX_CoursePosition_CourseId",
                table: "CoursePosition",
                newName: "IX_CoursePosition_course_id");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CourseDepartment",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "DepartmentId",
                table: "CourseDepartment",
                newName: "department_id");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "CourseDepartment",
                newName: "course_id");

            migrationBuilder.RenameIndex(
                name: "IX_CourseDepartment_DepartmentId",
                table: "CourseDepartment",
                newName: "IX_CourseDepartment_department_id");

            migrationBuilder.RenameIndex(
                name: "IX_CourseDepartment_CourseId",
                table: "CourseDepartment",
                newName: "IX_CourseDepartment_course_id");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "CourseCategories",
                newName: "name");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "CourseCategories",
                newName: "description");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CourseCategories",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "CourseAttachedFile",
                newName: "title");

            migrationBuilder.RenameColumn(
                name: "Link",
                table: "CourseAttachedFile",
                newName: "link");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "CourseAttachedFile",
                newName: "id");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "CourseAttachedFile",
                newName: "created_by_id");

            migrationBuilder.RenameColumn(
                name: "PublicIdUrlPdf",
                table: "CourseAttachedFile",
                newName: "public_id_url_pdf");

            migrationBuilder.RenameColumn(
                name: "ModifiedTime",
                table: "CourseAttachedFile",
                newName: "modified_at");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "CourseAttachedFile",
                newName: "created_at");

            migrationBuilder.RenameColumn(
                name: "CourseId",
                table: "CourseAttachedFile",
                newName: "course_id");

            migrationBuilder.RenameIndex(
                name: "IX_CourseAttachedFile_UserId",
                table: "CourseAttachedFile",
                newName: "IX_CourseAttachedFile_created_by_id");

            migrationBuilder.RenameIndex(
                name: "IX_CourseAttachedFile_CourseId",
                table: "CourseAttachedFile",
                newName: "IX_CourseAttachedFile_course_id");

            migrationBuilder.AddColumn<int>(
                name: "type_doc_id",
                table: "CourseAttachedFile",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Departments_manager_id",
                table: "Departments",
                column: "manager_id",
                unique: true,
                filter: "[manager_id] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_CourseAttachedFile_type_doc_id",
                table: "CourseAttachedFile",
                column: "type_doc_id");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAttachedFile_AspNetUsers_created_by_id",
                table: "CourseAttachedFile",
                column: "created_by_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAttachedFile_Courses_course_id",
                table: "CourseAttachedFile",
                column: "course_id",
                principalTable: "Courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_AttachedFile_type_document",
                table: "CourseAttachedFile",
                column: "type_doc_id",
                principalTable: "TypeDocument",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseDepartment_Courses_course_id",
                table: "CourseDepartment",
                column: "course_id",
                principalTable: "Courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CourseDepartment_Departments_department_id",
                table: "CourseDepartment",
                column: "department_id",
                principalTable: "Departments",
                principalColumn: "department_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoursePosition_Courses_course_id",
                table: "CoursePosition",
                column: "course_id",
                principalTable: "Courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoursePosition_Positions_position_id",
                table: "CoursePosition",
                column: "position_id",
                principalTable: "Positions",
                principalColumn: "position_id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_AspNetUsers_manager_id",
                table: "Departments",
                column: "manager_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_CourseAttachedFile_AspNetUsers_created_by_id",
                table: "CourseAttachedFile");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseAttachedFile_Courses_course_id",
                table: "CourseAttachedFile");

            migrationBuilder.DropForeignKey(
                name: "fk_AttachedFile_type_document",
                table: "CourseAttachedFile");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseDepartment_Courses_course_id",
                table: "CourseDepartment");

            migrationBuilder.DropForeignKey(
                name: "FK_CourseDepartment_Departments_department_id",
                table: "CourseDepartment");

            migrationBuilder.DropForeignKey(
                name: "FK_CoursePosition_Courses_course_id",
                table: "CoursePosition");

            migrationBuilder.DropForeignKey(
                name: "FK_CoursePosition_Positions_position_id",
                table: "CoursePosition");

            migrationBuilder.DropForeignKey(
                name: "FK_Departments_AspNetUsers_manager_id",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_Departments_manager_id",
                table: "Departments");

            migrationBuilder.DropIndex(
                name: "IX_CourseAttachedFile_type_doc_id",
                table: "CourseAttachedFile");

            migrationBuilder.DropColumn(
                name: "type_doc_id",
                table: "CourseAttachedFile");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "UserStatus",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "UserStatus",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "position_name",
                table: "Positions",
                newName: "PositionName");

            migrationBuilder.RenameColumn(
                name: "position_id",
                table: "Positions",
                newName: "PositionId");

            migrationBuilder.RenameColumn(
                name: "email",
                table: "Lecturers",
                newName: "Email");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "Lecturers",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "profile_image_url",
                table: "Lecturers",
                newName: "ProfileImageUrl");

            migrationBuilder.RenameColumn(
                name: "phone_number",
                table: "Lecturers",
                newName: "PhoneNumber");

            migrationBuilder.RenameColumn(
                name: "full_name",
                table: "Lecturers",
                newName: "FullName");

            migrationBuilder.RenameColumn(
                name: "q1_relevance",
                table: "Feedbacks",
                newName: "q1_revelance");

            migrationBuilder.RenameColumn(
                name: "status",
                table: "Departments",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "level",
                table: "Departments",
                newName: "Level");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "Departments",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "update_at",
                table: "Departments",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "manager_id",
                table: "Departments",
                newName: "ManagerId");

            migrationBuilder.RenameColumn(
                name: "department_name",
                table: "Departments",
                newName: "DepartmentName");

            migrationBuilder.RenameColumn(
                name: "department_code",
                table: "Departments",
                newName: "DepartmentCode");

            migrationBuilder.RenameColumn(
                name: "create_at",
                table: "Departments",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "department_id",
                table: "Departments",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "CourseStatus",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CourseStatus",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CoursePosition",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "position_id",
                table: "CoursePosition",
                newName: "PositionId");

            migrationBuilder.RenameColumn(
                name: "course_id",
                table: "CoursePosition",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_CoursePosition_position_id",
                table: "CoursePosition",
                newName: "IX_CoursePosition_PositionId");

            migrationBuilder.RenameIndex(
                name: "IX_CoursePosition_course_id",
                table: "CoursePosition",
                newName: "IX_CoursePosition_CourseId");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CourseDepartment",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "department_id",
                table: "CourseDepartment",
                newName: "DepartmentId");

            migrationBuilder.RenameColumn(
                name: "course_id",
                table: "CourseDepartment",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseDepartment_department_id",
                table: "CourseDepartment",
                newName: "IX_CourseDepartment_DepartmentId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseDepartment_course_id",
                table: "CourseDepartment",
                newName: "IX_CourseDepartment_CourseId");

            migrationBuilder.RenameColumn(
                name: "name",
                table: "CourseCategories",
                newName: "Name");

            migrationBuilder.RenameColumn(
                name: "description",
                table: "CourseCategories",
                newName: "Description");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CourseCategories",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "title",
                table: "CourseAttachedFile",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "link",
                table: "CourseAttachedFile",
                newName: "Link");

            migrationBuilder.RenameColumn(
                name: "id",
                table: "CourseAttachedFile",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "public_id_url_pdf",
                table: "CourseAttachedFile",
                newName: "PublicIdUrlPdf");

            migrationBuilder.RenameColumn(
                name: "modified_at",
                table: "CourseAttachedFile",
                newName: "ModifiedTime");

            migrationBuilder.RenameColumn(
                name: "created_by_id",
                table: "CourseAttachedFile",
                newName: "UserId");

            migrationBuilder.RenameColumn(
                name: "created_at",
                table: "CourseAttachedFile",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "course_id",
                table: "CourseAttachedFile",
                newName: "CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseAttachedFile_created_by_id",
                table: "CourseAttachedFile",
                newName: "IX_CourseAttachedFile_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_CourseAttachedFile_course_id",
                table: "CourseAttachedFile",
                newName: "IX_CourseAttachedFile_CourseId");

            migrationBuilder.AddColumn<string>(
                name: "Type",
                table: "CourseAttachedFile",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_Departments_ManagerId",
                table: "Departments",
                column: "ManagerId",
                unique: true,
                filter: "[ManagerId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_CourseAttachedFile_AspNetUsers_UserId",
                table: "CourseAttachedFile",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

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
                name: "FK_CourseDepartment_Departments_DepartmentId",
                table: "CourseDepartment",
                column: "DepartmentId",
                principalTable: "Departments",
                principalColumn: "DepartmentId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoursePosition_Courses_CourseId",
                table: "CoursePosition",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_CoursePosition_Positions_PositionId",
                table: "CoursePosition",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "PositionId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Departments_AspNetUsers_ManagerId",
                table: "Departments",
                column: "ManagerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
