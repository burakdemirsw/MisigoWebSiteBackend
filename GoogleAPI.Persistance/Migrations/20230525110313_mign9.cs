using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleAPI.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class mign9 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_ProductCards_ProductCardId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCards_Categories_CategoryId",
                table: "ProductCards");

            migrationBuilder.DropIndex(
                name: "IX_ProductCards_CategoryId",
                table: "ProductCards");

            migrationBuilder.DropIndex(
                name: "IX_Categories_ProductCardId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "ProductCards");

            migrationBuilder.DropColumn(
                name: "ProductCardId",
                table: "Categories");

            migrationBuilder.CreateTable(
                name: "CategoryProductCard",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "int", nullable: false),
                    ProductCardsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryProductCard", x => new { x.CategoriesId, x.ProductCardsId });
                    table.ForeignKey(
                        name: "FK_CategoryProductCard_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryProductCard_ProductCards_ProductCardsId",
                        column: x => x.ProductCardsId,
                        principalTable: "ProductCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryProductCard_ProductCardsId",
                table: "CategoryProductCard",
                column: "ProductCardsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CategoryProductCard");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "ProductCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductCardId",
                table: "Categories",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCards_CategoryId",
                table: "ProductCards",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_ProductCardId",
                table: "Categories",
                column: "ProductCardId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_ProductCards_ProductCardId",
                table: "Categories",
                column: "ProductCardId",
                principalTable: "ProductCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCards_Categories_CategoryId",
                table: "ProductCards",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
