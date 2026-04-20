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
                    Abbr = table.Column<string>(type: "longtext", nullable: false)
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
                    Desc = table.Column<string>(type: "longtext", nullable: false)
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
                    Abbr = table.Column<string>(type: "longtext", nullable: false)
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
                    Sid = table.Column<int>(type: "int", nullable: false),
                    HourCount = table.Column<int>(type: "int", nullable: true)
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
                    WeekDay = table.Column<int>(type: "int", nullable: false),
                    Hour = table.Column<int>(type: "int", nullable: false)
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
                    { 2, "D" },
                    { 3, "E1" },
                    { 4, "INSY" },
                    { 5, "RK" },
                    { 6, "SEW" },
                    { 7, "SYTD" },
                    { 8, "GGPH" },
                    { 9, "GGPG" },
                    { 10, "NW2P" },
                    { 11, "NW2C" },
                    { 12, "DSAI" },
                    { 13, "WIR" },
                    { 14, "ITPL" },
                    { 15, "ITPP" },
                    { 16, "BESP" },
                    { 17, "SYTI" },
                    { 18, "SYTS" }
                });

            migrationBuilder.InsertData(
                table: "Teachers",
                columns: new[] { "Id", "Abbr" },
                values: new object[,]
                {
                    { 1, "ALLI" },
                    { 2, "MACO" },
                    { 3, "NIGI" },
                    { 4, "LEYV" },
                    { 5, "WART" },
                    { 6, "BRUN" },
                    { 7, "KUBI" },
                    { 8, "JAGE" },
                    { 9, "WIEN" },
                    { 10, "ELSH" },
                    { 11, "WINN" },
                    { 12, "KIEN" },
                    { 13, "HAUP" },
                    { 14, "HAUL" }
                });

            migrationBuilder.InsertData(
                table: "ClassSubjects",
                columns: new[] { "Cid", "Sid" },
                values: new object[,]
                {
                    { 4, 1 },
                    { 4, 2 },
                    { 4, 3 },
                    { 4, 4 },
                    { 4, 5 },
                    { 4, 6 },
                    { 4, 7 },
                    { 4, 8 },
                    { 4, 9 },
                    { 4, 10 },
                    { 4, 11 },
                    { 4, 12 },
                    { 4, 13 },
                    { 4, 14 },
                    { 4, 15 },
                    { 4, 16 },
                    { 4, 17 },
                    { 4, 18 }
                });

            migrationBuilder.InsertData(
                table: "TeacherSubjects",
                columns: new[] { "Sid", "Tid", "HourCount" },
                values: new object[,]
                {
                    { 5, 1, null },
                    { 4, 2, null },
                    { 6, 2, null },
                    { 1, 3, null },
                    { 9, 3, null },
                    { 2, 4, null },
                    { 8, 4, null },
                    { 10, 5, null },
                    { 11, 5, null },
                    { 12, 6, null },
                    { 17, 6, null },
                    { 13, 7, null },
                    { 14, 8, null },
                    { 15, 9, null },
                    { 3, 10, null },
                    { 7, 11, null },
                    { 16, 12, null },
                    { 17, 13, null },
                    { 18, 14, null }
                });

            migrationBuilder.InsertData(
                table: "Lessons",
                columns: new[] { "Id", "Cid", "Hour", "Sid", "Tid", "WeekDay" },
                values: new object[,]
                {
                    { 1, 4, 1, 5, 1, 1 },
                    { 2, 4, 2, 13, 7, 1 },
                    { 3, 4, 3, 13, 7, 1 },
                    { 4, 4, 4, 5, 1, 1 },
                    { 5, 4, 5, 16, 12, 1 },
                    { 6, 4, 7, 10, 5, 1 },
                    { 7, 4, 8, 6, 2, 1 },
                    { 8, 4, 9, 6, 2, 1 },
                    { 9, 4, 1, 8, 4, 2 },
                    { 10, 4, 2, 14, 8, 2 },
                    { 11, 4, 3, 2, 4, 2 },
                    { 12, 4, 4, 2, 4, 2 },
                    { 13, 4, 5, 17, 13, 2 },
                    { 14, 4, 7, 14, 8, 2 },
                    { 15, 4, 8, 14, 8, 2 },
                    { 16, 4, 1, 9, 3, 3 },
                    { 17, 4, 2, 15, 9, 3 },
                    { 18, 4, 3, 3, 10, 3 },
                    { 19, 4, 4, 7, 11, 3 },
                    { 20, 4, 5, 7, 11, 3 },
                    { 21, 4, 6, 15, 9, 3 },
                    { 22, 4, 1, 11, 5, 4 },
                    { 23, 4, 2, 4, 2, 4 },
                    { 24, 4, 3, 4, 2, 4 },
                    { 25, 4, 4, 4, 2, 4 },
                    { 26, 4, 5, 4, 2, 4 },
                    { 27, 4, 6, 6, 2, 4 },
                    { 28, 4, 8, 18, 14, 4 },
                    { 29, 4, 9, 18, 14, 4 },
                    { 30, 4, 1, 12, 6, 5 },
                    { 31, 4, 2, 12, 6, 5 },
                    { 32, 4, 3, 13, 7, 5 },
                    { 33, 4, 4, 3, 10, 5 },
                    { 34, 4, 5, 1, 3, 5 },
                    { 35, 4, 6, 1, 3, 5 },
                    { 36, 4, 8, 17, 6, 5 },
                    { 37, 4, 9, 17, 6, 5 }
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
