using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrismBot.Migrations
{
    /// <inheritdoc />
    public partial class test2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Permission",
                columns: table => new
                {
                    PermissionName = table.Column<string>(type: "TEXT", nullable: false),
                    GroupName = table.Column<string>(type: "TEXT", nullable: true),
                    PlayerQQ = table.Column<long>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Permission", x => x.PermissionName);
                    table.ForeignKey(
                        name: "FK_Permission_Groups_GroupName",
                        column: x => x.GroupName,
                        principalTable: "Groups",
                        principalColumn: "GroupName");
                    table.ForeignKey(
                        name: "FK_Permission_Players_PlayerQQ",
                        column: x => x.PlayerQQ,
                        principalTable: "Players",
                        principalColumn: "QQ");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Permission_GroupName",
                table: "Permission",
                column: "GroupName");

            migrationBuilder.CreateIndex(
                name: "IX_Permission_PlayerQQ",
                table: "Permission",
                column: "PlayerQQ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Permission");
        }
    }
}
