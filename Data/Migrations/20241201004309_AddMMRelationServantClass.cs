using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyProject.Migrations
{
    /// <inheritdoc />
    public partial class AddMMRelationServantClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts");

            migrationBuilder.RenameTable(
                name: "Accounts",
                newName: "AccountVM");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AccountVM",
                table: "AccountVM",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "ServantClasses",
                columns: table => new
                {
                    ServantId = table.Column<int>(type: "int", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServantClasses", x => new { x.ServantId, x.ClassId });
                    table.ForeignKey(
                        name: "FK_ServantClasses_AccountVM_ServantId",
                        column: x => x.ServantId,
                        principalTable: "AccountVM",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ServantClasses_Classes_ClassId",
                        column: x => x.ClassId,
                        principalTable: "Classes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServantClasses_ClassId",
                table: "ServantClasses",
                column: "ClassId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServantClasses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AccountVM",
                table: "AccountVM");

            migrationBuilder.RenameTable(
                name: "AccountVM",
                newName: "Accounts");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Accounts",
                table: "Accounts",
                column: "Id");
        }
    }
}
