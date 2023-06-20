using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleAPI.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class new03 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductProperty_ProductCards_ProductCardId",
                table: "ProductProperty");

            migrationBuilder.DropIndex(
                name: "IX_ProductProperty_ProductCardId",
                table: "ProductProperty");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_ProductProperty_ProductCardId",
                table: "ProductProperty",
                column: "ProductCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductProperty_ProductCards_ProductCardId",
                table: "ProductProperty",
                column: "ProductCardId",
                principalTable: "ProductCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
