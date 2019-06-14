using Microsoft.EntityFrameworkCore.Migrations;

namespace ShoppingListArduino.Data.Migrations
{
    public partial class ProductsIdAutoIncrement : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserProduct_Products_ProductId",
                table: "UserProduct");

            migrationBuilder.DropForeignKey(
                name: "FK_UserProduct_AspNetUsers_UserId",
                table: "UserProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserProduct",
                table: "UserProduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Products",
                table: "Products");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "UserProduct",
                newName: "userproduct");

            migrationBuilder.RenameTable(
                name: "Products",
                newName: "products");

            migrationBuilder.RenameIndex(
                name: "IX_UserProduct_ProductId",
                table: "userproduct",
                newName: "IX_userproduct_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_userproduct",
                table: "userproduct",
                columns: new[] { "UserId", "ProductId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_products",
                table: "products",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_userproduct_products_ProductId",
                table: "userproduct",
                column: "ProductId",
                principalTable: "products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_userproduct_AspNetUsers_UserId",
                table: "userproduct",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userproduct_products_ProductId",
                table: "userproduct");

            migrationBuilder.DropForeignKey(
                name: "FK_userproduct_AspNetUsers_UserId",
                table: "userproduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userproduct",
                table: "userproduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_products",
                table: "products");

            migrationBuilder.DropIndex(
                name: "UserNameIndex",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles");

            migrationBuilder.RenameTable(
                name: "userproduct",
                newName: "UserProduct");

            migrationBuilder.RenameTable(
                name: "products",
                newName: "Products");

            migrationBuilder.RenameIndex(
                name: "IX_userproduct_ProductId",
                table: "UserProduct",
                newName: "IX_UserProduct_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserProduct",
                table: "UserProduct",
                columns: new[] { "UserId", "ProductId" });

            migrationBuilder.AddPrimaryKey(
                name: "PK_Products",
                table: "Products",
                column: "Id");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                table: "AspNetUsers",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                table: "AspNetRoles",
                column: "NormalizedName",
                unique: true,
                filter: "[NormalizedName] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_UserProduct_Products_ProductId",
                table: "UserProduct",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserProduct_AspNetUsers_UserId",
                table: "UserProduct",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
