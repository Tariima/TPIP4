using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class SeedEjercicios : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Ejercicios",
                columns: new[] { "Id", "Descripcion", "GrupoMuscular", "Nombre" },
                values: new object[,]
                {
                    { 1, "Acostado en el banco, bajar la barra hasta el pecho y empujarla hacia arriba.", "Pecho", "Press de banca" },
                    { 2, "En el banco inclinado, empujar las mancuernas hacia arriba desde la altura del pecho.", "Pecho", "Press inclinado con mancuernas" },
                    { 3, "Con el cuerpo recto, bajar el pecho hasta casi tocar el piso y volver a subir.", "Pecho", "Flexiones de brazos" },
                    { 4, "Colgado de la barra, subir el cuerpo hasta que el mentón pase la barra.", "Espalda", "Dominadas" },
                    { 5, "Con el torso inclinado hacia adelante, llevar la barra hacia el abdomen.", "Espalda", "Remo con barra" },
                    { 6, "Sentado en la polea alta, bajar la barra hasta la altura del pecho.", "Espalda", "Jalón al pecho" },
                    { 7, "Con la barra en la espalda, bajar flexionando las rodillas hasta que los muslos queden paralelos al piso.", "Piernas", "Sentadilla" },
                    { 8, "Levantar la barra desde el piso con la espalda recta hasta quedar parado.", "Piernas", "Peso muerto" },
                    { 9, "Sentado en la máquina, empujar la plataforma con los pies.", "Piernas", "Prensa de piernas" },
                    { 10, "Parado, empujar la barra desde los hombros hasta arriba de la cabeza.", "Hombros", "Press militar" },
                    { 11, "Con una mancuerna en cada mano, levantar los brazos por los costados hasta la altura de los hombros.", "Hombros", "Elevaciones laterales" },
                    { 12, "Con los codos pegados al cuerpo, flexionar los brazos llevando la barra hacia los hombros.", "Brazos", "Curl de bíceps con barra" },
                    { 13, "Con los codos fijos, estirar los brazos empujando la soga hacia abajo.", "Brazos", "Extensión de tríceps en polea" },
                    { 14, "Acostado boca arriba, levantar los hombros del piso contrayendo el abdomen.", "Abdominales", "Crunch abdominal" },
                    { 15, "Acostado boca arriba, levantar las piernas estiradas hasta formar un ángulo recto con el torso.", "Abdominales", "Elevación de piernas" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Ejercicios",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Ejercicios",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Ejercicios",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Ejercicios",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Ejercicios",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Ejercicios",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Ejercicios",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Ejercicios",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Ejercicios",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Ejercicios",
                keyColumn: "Id",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "Ejercicios",
                keyColumn: "Id",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "Ejercicios",
                keyColumn: "Id",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "Ejercicios",
                keyColumn: "Id",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "Ejercicios",
                keyColumn: "Id",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "Ejercicios",
                keyColumn: "Id",
                keyValue: 15);
        }
    }
}
