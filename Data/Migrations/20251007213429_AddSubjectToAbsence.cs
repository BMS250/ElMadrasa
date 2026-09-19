using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyProject.Migrations
{
    /// <inheritdoc />
    public partial class AddSubjectToAbsence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Class",
                table: "Students");

            migrationBuilder.AddColumn<string>(
                name: "ClassId",
                table: "Students",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Subject",
                table: "Absences",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Students_ClassId",
                table: "Students",
                column: "ClassId");

            migrationBuilder.AddForeignKey(
                name: "FK_Students_Classes_ClassId",
                table: "Students",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Students_Classes_ClassId",
                table: "Students");

            migrationBuilder.DropIndex(
                name: "IX_Students_ClassId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "ClassId",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Absences");

            migrationBuilder.AddColumn<int>(
                name: "Class",
                table: "Students",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
