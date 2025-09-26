using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Migrations
{
    /// <inheritdoc />
    public partial class FirebaseMessagingMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Devices",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    device_token = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    platform = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Devices", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Messages",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    title = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    body = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    data = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    send_type = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    sent_by = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Messages", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "Topics",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    created_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Topics", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "UserDevices",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    user_id = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: true),
                    device_id = table.Column<int>(type: "int", nullable: true),
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

            migrationBuilder.CreateTable(
                name: "MessageLogs",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    message_id = table.Column<int>(type: "int", nullable: true),
                    device_id = table.Column<int>(type: "int", nullable: true),
                    topic_id = table.Column<int>(type: "int", nullable: true),
                    status = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    error_message = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    sent_at = table.Column<DateTime>(type: "datetime2", nullable: false),
                    received_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageLogs", x => x.id);
                    table.ForeignKey(
                        name: "FK_MessageLogs_Devices_device_id",
                        column: x => x.device_id,
                        principalTable: "Devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MessageLogs_Messages_message_id",
                        column: x => x.message_id,
                        principalTable: "Messages",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MessageLogs_Topics_topic_id",
                        column: x => x.topic_id,
                        principalTable: "Topics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TopicSubscriptions",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    topic_id = table.Column<int>(type: "int", nullable: true),
                    device_id = table.Column<int>(type: "int", nullable: true),
                    subscribe_at = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TopicSubscriptions", x => x.id);
                    table.ForeignKey(
                        name: "FK_TopicSubscriptions_Devices_device_id",
                        column: x => x.device_id,
                        principalTable: "Devices",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_TopicSubscriptions_Topics_topic_id",
                        column: x => x.topic_id,
                        principalTable: "Topics",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_device_id",
                table: "MessageLogs",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_message_id",
                table: "MessageLogs",
                column: "message_id");

            migrationBuilder.CreateIndex(
                name: "IX_MessageLogs_topic_id",
                table: "MessageLogs",
                column: "topic_id");

            migrationBuilder.CreateIndex(
                name: "IX_TopicSubscriptions_device_id",
                table: "TopicSubscriptions",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "IX_TopicSubscriptions_topic_id",
                table: "TopicSubscriptions",
                column: "topic_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_device_id",
                table: "UserDevices",
                column: "device_id");

            migrationBuilder.CreateIndex(
                name: "IX_UserDevices_user_id",
                table: "UserDevices",
                column: "user_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageLogs");

            migrationBuilder.DropTable(
                name: "TopicSubscriptions");

            migrationBuilder.DropTable(
                name: "UserDevices");

            migrationBuilder.DropTable(
                name: "Messages");

            migrationBuilder.DropTable(
                name: "Topics");

            migrationBuilder.DropTable(
                name: "Devices");
        }
    }
}
