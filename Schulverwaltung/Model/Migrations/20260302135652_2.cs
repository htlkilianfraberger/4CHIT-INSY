using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Model.Migrations
{
    /// <inheritdoc />
    public partial class _2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pupils_Classes_ClassId",
                table: "Pupils");

            migrationBuilder.UpdateData(
                table: "Pupils",
                keyColumn: "Id",
                keyValue: 1,
                column: "ClassId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Pupils",
                keyColumn: "Id",
                keyValue: 2,
                column: "ClassId",
                value: 4);

            migrationBuilder.UpdateData(
                table: "Pupils",
                keyColumn: "Id",
                keyValue: 3,
                column: "ClassId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Pupils",
                keyColumn: "Id",
                keyValue: 4,
                column: "ClassId",
                value: 5);

            migrationBuilder.UpdateData(
                table: "Pupils",
                keyColumn: "Id",
                keyValue: 5,
                column: "ClassId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Pupils",
                keyColumn: "Id",
                keyValue: 6,
                column: "ClassId",
                value: 1);

            migrationBuilder.UpdateData(
                table: "Pupils",
                keyColumn: "Id",
                keyValue: 7,
                column: "ClassId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Pupils",
                keyColumn: "Id",
                keyValue: 8,
                column: "ClassId",
                value: 2);

            migrationBuilder.UpdateData(
                table: "Pupils",
                keyColumn: "Id",
                keyValue: 9,
                column: "ClassId",
                value: 3);

            migrationBuilder.UpdateData(
                table: "Pupils",
                keyColumn: "Id",
                keyValue: 10,
                column: "ClassId",
                value: 3);

            migrationBuilder.AddForeignKey(
                name: "FK_Pupils_Classes_ClassId",
                table: "Pupils",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Pupils_Classes_ClassId",
                table: "Pupils");

            migrationBuilder.UpdateData(
                table: "Pupils",
                keyColumn: "Id",
                keyValue: 1,
                column: "ClassId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Pupils",
                keyColumn: "Id",
                keyValue: 2,
                column: "ClassId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Pupils",
                keyColumn: "Id",
                keyValue: 3,
                column: "ClassId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Pupils",
                keyColumn: "Id",
                keyValue: 4,
                column: "ClassId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Pupils",
                keyColumn: "Id",
                keyValue: 5,
                column: "ClassId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Pupils",
                keyColumn: "Id",
                keyValue: 6,
                column: "ClassId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Pupils",
                keyColumn: "Id",
                keyValue: 7,
                column: "ClassId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Pupils",
                keyColumn: "Id",
                keyValue: 8,
                column: "ClassId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Pupils",
                keyColumn: "Id",
                keyValue: 9,
                column: "ClassId",
                value: null);

            migrationBuilder.UpdateData(
                table: "Pupils",
                keyColumn: "Id",
                keyValue: 10,
                column: "ClassId",
                value: null);

            migrationBuilder.AddForeignKey(
                name: "FK_Pupils_Classes_ClassId",
                table: "Pupils",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id");
        }
    }
}
