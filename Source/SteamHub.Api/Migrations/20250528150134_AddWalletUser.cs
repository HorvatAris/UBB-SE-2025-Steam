using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddWalletUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PointsBalance",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "WalletBalance",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Wallets",
                keyColumn: "WalletId",
                keyValue: 1,
                columns: new[] { "Balance", "Points" },
                values: new object[] { 500m, 6000 });

            migrationBuilder.UpdateData(
                table: "Wallets",
                keyColumn: "WalletId",
                keyValue: 2,
                columns: new[] { "Balance", "Points" },
                values: new object[] { 420m, 5000 });

            migrationBuilder.UpdateData(
                table: "Wallets",
                keyColumn: "WalletId",
                keyValue: 3,
                columns: new[] { "Balance", "Points" },
                values: new object[] { 390m, 5000 });

            migrationBuilder.UpdateData(
                table: "Wallets",
                keyColumn: "WalletId",
                keyValue: 4,
                columns: new[] { "Balance", "Points" },
                values: new object[] { 780m, 6000 });

            migrationBuilder.UpdateData(
                table: "Wallets",
                keyColumn: "WalletId",
                keyValue: 5,
                columns: new[] { "Balance", "Points" },
                values: new object[] { 5500m, 7000 });

            migrationBuilder.UpdateData(
                table: "Wallets",
                keyColumn: "WalletId",
                keyValue: 6,
                columns: new[] { "Balance", "Points" },
                values: new object[] { 950m, 6000 });

            migrationBuilder.UpdateData(
                table: "Wallets",
                keyColumn: "WalletId",
                keyValue: 7,
                columns: new[] { "Balance", "Points" },
                values: new object[] { 3300m, 4000 });

            migrationBuilder.UpdateData(
                table: "Wallets",
                keyColumn: "WalletId",
                keyValue: 8,
                columns: new[] { "Balance", "Points" },
                values: new object[] { 1100m, 5000 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<float>(
                name: "PointsBalance",
                table: "Users",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.AddColumn<float>(
                name: "WalletBalance",
                table: "Users",
                type: "real",
                nullable: false,
                defaultValue: 0f);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 1,
                columns: new[] { "PointsBalance", "WalletBalance" },
                values: new object[] { 6000f, 500f });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 2,
                columns: new[] { "PointsBalance", "WalletBalance" },
                values: new object[] { 5000f, 420f });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 3,
                columns: new[] { "PointsBalance", "WalletBalance" },
                values: new object[] { 5000f, 390f });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 4,
                columns: new[] { "PointsBalance", "WalletBalance" },
                values: new object[] { 6000f, 780f });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 5,
                columns: new[] { "PointsBalance", "WalletBalance" },
                values: new object[] { 7000f, 5500f });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 6,
                columns: new[] { "PointsBalance", "WalletBalance" },
                values: new object[] { 6000f, 950f });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 7,
                columns: new[] { "PointsBalance", "WalletBalance" },
                values: new object[] { 4000f, 3300f });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: 8,
                columns: new[] { "PointsBalance", "WalletBalance" },
                values: new object[] { 5000f, 1100f });

            migrationBuilder.UpdateData(
                table: "Wallets",
                keyColumn: "WalletId",
                keyValue: 1,
                columns: new[] { "Balance", "Points" },
                values: new object[] { 200m, 10 });

            migrationBuilder.UpdateData(
                table: "Wallets",
                keyColumn: "WalletId",
                keyValue: 2,
                columns: new[] { "Balance", "Points" },
                values: new object[] { 200m, 10 });

            migrationBuilder.UpdateData(
                table: "Wallets",
                keyColumn: "WalletId",
                keyValue: 3,
                columns: new[] { "Balance", "Points" },
                values: new object[] { 200m, 10 });

            migrationBuilder.UpdateData(
                table: "Wallets",
                keyColumn: "WalletId",
                keyValue: 4,
                columns: new[] { "Balance", "Points" },
                values: new object[] { 200m, 10 });

            migrationBuilder.UpdateData(
                table: "Wallets",
                keyColumn: "WalletId",
                keyValue: 5,
                columns: new[] { "Balance", "Points" },
                values: new object[] { 200m, 10 });

            migrationBuilder.UpdateData(
                table: "Wallets",
                keyColumn: "WalletId",
                keyValue: 6,
                columns: new[] { "Balance", "Points" },
                values: new object[] { 200m, 10 });

            migrationBuilder.UpdateData(
                table: "Wallets",
                keyColumn: "WalletId",
                keyValue: 7,
                columns: new[] { "Balance", "Points" },
                values: new object[] { 200m, 10 });

            migrationBuilder.UpdateData(
                table: "Wallets",
                keyColumn: "WalletId",
                keyValue: 8,
                columns: new[] { "Balance", "Points" },
                values: new object[] { 200m, 10 });
        }
    }
}
