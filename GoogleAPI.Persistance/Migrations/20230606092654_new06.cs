using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleAPI.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class new06 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Variations_VariationId",
                table: "Products");

            migrationBuilder.DropTable(
                name: "ColorVariation");

            migrationBuilder.DropTable(
                name: "DimentionVariation");

            migrationBuilder.DropTable(
                name: "Variations");

            migrationBuilder.DropIndex(
                name: "IX_Products_VariationId",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "StockCode",
                table: "ProductStocks");

            migrationBuilder.DropColumn(
                name: "VariationId",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "StockCode",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ColorProduct");

            migrationBuilder.DropTable(
                name: "DimentionProduct");

            migrationBuilder.DropColumn(
                name: "StockCode",
                table: "Products");

            migrationBuilder.AddColumn<string>(
                name: "StockCode",
                table: "ProductStocks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "VariationId",
                table: "Products",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "Variations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Variations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ColorVariation",
                columns: table => new
                {
                    ColorsId = table.Column<int>(type: "int", nullable: false),
                    VariationsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ColorVariation", x => new { x.ColorsId, x.VariationsId });
                    table.ForeignKey(
                        name: "FK_ColorVariation_Colors_ColorsId",
                        column: x => x.ColorsId,
                        principalTable: "Colors",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ColorVariation_Variations_VariationsId",
                        column: x => x.VariationsId,
                        principalTable: "Variations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DimentionVariation",
                columns: table => new
                {
                    DimentionsId = table.Column<int>(type: "int", nullable: false),
                    VariationsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DimentionVariation", x => new { x.DimentionsId, x.VariationsId });
                    table.ForeignKey(
                        name: "FK_DimentionVariation_Dimentions_DimentionsId",
                        column: x => x.DimentionsId,
                        principalTable: "Dimentions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DimentionVariation_Variations_VariationsId",
                        column: x => x.VariationsId,
                        principalTable: "Variations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Products_VariationId",
                table: "Products",
                column: "VariationId");

            migrationBuilder.CreateIndex(
                name: "IX_ColorVariation_VariationsId",
                table: "ColorVariation",
                column: "VariationsId");

            migrationBuilder.CreateIndex(
                name: "IX_DimentionVariation_VariationsId",
                table: "DimentionVariation",
                column: "VariationsId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Variations_VariationId",
                table: "Products",
                column: "VariationId",
                principalTable: "Variations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
