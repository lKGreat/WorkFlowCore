using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkFlowCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddMenuQueryAndCacheFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "IsCache",
                table: "Menus",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "MenuNameKey",
                table: "Menus",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Query",
                table: "Menus",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCache",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "MenuNameKey",
                table: "Menus");

            migrationBuilder.DropColumn(
                name: "Query",
                table: "Menus");
        }
    }
}
