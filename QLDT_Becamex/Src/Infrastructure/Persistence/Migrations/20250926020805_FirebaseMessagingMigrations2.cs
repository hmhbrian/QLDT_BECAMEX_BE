using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class FirebaseMessagingMigrations2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserDevices");

            migrationBuilder.AddColumn<string>(
                name: "user_id",
                table: "Devices",
                type: "nvarchar(450)",
                maxLength: 450,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Devices_user_id",
                table: "Devices",
                column: "user_id");

            migrationBuilder.AddForeignKey(
                name: "FK_Devices_AspNetUsers_user_id",
                table: "Devices",
                column: "user_id",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Devices_AspNetUsers_user_id",
                table: "Devices");

            migrationBuilder.DropIndex(
                name: "IX_Devices_user_id",
                table: "Devices");

            migrationBuilder.DropColumn(
                name: "user_id",
                table: "Devices");

            migrationBuilder.CreateTable(
                name: "UserDevices",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    device_id = table.Column<int>(type: "int", nullable: true),
                    user_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    last_login_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDevices", x => x.id);
                    table.ForeignKey(
                        name: "FK_UserDevices_AspNetUsers_user_id",
                        column: x => x.user_id,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_UserDevices_Devices_device_id",
                        column: x => x.device_id,
                        principalTable: "Devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_device_id",
                table: "UserDevices",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_user_id",
                table: "UserDevices",
                column: "user_id");
        }
    }
}
