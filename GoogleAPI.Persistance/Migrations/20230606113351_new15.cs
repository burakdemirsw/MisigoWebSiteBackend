using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleAPI.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class new15 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColorProduct");

            migrationBuilder.DropTable(
                name: "DimentionProduct");

            migrationBuilder.DropColumn(
                name: "Dimention",
                table: "ProductStocks");

            migrationBuilder.AddColumn<int>(
                name: "ColorID",
                table: "ProductStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "DimentionID",
                table: "ProductStocks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Dimentions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ProductId",
                table: "Colors",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_ColorID",
                table: "ProductStocks",
                column: "ColorID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductStocks_DimentionID",
                table: "ProductStocks",
                column: "DimentionID",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Dimentions_ProductId",
                table: "Dimentions",
                column: "ProductId");

            migrationBuilder.CreateIndex(
                name: "IX_Colors_ProductId",
                table: "Colors",
                column: "ProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_Colors_Products_ProductId",
                table: "Colors",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Dimentions_Products_ProductId",
                table: "Dimentions",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStocks_Colors_ColorID",
                table: "ProductStocks",
                column: "ColorID",
                principalTable: "Colors",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductStocks_Dimentions_DimentionID",
                table: "ProductStocks",
                column: "DimentionID",
                principalTable: "Dimentions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Colors_Products_ProductId",
                table: "Colors");

            migrationBuilder.DropForeignKey(
                name: "FK_Dimentions_Products_ProductId",
                table: "Dimentions");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductStocks_Colors_ColorID",
                table: "ProductStocks");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductStocks_Dimentions_DimentionID",
                table: "ProductStocks");

            migrationBuilder.DropIndex(
                name: "IX_ProductStocks_ColorID",
                table: "ProductStocks");

            migrationBuilder.DropIndex(
                name: "IX_ProductStocks_DimentionID",
                table: "ProductStocks");

            migrationBuilder.DropIndex(
                name: "IX_Dimentions_ProductId",
                table: "Dimentions");

            migrationBuilder.DropIndex(
                name: "IX_Colors_ProductId",
                table: "Colors");

            migrationBuilder.DropColumn(
                name: "ColorID",
                table: "ProductStocks");

            migrationBuilder.DropColumn(
                name: "DimentionID",
                table: "ProductStocks");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Dimentions");

            migrationBuilder.DropColumn(
                name: "ProductId",
                table: "Colors");

            migrationBuilder.AddColumn<string>(
                name: "Dimention",
                table: "ProductStocks",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ColorProduct",
                columns: table => new
                {
                    ColorsId = table.Column<int>(type: "int", nullable: false),
                    ProductsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorProduct", x => new { x.ColorsId, x.ProductsId });
                    table.ForeignKey(
                        name: "FK_ColorProduct_Colors_ColorsId",
                        column: x => x.ColorsId,
                        principalTable: "Colors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ColorProduct_Products_ProductsId",
                        column: x => x.ProductsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DimentionProduct",
                columns: table => new
                {
                    DimentionsId = table.Column<int>(type: "int", nullable: false),
                    ProdutcsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimentionProduct", x => new { x.DimentionsId, x.ProdutcsId });
                    table.ForeignKey(
                        name: "FK_DimentionProduct_Dimentions_DimentionsId",
                        column: x => x.DimentionsId,
                        principalTable: "Dimentions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DimentionProduct_Products_ProdutcsId",
                        column: x => x.ProdutcsId,
                        principalTable: "Products",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ColorProduct_ProductsId",
                table: "ColorProduct",
                column: "ProductsId");

            migrationBuilder.CreateIndex(
                name: "IX_DimentionProduct_ProdutcsId",
                table: "DimentionProduct",
                column: "ProdutcsId");
        }
    }
}
