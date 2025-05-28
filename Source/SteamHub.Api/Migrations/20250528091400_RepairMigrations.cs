using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class RepairMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewsPostRatingTypes_NewsPosts_PostId",
                table: "NewsPostRatingTypes");

            migrationBuilder.AddColumn<int>(
                name: "PostId1",
                table: "NewsComments",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_NewsComments_PostId1",
                table: "NewsComments",
                column: "PostId1");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsComments_NewsPosts_PostId1",
                table: "NewsComments",
                column: "PostId1",
                principalTable: "NewsPosts",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsPostRatingTypes_NewsPosts_PostId",
                table: "NewsPostRatingTypes",
                column: "PostId",
                principalTable: "NewsPosts",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_NewsComments_NewsPosts_PostId1",
                table: "NewsComments");

            migrationBuilder.DropForeignKey(
                name: "FK_NewsPostRatingTypes_NewsPosts_PostId",
                table: "NewsPostRatingTypes");

            migrationBuilder.DropIndex(
                name: "IX_NewsComments_PostId1",
                table: "NewsComments");

            migrationBuilder.DropColumn(
                name: "PostId1",
                table: "NewsComments");

            migrationBuilder.AddForeignKey(
                name: "FK_NewsPostRatingTypes_NewsPosts_PostId",
                table: "NewsPostRatingTypes",
                column: "PostId",
                principalTable: "NewsPosts",
                principalColumn: "Id");
        }
    }
}
