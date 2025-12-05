using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusEats.Migrations
{

    public partial class AddDietaryRestrictionsToMenuItem : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int[]>(
                name: "DietaryRestrictions",
                table: "MenuItems",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DietaryRestrictions",
                table: "MenuItems");
        }
    }
}
