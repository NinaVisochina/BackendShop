using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BackendShop.Migrations
{
    /// <inheritdoc />
    public partial class add2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tblSubCategories",
                newName: "SubCategoryId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tblProducts",
                newName: "ProductId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "tblCategories",
                newName: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SubCategoryId",
                table: "tblSubCategories",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "ProductId",
                table: "tblProducts",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "CategoryId",
                table: "tblCategories",
                newName: "Id");
        }
    }
}
