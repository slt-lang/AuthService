using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthService.Adapters.Migrations
{
    /// <inheritdoc />
    public partial class Auth02_FixInvitedBy : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_InvitedById",
                schema: "AuthService",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_InvitedById",
                schema: "AuthService",
                table: "Users",
                column: "InvitedById");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_InvitedById",
                schema: "AuthService",
                table: "Users");

            migrationBuilder.CreateIndex(
                name: "IX_Users_InvitedById",
                schema: "AuthService",
                table: "Users",
                column: "InvitedById",
                unique: true);
        }
    }
}
