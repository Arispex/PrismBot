using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PrismBot.SDK.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    GroupName = table.Column<string>(type: "TEXT", nullable: false),
                    ParentGroupName = table.Column<string>(type: "TEXT", nullable: true),
                    Permissions = table.Column<string>(type: "TEXT", nullable: false)
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

            migrationBuilder.CreateTable(
                name: "Servers",
                columns: table => new
                {
                    Identity = table.Column<string>(type: "TEXT", nullable: false),
                    ServerName = table.Column<string>(type: "TEXT", nullable: false),
                    Host = table.Column<string>(type: "TEXT", nullable: false),
                    Port = table.Column<int>(type: "INTEGER", nullable: false),
                    Token = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Servers", x => x.Identity);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    QQ = table.Column<long>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    Coins = table.Column<long>(type: "INTEGER", nullable: false),
                    GroupName = table.Column<string>(type: "TEXT", nullable: true),
                    Permissions = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.QQ);
                    table.ForeignKey(
                        name: "FK_Players_Groups_GroupName",
                        column: x => x.GroupName,
                        principalTable: "Groups",
                        principalColumn: "GroupName");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ParentGroupName",
                table: "Groups",
                column: "ParentGroupName");

            migrationBuilder.CreateIndex(
                name: "IX_Players_GroupName",
                table: "Players",
                column: "GroupName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Players");

            migrationBuilder.DropTable(
                name: "Servers");

            migrationBuilder.DropTable(
                name: "Groups");
        }
    }
}
