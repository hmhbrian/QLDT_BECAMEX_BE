using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QLDT_Becamex.Src.Migrations
{
    /// <inheritdoc />
    public partial class UpdateToApplicationUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "EndWork",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ModifedAt",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartWork",
                table: "AspNetUsers",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "EndWork",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ModifedAt",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "StartWork",
                table: "AspNetUsers");
        }
    }
}
