using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AuthenticationService.Data.Migrations
{
    public partial class ExternalLoginParameters : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ExternalLoginProvider",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ExternalLoginProviderKey",
                table: "AspNetUsers",
                type: "nvarchar(max)",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExternalLoginProvider",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ExternalLoginProviderKey",
                table: "AspNetUsers");
        }
    }
}
