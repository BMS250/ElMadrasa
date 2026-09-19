using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyProject.Migrations
{
    /// <inheritdoc />
    public partial class MakeAllIdsStrings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraints first
            migrationBuilder.DropForeignKey(
                name: "FK_Absences_Students_StudentId",
                table: "Absences");

            migrationBuilder.DropForeignKey(
                name: "FK_ServantClasses_Classes_ClassId",
                table: "ServantClasses");

            // Drop any custom indexes that depend on columns we're changing
            migrationBuilder.DropIndex(
                name: "IDX_AbsenceDate_StudentID",
                table: "Absences");

            migrationBuilder.DropIndex(
                name: "IX_Absences_StudentId",
                table: "Absences");

            migrationBuilder.DropIndex(
                name: "IX_ServantClasses_ClassId",
                table: "ServantClasses");

            // Students table - Drop PK, drop column, add new column, re-add PK
            migrationBuilder.DropPrimaryKey(
                name: "PK_Students",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Students");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Students",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            // Generate unique IDs for existing rows using GUIDs
            migrationBuilder.Sql("UPDATE Students SET Id = CAST(NEWID() AS NVARCHAR(450)) WHERE Id = ''");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Students",
                table: "Students",
                column: "Id");

            // ServantClasses - Change ClassId foreign key and Id
            migrationBuilder.AlterColumn<string>(
                name: "ClassId",
                table: "ServantClasses",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // Clear ServantClasses data since we can't maintain FK relationship when changing ID types
            migrationBuilder.Sql("DELETE FROM ServantClasses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServantClasses",
                table: "ServantClasses");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ServantClasses");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "ServantClasses",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE ServantClasses SET Id = CAST(NEWID() AS NVARCHAR(450)) WHERE Id = ''");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServantClasses",
                table: "ServantClasses",
                column: "Id");

            // Otps - Drop PK, drop column, add new column, re-add PK
            migrationBuilder.DropPrimaryKey(
                name: "PK_Otps",
                table: "Otps");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Otps");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Otps",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("UPDATE Otps SET Id = CAST(NEWID() AS NVARCHAR(450)) WHERE Id = ''");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Otps",
                table: "Otps",
                column: "Id");

            // Classes table - Drop PK, drop column, add new column, re-add PK
            migrationBuilder.DropPrimaryKey(
                name: "PK_Classes",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Classes");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Classes",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            // Generate unique IDs for Classes
            migrationBuilder.Sql("UPDATE Classes SET Id = CAST(NEWID() AS NVARCHAR(450)) WHERE Id = ''");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Classes",
                table: "Classes",
                column: "Id");

            migrationBuilder.AddColumn<int>(
                name: "Number",
                table: "Classes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            // Absences table - Change StudentId foreign key, then drop PK, drop column, add new column, re-add PK
            migrationBuilder.AddColumn<string>(
                name: "StudentId_Temp",
                table: "Absences",
                type: "nvarchar(450)",
                nullable: true);

            // Map old StudentId (int) to new Students.Id (string) - this will break the relationship
            // Since we can't maintain the FK relationship when changing types, we'll set to NULL temporarily

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Absences");

            migrationBuilder.RenameColumn(
                name: "StudentId_Temp",
                table: "Absences",
                newName: "StudentId");

            migrationBuilder.AlterColumn<string>(
                name: "StudentId",
                table: "Absences",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            // Clear Absences data since we can't maintain FK relationship when changing ID types
            migrationBuilder.Sql("DELETE FROM Absences");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Absences",
                table: "Absences");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Absences");

            migrationBuilder.AddColumn<string>(
                name: "Id",
                table: "Absences",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            // Generate unique IDs for Absences
            migrationBuilder.Sql("UPDATE Absences SET Id = CAST(NEWID() AS NVARCHAR(450)) WHERE Id = ''");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Absences",
                table: "Absences",
                column: "Id");

            // Re-create indexes
            migrationBuilder.CreateIndex(
                name: "IX_Absences_StudentId",
                table: "Absences",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IDX_AbsenceDate_StudentID",
                table: "Absences",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ServantClasses_ClassId",
                table: "ServantClasses",
                column: "ClassId");

            // Re-add foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_Absences_Students_StudentId",
                table: "Absences",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServantClasses_Classes_ClassId",
                table: "ServantClasses",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop foreign key constraints first
            migrationBuilder.DropForeignKey(
                name: "FK_Absences_Students_StudentId",
                table: "Absences");

            migrationBuilder.DropForeignKey(
                name: "FK_ServantClasses_Classes_ClassId",
                table: "ServantClasses");

            // Drop indexes
            migrationBuilder.DropIndex(
                name: "IDX_AbsenceDate_StudentID",
                table: "Absences");

            migrationBuilder.DropIndex(
                name: "IX_Absences_StudentId",
                table: "Absences");

            migrationBuilder.DropIndex(
                name: "IX_ServantClasses_ClassId",
                table: "ServantClasses");

            migrationBuilder.DropColumn(
                name: "Number",
                table: "Classes");

            // Students table - Drop PK, drop column, add back int IDENTITY, re-add PK
            migrationBuilder.DropPrimaryKey(
                name: "PK_Students",
                table: "Students");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Students");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Students",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Students",
                table: "Students",
                column: "Id");

            // ServantClasses - Change ClassId back to int
            migrationBuilder.AlterColumn<int>(
                name: "ClassId",
                table: "ServantClasses",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.DropPrimaryKey(
                name: "PK_ServantClasses",
                table: "ServantClasses");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ServantClasses");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "ServantClasses",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.Sql("UPDATE ServantClasses SET Id = NEWID() WHERE Id = '00000000-0000-0000-0000-000000000000'");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServantClasses",
                table: "ServantClasses",
                column: "Id");

            // Otps - Drop PK, drop column, add back Guid, re-add PK
            migrationBuilder.DropPrimaryKey(
                name: "PK_Otps",
                table: "Otps");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Otps");

            migrationBuilder.AddColumn<Guid>(
                name: "Id",
                table: "Otps",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: Guid.Empty);

            migrationBuilder.Sql("UPDATE Otps SET Id = NEWID() WHERE Id = '00000000-0000-0000-0000-000000000000'");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Otps",
                table: "Otps",
                column: "Id");

            // Classes table - Drop PK, drop column, add back int IDENTITY, re-add PK
            migrationBuilder.DropPrimaryKey(
                name: "PK_Classes",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Classes");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Classes",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Classes",
                table: "Classes",
                column: "Id");

            // Absences table - Change StudentId back to int, drop PK, drop column, add back int IDENTITY, re-add PK
            migrationBuilder.AddColumn<int>(
                name: "StudentId_Temp",
                table: "Absences",
                type: "int",
                nullable: true);

            migrationBuilder.DropColumn(
                name: "StudentId",
                table: "Absences");

            migrationBuilder.RenameColumn(
                name: "StudentId_Temp",
                table: "Absences",
                newName: "StudentId");

            migrationBuilder.AlterColumn<int>(
                name: "StudentId",
                table: "Absences",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.DropPrimaryKey(
                name: "PK_Absences",
                table: "Absences");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "Absences");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "Absences",
                type: "int",
                nullable: false)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Absences",
                table: "Absences",
                column: "Id");

            // Re-create indexes
            migrationBuilder.CreateIndex(
                name: "IX_Absences_StudentId",
                table: "Absences",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IDX_AbsenceDate_StudentID",
                table: "Absences",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_ServantClasses_ClassId",
                table: "ServantClasses",
                column: "ClassId");

            // Re-add foreign key constraints
            migrationBuilder.AddForeignKey(
                name: "FK_Absences_Students_StudentId",
                table: "Absences",
                column: "StudentId",
                principalTable: "Students",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ServantClasses_Classes_ClassId",
                table: "ServantClasses",
                column: "ClassId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}