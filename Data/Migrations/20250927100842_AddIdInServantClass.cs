using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyProject.Migrations
{
    /// <inheritdoc />
    public partial class AddIdInServantClass : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ServantClasses",
                table: "ServantClasses");

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "ServantClasses",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServantClasses",
                table: "ServantClasses",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_ServantClasses_ServantId",
                table: "ServantClasses",
                column: "ServantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ServantClasses",
                table: "ServantClasses");

            migrationBuilder.DropIndex(
                name: "IX_ServantClasses_ServantId",
                table: "ServantClasses");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "ServantClasses");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ServantClasses",
                table: "ServantClasses",
                columns: new[] { "ServantId", "ClassId" });
        }
    }
}
