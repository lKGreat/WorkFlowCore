using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkFlowCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleDataScopeFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DataScope",
                table: "Roles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "RoleSort",
                table: "Roles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Roles",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DataScope",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "RoleSort",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Roles");
        }
    }
}
