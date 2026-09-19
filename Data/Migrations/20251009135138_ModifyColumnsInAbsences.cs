using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyProject.Migrations
{
    /// <inheritdoc />
    public partial class ModifyColumnsInAbsences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Absences");

            migrationBuilder.RenameColumn(
                name: "Attendant",
                table: "Absences",
                newName: "TacsAttendant");

            migrationBuilder.AddColumn<bool>(
                name: "AlhanAttendant",
                table: "Absences",
                type: "bit",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "CopticAttendant",
                table: "Absences",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AlhanAttendant",
                table: "Absences");

            migrationBuilder.DropColumn(
                name: "CopticAttendant",
                table: "Absences");

            migrationBuilder.RenameColumn(
                name: "TacsAttendant",
                table: "Absences",
                newName: "Attendant");

            migrationBuilder.AddColumn<int>(
                name: "Subject",
                table: "Absences",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
