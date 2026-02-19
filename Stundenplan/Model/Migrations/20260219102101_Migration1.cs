using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Model.Migrations
{
    /// <inheritdoc />
    public partial class Migration1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Classes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Classes", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Description = table.Column<string>(type: "longtext", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClassesSubjects",
                columns: table => new
                {
                    CId = table.Column<int>(type: "int", nullable: false),
                    SId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassesSubjects", x => new { x.CId, x.SId });
                    table.ForeignKey(
                        name: "FK_ClassesSubjects_Classes_CId",
                        column: x => x.CId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassesSubjects_Subjects_SId",
                        column: x => x.SId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Classes",
                columns: new[] { "Id", "Description" },
                values: new object[,]
                {
                    { 1, "1CHIT" },
                    { 2, "2CHIT" },
                    { 3, "3CHIT" },
                    { 4, "4CHIT" },
                    { 5, "5CHIT" }
                });

            migrationBuilder.InsertData(
                table: "Subjects",
                columns: new[] { "Id", "Description" },
                values: new object[,]
                {
                    { 1, "AM" },
                    { 2, "SEW" },
                    { 3, "INSY" },
                    { 4, "D" },
                    { 5, "E" }
                });

            migrationBuilder.InsertData(
                table: "ClassesSubjects",
                columns: new[] { "CId", "SId" },
                values: new object[,]
                {
                    { 1, 2 },
                    { 5, 2 },
                    { 5, 3 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassesSubjects_SId",
                table: "ClassesSubjects",
                column: "SId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ClassesSubjects");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Subjects");
        }
    }
}
