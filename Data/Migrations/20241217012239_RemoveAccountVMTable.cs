using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyProject.Migrations
{
    /// <inheritdoc />
    public partial class RemoveAccountVMTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_ServantClasses_AccountVM_ServantId",
                table: "ServantClasses");

            // Drop the primary key on ServantClasses
            migrationBuilder.DropPrimaryKey(
                name: "PK_ServantClasses",
                table: "ServantClasses");

            // Alter the column type of ServantId
            migrationBuilder.AlterColumn<string>(
                name: "ServantId",
                table: "ServantClasses",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            // Recreate the primary key
            migrationBuilder.AddPrimaryKey(
                name: "PK_ServantClasses",
                table: "ServantClasses",
                columns: new[] { "ServantId", "ClassId" });

            // Add the foreign key constraint referencing AspNetUsers
            migrationBuilder.AddForeignKey(
                name: "FK_ServantClasses_AspNetUsers_ServantId",
                table: "ServantClasses",
                column: "ServantId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            // Drop the AccountVM table
            migrationBuilder.DropTable(
                name: "AccountVM");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // Drop the foreign key constraint
            migrationBuilder.DropForeignKey(
                name: "FK_ServantClasses_AspNetUsers_ServantId",
                table: "ServantClasses");

            // Drop the primary key
            migrationBuilder.DropPrimaryKey(
                name: "PK_ServantClasses",
                table: "ServantClasses");

            // Revert the column type of ServantId back to int
            migrationBuilder.AlterColumn<int>(
                name: "ServantId",
                table: "ServantClasses",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            // Recreate the original primary key
            migrationBuilder.AddPrimaryKey(
                name: "PK_ServantClasses",
                table: "ServantClasses",
                columns: new[] { "ServantId", "ClassId" });

            // Recreate the AccountVM table
            migrationBuilder.CreateTable(
                name: "AccountVM",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountVM", x => x.Id);
                });

            // Re-add the foreign key constraint to AccountVM
            migrationBuilder.AddForeignKey(
                name: "FK_ServantClasses_AccountVM_ServantId",
                table: "ServantClasses",
                column: "ServantId",
                principalTable: "AccountVM",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }


    }
}
