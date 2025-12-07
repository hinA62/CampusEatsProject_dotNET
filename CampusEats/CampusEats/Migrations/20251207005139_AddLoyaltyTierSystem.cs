using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusEats.Migrations
{
    /// <inheritdoc />
    public partial class AddLoyaltyTierSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CurrentTier",
                table: "LoyaltyAccounts",
                type: "text",
                nullable: false,
                defaultValue: "Bronze");

            migrationBuilder.AddColumn<int>(
                name: "TotalPointsEarned",
                table: "LoyaltyAccounts",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CurrentTier",
                table: "LoyaltyAccounts");

            migrationBuilder.DropColumn(
                name: "TotalPointsEarned",
                table: "LoyaltyAccounts");
        }
    }
}
