using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleAPI.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class mign12 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductPriceTypes_Products_ProductId",
                table: "ProductPriceTypes");

            migrationBuilder.DropIndex(
                name: "IX_ProductPriceTypes_ProductId",
                table: "ProductPriceTypes");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "ProductPriceTypes");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "ProductPriceTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductPriceTypes_ProductId",
                table: "ProductPriceTypes",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductPriceTypes_Products_ProductId",
                table: "ProductPriceTypes",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
