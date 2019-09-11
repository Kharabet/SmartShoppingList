using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ShoppingListArduino.Data.Migrations
{
    public partial class AddRfid : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userproduct_AspNetUsers_UserId",
                table: "userproduct");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userproduct",
                table: "userproduct");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "userproduct",
                nullable: true,
                oldClrType: typeof(string));

            migrationBuilder.AddColumn<int>(
                name: "Id",
                table: "userproduct",
                nullable: false,
                defaultValue: 0)
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddPrimaryKey(
                name: "PK_userproduct",
                table: "userproduct",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "userproductrfids",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    UserProductId = table.Column<int>(nullable: false),
                    Rfid = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_userproductrfids", x => x.Id);
                    table.ForeignKey(
                        name: "FK_userproductrfids_userproduct_UserProductId",
                        column: x => x.UserProductId,
                        principalTable: "userproduct",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_userproduct_UserId",
                table: "userproduct",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_userproductrfids_UserProductId",
                table: "userproductrfids",
                column: "UserProductId");

            migrationBuilder.AddForeignKey(
                name: "FK_userproduct_AspNetUsers_UserId",
                table: "userproduct",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_userproduct_AspNetUsers_UserId",
                table: "userproduct");

            migrationBuilder.DropTable(
                name: "userproductrfids");

            migrationBuilder.DropPrimaryKey(
                name: "PK_userproduct",
                table: "userproduct");

            migrationBuilder.DropIndex(
                name: "IX_userproduct_UserId",
                table: "userproduct");

            migrationBuilder.DropColumn(
                name: "Id",
                table: "userproduct");

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "userproduct",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_userproduct",
                table: "userproduct",
                columns: new[] { "UserId", "ProductId" });

            migrationBuilder.AddForeignKey(
                name: "FK_userproduct_AspNetUsers_UserId",
                table: "userproduct",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
