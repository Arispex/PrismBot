using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrismBot.Migrations
{
    /// <inheritdoc />
    public partial class test : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "GroupName",
                table: "Players",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    GroupName = table.Column<string>(type: "TEXT", nullable: false),
                    ParentGroupName = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.GroupName);
                    table.ForeignKey(
                        name: "FK_Groups_Groups_ParentGroupName",
                        column: x => x.ParentGroupName,
                        principalTable: "Groups",
                        principalColumn: "GroupName");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Players_GroupName",
                table: "Players",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ParentGroupName",
                table: "Groups",
                column: "ParentGroupName");

            migrationBuilder.AddForeignKey(
                name: "FK_Players_Groups_GroupName",
                table: "Players",
                column: "GroupName",
                principalTable: "Groups",
                principalColumn: "GroupName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Players_Groups_GroupName",
                table: "Players");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropIndex(
                name: "IX_Players_GroupName",
                table: "Players");

            migrationBuilder.DropColumn(
                name: "GroupName",
                table: "Players");
        }
    }
}
