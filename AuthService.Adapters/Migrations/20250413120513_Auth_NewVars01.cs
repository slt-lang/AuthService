using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Adapters.Migrations
{
    /// <inheritdoc />
    public partial class Auth_NewVars01 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "AuthService",
                table: "RefVariables",
                columns: new[] { "Id", "Description", "Name", "Type" },
                values: new object[] { 2, "Количество одновременных ссылок-приглашений", "MaximumInviteLinks", "Int32" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "AuthService",
                table: "RefVariables",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
