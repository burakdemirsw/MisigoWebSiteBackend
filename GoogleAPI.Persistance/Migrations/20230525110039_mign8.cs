using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleAPI.Persistance.Migrations
{
    /// <inheritdoc />
    public partial class mign8 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categorys_Photos_PhotoId",
                table: "Categorys");

            migrationBuilder.DropForeignKey(
                name: "FK_Categorys_ProductCards_ProductCardId",
                table: "Categorys");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCards_Categorys_CategoryId",
                table: "ProductCards");

            migrationBuilder.DropForeignKey(
                name: "FK_SubCategorys_Categorys_CategoryId",
                table: "SubCategorys");

            migrationBuilder.DropForeignKey(
                name: "FK_SubCategorys_ProductCards_ProductCardId",
                table: "SubCategorys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubCategorys",
                table: "SubCategorys");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categorys",
                table: "Categorys");

            migrationBuilder.DropIndex(
                name: "IX_Categorys_PhotoId",
                table: "Categorys");

            migrationBuilder.DropColumn(
                name: "PhotoId",
                table: "Categorys");

            migrationBuilder.RenameTable(
                name: "SubCategorys",
                newName: "SubCategories");

            migrationBuilder.RenameTable(
                name: "Categorys",
                newName: "Categories");

            migrationBuilder.RenameIndex(
                name: "IX_SubCategorys_ProductCardId",
                table: "SubCategories",
                newName: "IX_SubCategories_ProductCardId");

            migrationBuilder.RenameIndex(
                name: "IX_SubCategorys_CategoryId",
                table: "SubCategories",
                newName: "IX_SubCategories_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Categorys_ProductCardId",
                table: "Categories",
                newName: "IX_Categories_ProductCardId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubCategories",
                table: "SubCategories",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categories",
                table: "Categories",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "CategoryPhoto",
                columns: table => new
                {
                    CategoriesId = table.Column<int>(type: "int", nullable: false),
                    PhotosId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CategoryPhoto", x => new { x.CategoriesId, x.PhotosId });
                    table.ForeignKey(
                        name: "FK_CategoryPhoto_Categories_CategoriesId",
                        column: x => x.CategoriesId,
                        principalTable: "Categories",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_CategoryPhoto_Photos_PhotosId",
                        column: x => x.PhotosId,
                        principalTable: "Photos",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CategoryPhoto_PhotosId",
                table: "CategoryPhoto",
                column: "PhotosId");

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
                onDelete: ReferentialAction.NoAction);

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategories_Categories_CategoryId",
                table: "SubCategories",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategories_ProductCards_ProductCardId",
                table: "SubCategories",
                column: "ProductCardId",
                principalTable: "ProductCards",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_ProductCards_ProductCardId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductCards_Categories_CategoryId",
                table: "ProductCards");

            migrationBuilder.DropForeignKey(
                name: "FK_SubCategories_Categories_CategoryId",
                table: "SubCategories");

            migrationBuilder.DropForeignKey(
                name: "FK_SubCategories_ProductCards_ProductCardId",
                table: "SubCategories");

            migrationBuilder.DropTable(
                name: "CategoryPhoto");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SubCategories",
                table: "SubCategories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Categories",
                table: "Categories");

            migrationBuilder.RenameTable(
                name: "SubCategories",
                newName: "SubCategorys");

            migrationBuilder.RenameTable(
                name: "Categories",
                newName: "Categorys");

            migrationBuilder.RenameIndex(
                name: "IX_SubCategories_ProductCardId",
                table: "SubCategorys",
                newName: "IX_SubCategorys_ProductCardId");

            migrationBuilder.RenameIndex(
                name: "IX_SubCategories_CategoryId",
                table: "SubCategorys",
                newName: "IX_SubCategorys_CategoryId");

            migrationBuilder.RenameIndex(
                name: "IX_Categories_ProductCardId",
                table: "Categorys",
                newName: "IX_Categorys_ProductCardId");

            migrationBuilder.AddColumn<int>(
                name: "PhotoId",
                table: "Categorys",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SubCategorys",
                table: "SubCategorys",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Categorys",
                table: "Categorys",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "IX_Categorys_PhotoId",
                table: "Categorys",
                column: "PhotoId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Categorys_Photos_PhotoId",
                table: "Categorys",
                column: "PhotoId",
                principalTable: "Photos",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Categorys_ProductCards_ProductCardId",
                table: "Categorys",
                column: "ProductCardId",
                principalTable: "ProductCards",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductCards_Categorys_CategoryId",
                table: "ProductCards",
                column: "CategoryId",
                principalTable: "Categorys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategorys_Categorys_CategoryId",
                table: "SubCategorys",
                column: "CategoryId",
                principalTable: "Categorys",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SubCategorys_ProductCards_ProductCardId",
                table: "SubCategorys",
                column: "ProductCardId",
                principalTable: "ProductCards",
                principalColumn: "Id");
        }
    }
}
