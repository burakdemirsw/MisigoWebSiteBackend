using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleAPI.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class new08 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductSeoDetails_ProductCards_ProductCardId",
                table: "ProductSeoDetails");

            migrationBuilder.DropIndex(
                name: "IX_ProductSeoDetails_ProductCardId",
                table: "ProductSeoDetails");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProductSeoDetails_ProductCardId",
                table: "ProductSeoDetails",
                column: "ProductCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductSeoDetails_ProductCards_ProductCardId",
                table: "ProductSeoDetails",
                column: "ProductCardId",
                principalTable: "ProductCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
