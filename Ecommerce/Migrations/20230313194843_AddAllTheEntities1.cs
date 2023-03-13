using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Ecommerce.Migrations
{
    /// <inheritdoc />
    public partial class AddAllTheEntities1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Images_Products_ProductId",
                table: "Images");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Images",
                table: "Images");

            migrationBuilder.RenameTable(
                name: "Images",
                newName: "MyImages");

            migrationBuilder.RenameIndex(
                name: "IX_Images_ProductId",
                table: "MyImages",
                newName: "IX_MyImages_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_MyImages",
                table: "MyImages",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_MyImages_Products_ProductId",
                table: "MyImages",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MyImages_Products_ProductId",
                table: "MyImages");

            migrationBuilder.DropPrimaryKey(
                name: "PK_MyImages",
                table: "MyImages");

            migrationBuilder.RenameTable(
                name: "MyImages",
                newName: "Images");

            migrationBuilder.RenameIndex(
                name: "IX_MyImages_ProductId",
                table: "Images",
                newName: "IX_Images_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Images",
                table: "Images",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Images_Products_ProductId",
                table: "Images",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");
        }
    }
}
