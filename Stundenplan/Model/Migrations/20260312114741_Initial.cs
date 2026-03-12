using Microsoft.EntityFrameworkCore.Migrations;
using MySql.EntityFrameworkCore.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Model.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
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
                    Abbr = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
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
                    Desc = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Teachers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Abbr = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Teachers", x => x.Id);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "ClassSubjects",
                columns: table => new
                {
                    Cid = table.Column<int>(type: "int", nullable: false),
                    Sid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ClassSubjects", x => new { x.Cid, x.Sid });
                    table.ForeignKey(
                        name: "FK_ClassSubjects_Classes_Cid",
                        column: x => x.Cid,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ClassSubjects_Subjects_Sid",
                        column: x => x.Sid,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "TeacherSubjects",
                columns: table => new
                {
                    Tid = table.Column<int>(type: "int", nullable: false),
                    Sid = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherSubjects", x => new { x.Tid, x.Sid });
                    table.ForeignKey(
                        name: "FK_TeacherSubjects_Subjects_Sid",
                        column: x => x.Sid,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherSubjects_Teachers_Tid",
                        column: x => x.Tid,
                        principalTable: "Teachers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.CreateTable(
                name: "Lessons",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySQL:ValueGenerationStrategy", MySQLValueGenerationStrategy.IdentityColumn),
                    Tid = table.Column<int>(type: "int", nullable: false),
                    Sid = table.Column<int>(type: "int", nullable: false),
                    Cid = table.Column<int>(type: "int", nullable: false),
                    Day = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: true),
                    Hour = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lessons", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Lessons_ClassSubjects_Cid_Sid",
                        columns: x => new { x.Cid, x.Sid },
                        principalTable: "ClassSubjects",
                        principalColumns: new[] { "Cid", "Sid" },
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Lessons_TeacherSubjects_Tid_Sid",
                        columns: x => new { x.Tid, x.Sid },
                        principalTable: "TeacherSubjects",
                        principalColumns: new[] { "Tid", "Sid" },
                        onDelete: ReferentialAction.Restrict);
                })
                .Annotation("MySQL:Charset", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Classes",
                columns: new[] { "Id", "Abbr" },
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
                columns: new[] { "Id", "Desc" },
                values: new object[,]
                {
                    { 1, "AM" },
                    { 4, "D" },
                    { 5, "E" },
                    { 9, "INSY" },
                    { 18, "REL" },
                    { 19, "SEW" },
                    { 20, "SOPK" },
                    { 21, "SYT" }
                });

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "Id", "Abbr" },
                values: new object[,]
                {
                    { 1, "ALLI" },
                    { 2, "BIRN" },
                    { 13, "MACO" },
                    { 14, "NIGI" },
                    { 19, "STRO" }
                });

            migrationBuilder.InsertData(
                table: "ClassSubjects",
                columns: new[] { "Cid", "Sid" },
                values: new object[,]
                {
                    { 1, 18 },
                    { 4, 9 },
                    { 4, 18 },
                    { 4, 19 },
                    { 5, 19 }
                });

            migrationBuilder.InsertData(
                table: "TeacherSubjects",
                columns: new[] { "Sid", "Tid" },
                values: new object[,]
                {
                    { 18, 1 },
                    { 19, 2 },
                    { 9, 13 },
                    { 19, 13 }
                });

            migrationBuilder.InsertData(
                table: "Lessons",
                columns: new[] { "Id", "Cid", "Day", "Hour", "Sid", "Tid" },
                values: new object[,]
                {
                    { 203, 1, "Mo", 1, 18, 1 },
                    { 299, 4, "Di", 2, 18, 1 },
                    { 378, 4, "Mi", 3, 9, 13 },
                    { 379, 4, "Do", 4, 19, 13 },
                    { 399, 5, "Fr", 5, 19, 13 },
                    { 1277, 5, "Mo", 6, 19, 2 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_ClassSubjects_Sid",
                table: "ClassSubjects",
                column: "Sid");

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_Cid_Sid",
                table: "Lessons",
                columns: new[] { "Cid", "Sid" });

            migrationBuilder.CreateIndex(
                name: "IX_Lessons_Tid_Sid",
                table: "Lessons",
                columns: new[] { "Tid", "Sid" });

            migrationBuilder.CreateIndex(
                name: "IX_TeacherSubjects_Sid",
                table: "TeacherSubjects",
                column: "Sid");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lessons");

            migrationBuilder.DropTable(
                name: "ClassSubjects");

            migrationBuilder.DropTable(
                name: "TeacherSubjects");

            migrationBuilder.DropTable(
                name: "Classes");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "Teachers");
        }
    }
}
