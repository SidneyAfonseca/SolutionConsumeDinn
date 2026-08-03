using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ConsumeDinnApi.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsuarioNormal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CreatedAt", "IsActive", "PasswordHash", "Role", "Username" },
                values: new object[] { 2, new DateTime(2026, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), true, "pbkdf2.sha256.100000.REluU2VlZFVzZXIyMDI2IQ==.xT6z9JCMstAg/lUOCv2rI/50mHcW6Z+u8WgAbNi3Qmk=", 2, "usuario" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2);
        }
    }
}
