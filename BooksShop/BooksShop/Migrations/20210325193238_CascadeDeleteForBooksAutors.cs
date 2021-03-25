using Microsoft.EntityFrameworkCore.Migrations;

namespace BooksShop.Migrations
{
    public partial class CascadeDeleteForBooksAutors : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Books_Aut__id_au__3C69FB99",
                table: "Books_Autors");

            migrationBuilder.DropForeignKey(
                name: "fk_books_to_autors",
                table: "Books_Autors");

            migrationBuilder.AddForeignKey(
                name: "FK__Books_Aut__id_au__3C69FB99",
                table: "Books_Autors",
                column: "id_autor",
                principalTable: "Autors",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "fk_books_to_autors",
                table: "Books_Autors",
                column: "id_book",
                principalTable: "Books",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK__Books_Aut__id_au__3C69FB99",
                table: "Books_Autors");

            migrationBuilder.DropForeignKey(
                name: "fk_books_to_autors",
                table: "Books_Autors");

            migrationBuilder.AddForeignKey(
                name: "FK__Books_Aut__id_au__3C69FB99",
                table: "Books_Autors",
                column: "id_autor",
                principalTable: "Autors",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "fk_books_to_autors",
                table: "Books_Autors",
                column: "id_book",
                principalTable: "Books",
                principalColumn: "id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
