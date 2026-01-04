using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data_Access.Migrations
{
    /// <inheritdoc />
    public partial class AddHeroLastActiveAt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastActiveAt",
                table: "Hero",
                type: "datetime2",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Hero",
                keyColumn: "Id",
                keyValue: new Guid("11111111-1111-1111-1111-111111111111"),
                column: "LastActiveAt",
                value: null);

            migrationBuilder.UpdateData(
                table: "Hero",
                keyColumn: "Id",
                keyValue: new Guid("22222222-2222-2222-2222-222222222222"),
                column: "LastActiveAt",
                value: null);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastActiveAt",
                table: "Hero");
        }
    }
}
