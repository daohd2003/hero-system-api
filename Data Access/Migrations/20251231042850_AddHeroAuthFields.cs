using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data_Access.Migrations
{
    /// <inheritdoc />
    public partial class AddHeroAuthFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "SuperPower",
                table: "Hero",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<string>(
                name: "PasswordHash",
                table: "Hero",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "RefreshToken",
                table: "Hero",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "RefreshTokenExpiryTime",
                table: "Hero",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Hero",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                columns: new[] { "PasswordHash", "RefreshToken", "RefreshTokenExpiryTime" },
                values: new object[] { "123456", null, null });

            migrationBuilder.UpdateData(
                table: "Hero",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                columns: new[] { "PasswordHash", "RefreshToken", "RefreshTokenExpiryTime" },
                values: new object[] { "123456", null, null });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PasswordHash",
                table: "Hero");

            migrationBuilder.DropColumn(
                name: "RefreshToken",
                table: "Hero");

            migrationBuilder.DropColumn(
                name: "RefreshTokenExpiryTime",
                table: "Hero");

            migrationBuilder.AlterColumn<string>(
                name: "SuperPower",
                table: "Hero",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(100)",
                oldMaxLength: 100);
        }
    }
}
