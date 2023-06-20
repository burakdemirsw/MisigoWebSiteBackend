using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleAPI.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class mign7 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductCards_Photos_PhotoId",
                table: "ProductCards");

            migrationBuilder.DropIndex(
                name: "IX_ProductCards_PhotoId",
                table: "ProductCards");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "ProductCards");

            migrationBuilder.CreateTable(
                name: "PhotoProductCard",
                columns: table => new
                {
                    PhotoId = table.Column<int>(type: "int", nullable: false),
                    ProductCardsId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PhotoProductCard", x => new { x.PhotoId, x.ProductCardsId });
                    table.ForeignKey(
                        name: "FK_PhotoProductCard_Photos_PhotoId",
                        column: x => x.PhotoId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PhotoProductCard_ProductCards_ProductCardsId",
                        column: x => x.ProductCardsId,
                        principalTable: "ProductCards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.NoAction);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PhotoProductCard_ProductCardsId",
                table: "PhotoProductCard",
                column: "ProductCardsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PhotoProductCard");

            migrationBuilder.AddColumn<int>(
                name: "PhotoId",
                table: "ProductCards",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_ProductCards_PhotoId",
                table: "ProductCards",
                column: "PhotoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCards_Photos_PhotoId",
                table: "ProductCards",
                column: "PhotoId",
                principalTable: "Photos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
