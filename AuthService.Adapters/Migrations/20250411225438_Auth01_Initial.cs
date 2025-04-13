using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace AuthService.Adapters.Migrations
{
    /// <inheritdoc />
    public partial class Auth01_Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "AuthService");

            migrationBuilder.CreateTable(
                name: "RefPermissions",
                schema: "AuthService",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefPermissions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RefVariables",
                schema: "AuthService",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Type = table.Column<string>(type: "text", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefVariables", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                schema: "AuthService",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Username = table.Column<string>(type: "character varying(64)", unicode: false, maxLength: 64, nullable: false),
                    PasswordHash = table.Column<string>(type: "character varying(256)", unicode: false, maxLength: 256, nullable: true),
                    RegisterDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    Enabled = table.Column<bool>(type: "boolean", nullable: false),
                    IsTemplate = table.Column<bool>(type: "boolean", nullable: false),
                    InvitedById = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Users_InvitedById",
                        column: x => x.InvitedById,
                        principalSchema: "AuthService",
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Invites",
                schema: "AuthService",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Link = table.Column<string>(type: "character varying(200)", unicode: false, maxLength: 200, nullable: false),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    InheritanceUserId = table.Column<int>(type: "integer", nullable: true),
                    Ttl = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Invites_Users_InheritanceUserId",
                        column: x => x.InheritanceUserId,
                        principalSchema: "AuthService",
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Invites_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "AuthService",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserPermissions",
                schema: "AuthService",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    PermissionId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AllowInheritance = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserPermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserPermissions_RefPermissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "AuthService",
                        principalTable: "RefPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserPermissions_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "AuthService",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserVariables",
                schema: "AuthService",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserVariables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserVariables_RefVariables_Name",
                        column: x => x.Name,
                        principalSchema: "AuthService",
                        principalTable: "RefVariables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserVariables_Users_UserId",
                        column: x => x.UserId,
                        principalSchema: "AuthService",
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InvitePermissions",
                schema: "AuthService",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InviteId = table.Column<int>(type: "integer", nullable: false),
                    PermissionId = table.Column<int>(type: "integer", nullable: false),
                    CreateDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false, defaultValueSql: "NOW()"),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    AllowInheritance = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InvitePermissions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InvitePermissions_Invites_InviteId",
                        column: x => x.InviteId,
                        principalSchema: "AuthService",
                        principalTable: "Invites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InvitePermissions_RefPermissions_PermissionId",
                        column: x => x.PermissionId,
                        principalSchema: "AuthService",
                        principalTable: "RefPermissions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InviteVariables",
                schema: "AuthService",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    InviteId = table.Column<int>(type: "integer", nullable: false),
                    Name = table.Column<int>(type: "integer", nullable: false),
                    Value = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InviteVariables", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InviteVariables_Invites_InviteId",
                        column: x => x.InviteId,
                        principalSchema: "AuthService",
                        principalTable: "Invites",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InviteVariables_RefVariables_Name",
                        column: x => x.Name,
                        principalSchema: "AuthService",
                        principalTable: "RefVariables",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "AuthService",
                table: "RefPermissions",
                columns: new[] { "Id", "Description", "Name" },
                values: new object[,]
                {
                    { 1024, "Root", "RootPermission" },
                    { 2000, "Просмотр Swagger", "Swagger" },
                    { 52000, "[Статьи] Создание новых", "ArticleCreating" },
                    { 52001, "[Статьи] Внесение изменений", "ArticleEditing" },
                    { 52002, "[Статьи] Закрепление", "ArticlePinning" },
                    { 52003, "[Статьи] Удаление", "ArticleRemoving" },
                    { 53000, "Смена имени пользователя", "AuthUsernameChanging" },
                    { 53001, "Смена пароля", "AuthPasswordChanging" },
                    { 53002, "Ссылки-приглашения", "AuthInviteLinks" },
                    { 53003, "Многоразовые ссылки-приглашения", "AuthReusableInviteLinks" },
                    { 53004, "Наследование прав по ссылкам-приглашениям", "AuthInheritanceInviteLinks" },
                    { 53005, "Изменение переменных (Security None)", "AuthChangingNoneSecureVariables" },
                    { 53006, "Изменение переменных (Security Low)", "AuthChangingLowSecureVariables" },
                    { 53007, "Изменение переменных (Security Medium)", "AuthChangingMediumSecureVariables" },
                    { 53008, "Изменение переменных (Security High)", "AuthChangingHighSecureVariables" },
                    { 53009, "Изменение переменных (Security Critical)", "AuthChangingCriticalSecureVariables" }
                });

            migrationBuilder.InsertData(
                schema: "AuthService",
                table: "RefVariables",
                columns: new[] { "Id", "Description", "Name", "Type" },
                values: new object[,]
                {
                    { 0, "Отображаемое имя", "DisplayName", "String" },
                    { 1, "TTL ссылки-приглашения (минуты)", "MaxLinkTTL", "Int32" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_InvitePermissions_InviteId",
                schema: "AuthService",
                table: "InvitePermissions",
                column: "InviteId");

            migrationBuilder.CreateIndex(
                name: "IX_InvitePermissions_PermissionId",
                schema: "AuthService",
                table: "InvitePermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_InheritanceUserId",
                schema: "AuthService",
                table: "Invites",
                column: "InheritanceUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Invites_UserId",
                schema: "AuthService",
                table: "Invites",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InviteVariables_InviteId",
                schema: "AuthService",
                table: "InviteVariables",
                column: "InviteId");

            migrationBuilder.CreateIndex(
                name: "IX_InviteVariables_Name",
                schema: "AuthService",
                table: "InviteVariables",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_PermissionId",
                schema: "AuthService",
                table: "UserPermissions",
                column: "PermissionId");

            migrationBuilder.CreateIndex(
                name: "IX_UserPermissions_UserId",
                schema: "AuthService",
                table: "UserPermissions",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_InvitedById",
                schema: "AuthService",
                table: "Users",
                column: "InvitedById",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                schema: "AuthService",
                table: "Users",
                column: "Username");

            migrationBuilder.CreateIndex(
                name: "IX_UserVariables_Name",
                schema: "AuthService",
                table: "UserVariables",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_UserVariables_UserId",
                schema: "AuthService",
                table: "UserVariables",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InvitePermissions",
                schema: "AuthService");

            migrationBuilder.DropTable(
                name: "InviteVariables",
                schema: "AuthService");

            migrationBuilder.DropTable(
                name: "UserPermissions",
                schema: "AuthService");

            migrationBuilder.DropTable(
                name: "UserVariables",
                schema: "AuthService");

            migrationBuilder.DropTable(
                name: "Invites",
                schema: "AuthService");

            migrationBuilder.DropTable(
                name: "RefPermissions",
                schema: "AuthService");

            migrationBuilder.DropTable(
                name: "RefVariables",
                schema: "AuthService");

            migrationBuilder.DropTable(
                name: "Users",
                schema: "AuthService");
        }
    }
}
