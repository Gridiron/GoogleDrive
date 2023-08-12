using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoogleDrive.Database.Migrations
{
    public partial class RemoveChunks : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Chunks");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Chunks",
                columns: table => new
                {
                    ChunkHash = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    FileId = table.Column<int>(type: "int", nullable: false),
                    BlobStorageUrl = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Chunks", x => x.ChunkHash);
                    table.UniqueConstraint("AK_Chunks_BlobStorageUrl", x => x.BlobStorageUrl);
                    table.ForeignKey(
                        name: "FK_Chunks_Files_FileId",
                        column: x => x.FileId,
                        principalTable: "Files",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Chunks_FileId",
                table: "Chunks",
                column: "FileId");
        }
    }
}
