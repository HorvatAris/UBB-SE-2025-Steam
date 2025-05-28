using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SteamHub.Api.Migrations
{
    /// <inheritdoc />
    public partial class wallet1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
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
