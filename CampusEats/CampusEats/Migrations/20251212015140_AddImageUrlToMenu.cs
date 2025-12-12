using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CampusEats.Migrations
{
    /// <inheritdoc />
    public partial class AddImageUrlToMenu : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                DO $$
                BEGIN
                    IF NOT EXISTS (
                        SELECT 1 
                        FROM information_schema.columns 
                        WHERE table_name = 'Menus' 
                        AND column_name = 'ImageUrl'
                    ) THEN
                        ALTER TABLE ""Menus"" ADD COLUMN ""ImageUrl"" text NULL;
                    END IF;
                END $$;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "Menus");
        }
    }
}
