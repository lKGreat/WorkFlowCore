using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkFlowCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTaskEntities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SysTaskLogs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    TaskId = table.Column<long>(type: "INTEGER", nullable: false),
                    TaskName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TaskGroup = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    InvokeTarget = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    LogInfo = table.Column<string>(type: "TEXT", maxLength: 2000, nullable: true),
                    Exception = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: true),
                    StartTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    EndTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    Duration = table.Column<int>(type: "INTEGER", nullable: true),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ExtraProperties = table.Column<string>(type: "TEXT", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<Guid>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysTaskLogs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SysTasks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false),
                    TaskName = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    TaskGroup = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    InvokeTarget = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    CronExpression = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    Concurrent = table.Column<int>(type: "INTEGER", nullable: false),
                    MisfirePolicy = table.Column<int>(type: "INTEGER", nullable: false),
                    Remark = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TenantId = table.Column<Guid>(type: "TEXT", nullable: true),
                    ExtraProperties = table.Column<string>(type: "TEXT", nullable: false),
                    ConcurrencyStamp = table.Column<string>(type: "TEXT", maxLength: 40, nullable: false),
                    CreationTime = table.Column<DateTime>(type: "TEXT", nullable: false),
                    CreatorId = table.Column<Guid>(type: "TEXT", nullable: true),
                    LastModificationTime = table.Column<DateTime>(type: "TEXT", nullable: true),
                    LastModifierId = table.Column<Guid>(type: "TEXT", nullable: true),
                    IsDeleted = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    DeleterId = table.Column<Guid>(type: "TEXT", nullable: true),
                    DeletionTime = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SysTasks", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SysTaskLogs_CreationTime",
                table: "SysTaskLogs",
                column: "CreationTime");

            migrationBuilder.CreateIndex(
                name: "IX_SysTaskLogs_Status",
                table: "SysTaskLogs",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SysTaskLogs_TaskId",
                table: "SysTaskLogs",
                column: "TaskId");

            migrationBuilder.CreateIndex(
                name: "IX_SysTasks_Status",
                table: "SysTasks",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_SysTasks_TaskName_TaskGroup",
                table: "SysTasks",
                columns: new[] { "TaskName", "TaskGroup" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SysTaskLogs");

            migrationBuilder.DropTable(
                name: "SysTasks");
        }
    }
}
