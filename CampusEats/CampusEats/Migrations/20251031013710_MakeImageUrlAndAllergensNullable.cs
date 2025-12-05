using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusEats.Migrations
{

    public partial class MakeImageUrlAndAllergensNullable : Migration
    {

        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DietaryRestrictions",
                table: "MenuItems");
        }


        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int[]>(
                name: "DietaryRestrictions",
                table: "MenuItems",
                type: "integer[]",
                nullable: false,
                defaultValue: new int[0]);
        }
    }
}
