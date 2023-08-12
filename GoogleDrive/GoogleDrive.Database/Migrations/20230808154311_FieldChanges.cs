using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleDrive.Database.Migrations
{
    public partial class FieldChanges : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Files_Path",
                table: "Files");

            migrationBuilder.AddColumn<string>(
                name: "BlobStorageUrl",
                table: "FileVersionChunks",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "ChunkNumber",
                table: "FileVersionChunks",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Files_Id_Path",
                table: "Files",
                columns: new[] { "Id", "Path" },
                unique: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Files_Id_Path",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "BlobStorageUrl",
                table: "FileVersionChunks");

            migrationBuilder.DropColumn(
                name: "ChunkNumber",
                table: "FileVersionChunks");

            migrationBuilder.CreateIndex(
                name: "IX_Files_Path",
                table: "Files",
                column: "Path",
                unique: true);
        }
    }
}
