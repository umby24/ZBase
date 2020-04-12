using Microsoft.EntityFrameworkCore.Migrations;

namespace ZBase.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "IpBans",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ip = table.Column<string>(nullable: true),
                    Reason = table.Column<string>(nullable: true),
                    BannedBy = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IpBans", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Players",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(nullable: true),
                    Ip = table.Column<string>(nullable: true),
                    Rank = table.Column<short>(nullable: false),
                    GlobalChat = table.Column<bool>(nullable: false),
                    Stopped = table.Column<bool>(nullable: false),
                    Vanished = table.Column<bool>(nullable: false),
                    BoundBlock = table.Column<int>(nullable: false),
                    TimeMuted = table.Column<double>(nullable: false),
                    BannedUntil = table.Column<double>(nullable: false),
                    Banned = table.Column<bool>(nullable: false),
                    BannedBy = table.Column<string>(nullable: true),
                    BanMessage = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Players", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IpBans");

            migrationBuilder.DropTable(
                name: "Players");
        }
    }
}
