using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkFlowCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAbpUserIdToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "AbpUserId",
                table: "Users",
                type: "TEXT",
                nullable: true,
                comment: "关联的 ABP Identity 用户ID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AbpUserId",
                table: "Users",
                column: "AbpUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_AbpUserId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AbpUserId",
                table: "Users");
        }
    }
}
