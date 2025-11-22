using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkFlowCore.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddFileUploadTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CreateTime",
                table: "TaskInstances");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Users",
                newName: "ExtraProperties");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "Users",
                newName: "LastModifierId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Users",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Tenants",
                newName: "ExtraProperties");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "Tenants",
                newName: "LastModifierId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Tenants",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "TaskInstances",
                newName: "ExtraProperties");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "TaskInstances",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Roles",
                newName: "ExtraProperties");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "Roles",
                newName: "LastModifierId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Roles",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "ProcessInstances",
                newName: "ExtraProperties");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ProcessInstances",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "ProcessDefinitions",
                newName: "ExtraProperties");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "ProcessDefinitions",
                newName: "LastModifierId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "ProcessDefinitions",
                newName: "CreationTime");

            migrationBuilder.RenameColumn(
                name: "UpdatedAt",
                table: "Departments",
                newName: "ExtraProperties");

            migrationBuilder.RenameColumn(
                name: "DeletedAt",
                table: "Departments",
                newName: "LastModifierId");

            migrationBuilder.RenameColumn(
                name: "CreatedAt",
                table: "Departments",
                newName: "CreationTime");

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "Users",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Users",
                type: "TEXT",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "Users",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Tenants",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Tenants",
                type: "TEXT",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "Tenants",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Tenants",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Tenants",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "Tenants",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "TaskInstances",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "TaskInstances",
                type: "TEXT",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "TaskInstances",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "TaskInstances",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "TaskInstances",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "Roles",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Roles",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Roles",
                type: "TEXT",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "Roles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Roles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Roles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "Roles",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "ProcessInstances",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "ProcessInstances",
                type: "TEXT",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "ProcessInstances",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "ProcessInstances",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "LastModifierId",
                table: "ProcessInstances",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "ProcessDefinitions",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "ProcessDefinitions",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "ProcessDefinitions",
                type: "TEXT",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "ProcessDefinitions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "ProcessDefinitions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "ProcessDefinitions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "ProcessDefinitions",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "Departments",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Departments",
                type: "INTEGER",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER");

            migrationBuilder.AddColumn<string>(
                name: "ConcurrencyStamp",
                table: "Departments",
                type: "TEXT",
                maxLength: 40,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<Guid>(
                name: "CreatorId",
                table: "Departments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "DeleterId",
                table: "Departments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletionTime",
                table: "Departments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastModificationTime",
                table: "Departments",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "FileStorageProviders",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProviderName = table.Column<string>(type: "TEXT", nullable: false),
                    ProviderType = table.Column<int>(type: "INTEGER", nullable: false),
                    Configuration = table.Column<string>(type: "TEXT", maxLength: 4000, nullable: false),
                    IsEnabled = table.Column<bool>(type: "INTEGER", nullable: false),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false),
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
                    table.PrimaryKey("PK_FileStorageProviders", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FileAttachments",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    FileName = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    OriginalFileName = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    FileSize = table.Column<long>(type: "INTEGER", nullable: false),
                    ContentType = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    FileExtension = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    StorageProviderId = table.Column<long>(type: "INTEGER", nullable: false),
                    StoragePath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Md5Hash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    BusinessType = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    BusinessId = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    UploadStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    TotalChunks = table.Column<int>(type: "INTEGER", nullable: false),
                    UploadedChunks = table.Column<int>(type: "INTEGER", nullable: false),
                    AccessToken = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    TokenExpireAt = table.Column<DateTime>(type: "TEXT", nullable: true),
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
                    table.PrimaryKey("PK_FileAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileAttachments_FileStorageProviders_StorageProviderId",
                        column: x => x.StorageProviderId,
                        principalTable: "FileStorageProviders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FileChunks",
                columns: table => new
                {
                    Id = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AttachmentId = table.Column<long>(type: "INTEGER", nullable: false),
                    ChunkIndex = table.Column<int>(type: "INTEGER", nullable: false),
                    ChunkSize = table.Column<long>(type: "INTEGER", nullable: false),
                    ChunkHash = table.Column<string>(type: "TEXT", maxLength: 64, nullable: false),
                    UploadStatus = table.Column<int>(type: "INTEGER", nullable: false),
                    StoragePath = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
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
                    table.PrimaryKey("PK_FileChunks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FileChunks_FileAttachments_AttachmentId",
                        column: x => x.AttachmentId,
                        principalTable: "FileAttachments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_AccessToken",
                table: "FileAttachments",
                column: "AccessToken");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_BusinessType_BusinessId",
                table: "FileAttachments",
                columns: new[] { "BusinessType", "BusinessId" });

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_Md5Hash",
                table: "FileAttachments",
                column: "Md5Hash");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_StorageProviderId",
                table: "FileAttachments",
                column: "StorageProviderId");

            migrationBuilder.CreateIndex(
                name: "IX_FileAttachments_TenantId_UploadStatus",
                table: "FileAttachments",
                columns: new[] { "TenantId", "UploadStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_FileChunks_AttachmentId_ChunkIndex",
                table: "FileChunks",
                columns: new[] { "AttachmentId", "ChunkIndex" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FileChunks_AttachmentId_UploadStatus",
                table: "FileChunks",
                columns: new[] { "AttachmentId", "UploadStatus" });

            migrationBuilder.CreateIndex(
                name: "IX_FileStorageProviders_ProviderType",
                table: "FileStorageProviders",
                column: "ProviderType");

            migrationBuilder.CreateIndex(
                name: "IX_FileStorageProviders_TenantId_IsEnabled",
                table: "FileStorageProviders",
                columns: new[] { "TenantId", "IsEnabled" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FileChunks");

            migrationBuilder.DropTable(
                name: "FileAttachments");

            migrationBuilder.DropTable(
                name: "FileStorageProviders");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "Tenants");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "TaskInstances");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "TaskInstances");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "TaskInstances");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "TaskInstances");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "Roles");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "ProcessInstances");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "ProcessInstances");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "ProcessInstances");

            migrationBuilder.DropColumn(
                name: "LastModifierId",
                table: "ProcessInstances");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "ProcessDefinitions");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "ProcessDefinitions");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "ProcessDefinitions");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "ProcessDefinitions");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "ProcessDefinitions");

            migrationBuilder.DropColumn(
                name: "ConcurrencyStamp",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "CreatorId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DeleterId",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "DeletionTime",
                table: "Departments");

            migrationBuilder.DropColumn(
                name: "LastModificationTime",
                table: "Departments");

            migrationBuilder.RenameColumn(
                name: "LastModifierId",
                table: "Users",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "ExtraProperties",
                table: "Users",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Users",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifierId",
                table: "Tenants",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "ExtraProperties",
                table: "Tenants",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Tenants",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ExtraProperties",
                table: "TaskInstances",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "TaskInstances",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifierId",
                table: "Roles",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "ExtraProperties",
                table: "Roles",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Roles",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "ExtraProperties",
                table: "ProcessInstances",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "ProcessInstances",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifierId",
                table: "ProcessDefinitions",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "ExtraProperties",
                table: "ProcessDefinitions",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "ProcessDefinitions",
                newName: "CreatedAt");

            migrationBuilder.RenameColumn(
                name: "LastModifierId",
                table: "Departments",
                newName: "DeletedAt");

            migrationBuilder.RenameColumn(
                name: "ExtraProperties",
                table: "Departments",
                newName: "UpdatedAt");

            migrationBuilder.RenameColumn(
                name: "CreationTime",
                table: "Departments",
                newName: "CreatedAt");

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "Users",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Users",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Tenants",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "TaskInstances",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreateTime",
                table: "TaskInstances",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "Roles",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Roles",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "ProcessInstances",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "ProcessDefinitions",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "ProcessDefinitions",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldDefaultValue: false);

            migrationBuilder.AlterColumn<Guid>(
                name: "TenantId",
                table: "Departments",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "IsDeleted",
                table: "Departments",
                type: "INTEGER",
                nullable: false,
                oldClrType: typeof(bool),
                oldType: "INTEGER",
                oldDefaultValue: false);
        }
    }
}
