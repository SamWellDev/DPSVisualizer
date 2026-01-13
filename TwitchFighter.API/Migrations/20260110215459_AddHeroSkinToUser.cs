using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitchFighter.API.Migrations
{
    /// <inheritdoc />
    public partial class AddHeroSkinToUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "HeroSkin",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HeroSkin",
                table: "Users");
        }
    }
}
