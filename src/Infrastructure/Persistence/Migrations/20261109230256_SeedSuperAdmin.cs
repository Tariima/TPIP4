using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedSuperAdmin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Usuarios",
                columns: new[] { "Id", "Activo", "Apellido", "Contrasena", "Discriminator", "Email", "FechaRegistro", "Nombre" },
                values: new object[] { 1, true, "Admin", "$2a$11$bjdAH2wJIDk04JQa3kP8cO9kgGk/conQEIKICUug66qzuzix0LINC", "SuperAdmin", "superadmin@tpip4.com", new DateTime(2026, 11, 9, 0, 0, 0, 0, DateTimeKind.Unspecified), "Super" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Usuarios",
                keyColumn: "Id",
                keyValue: 1);
        }
    }
}
