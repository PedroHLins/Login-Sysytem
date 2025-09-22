using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Login_Sysytem.Migrations
{
    /// <inheritdoc />
    public partial class HashedUserPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Password",
                table: "Usuarios",
                newName: "PasswordHash");

            migrationBuilder.RenameColumn(
                name: "Password",
                table: "admins",
                newName: "PasswordHash");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "Usuarios",
                newName: "Password");

            migrationBuilder.RenameColumn(
                name: "PasswordHash",
                table: "admins",
                newName: "Password");
        }
    }
}
