using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GauntletSlack3.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddUserOnlineStatus : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsOnline",
                table: "Users",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "ChannelMemberships",
                keyColumns: new[] { "ChannelId", "UserId" },
                keyValues: new object[] { -1, -1 },
                column: "JoinedAt",
                value: new DateTime(2025, 1, 10, 8, 34, 49, 754, DateTimeKind.Utc).AddTicks(1051));

            migrationBuilder.UpdateData(
                table: "Channels",
                keyColumn: "Id",
                keyValue: -1,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 10, 8, 34, 49, 754, DateTimeKind.Utc).AddTicks(1051));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: -1,
                column: "IsOnline",
                value: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnline",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "ChannelMemberships",
                keyColumns: new[] { "ChannelId", "UserId" },
                keyValues: new object[] { -1, -1 },
                column: "JoinedAt",
                value: new DateTime(2025, 1, 10, 0, 33, 56, 391, DateTimeKind.Utc).AddTicks(6223));

            migrationBuilder.UpdateData(
                table: "Channels",
                keyColumn: "Id",
                keyValue: -1,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 10, 0, 33, 56, 391, DateTimeKind.Utc).AddTicks(6223));
        }
    }
}
