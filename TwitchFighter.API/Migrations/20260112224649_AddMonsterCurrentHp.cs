using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TwitchFighter.API.Migrations
{
    /// <inheritdoc />
    public partial class AddMonsterCurrentHp : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "MonsterCurrentHp",
                table: "Progresses",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MonsterCurrentHp",
                table: "Progresses");
        }
    }
}
