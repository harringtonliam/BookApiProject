using Microsoft.EntityFrameworkCore.Migrations;

namespace BookApiProject.Migrations
{
    public partial class InitialDatabaseCreation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authors_Countries_countryId",
                table: "Authors");

            migrationBuilder.RenameColumn(
                name: "Reviewtext",
                table: "Reviews",
                newName: "ReviewText");

            migrationBuilder.RenameColumn(
                name: "countryId",
                table: "Authors",
                newName: "CountryId");

            migrationBuilder.RenameIndex(
                name: "IX_Authors_countryId",
                table: "Authors",
                newName: "IX_Authors_CountryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_Countries_CountryId",
                table: "Authors",
                column: "CountryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Authors_Countries_CountryId",
                table: "Authors");

            migrationBuilder.RenameColumn(
                name: "ReviewText",
                table: "Reviews",
                newName: "Reviewtext");

            migrationBuilder.RenameColumn(
                name: "CountryId",
                table: "Authors",
                newName: "countryId");

            migrationBuilder.RenameIndex(
                name: "IX_Authors_CountryId",
                table: "Authors",
                newName: "IX_Authors_countryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Authors_Countries_countryId",
                table: "Authors",
                column: "countryId",
                principalTable: "Countries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
