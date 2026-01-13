using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitchFighter.API.Migrations
{
    /// <inheritdoc />
    public partial class AddUserDisplayConfig : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LayoutMode",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "ShowDamageNumbers",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "ShowHUD",
                table: "Users",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LayoutMode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ShowDamageNumbers",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ShowHUD",
                table: "Users");
        }
    }
}
