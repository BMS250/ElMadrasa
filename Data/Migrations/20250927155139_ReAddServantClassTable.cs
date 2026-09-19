using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MyProject.Migrations
{
    /// <inheritdoc />
    public partial class ReAddServantClassTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ServantClasses",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ServantId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ClassId = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServantClasses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServantClasses_AspNetUsers_ServantId",
                        column: x => x.ServantId,
                        principalTable: "AspNetUsers",
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

            migrationBuilder.CreateIndex(
                name: "IX_ServantClasses_ServantId",
                table: "ServantClasses",
                column: "ServantId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServantClasses");
        }
    }
}
