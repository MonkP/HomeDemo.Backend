using Microsoft.EntityFrameworkCore.Migrations;

namespace HomeProject.Backend.Models.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 64, nullable: false),
                    UserCode = table.Column<string>(maxLength: 64, nullable: false),
                    DisplayName = table.Column<string>(maxLength: 64, nullable: true),
                    PasswordMD5 = table.Column<string>(maxLength: 32, nullable: false),
                    PasswordSHA1 = table.Column<string>(maxLength: 64, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
