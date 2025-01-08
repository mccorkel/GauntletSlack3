using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GauntletSlack3.Api.Migrations
{
    /// <inheritdoc />
    public partial class SeedSampleData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ChannelMemberships",
                keyColumns: new[] { "ChannelId", "UserId" },
                keyValues: new object[] { 1, "admin" },
                column: "JoinedAt",
                value: new DateTime(2025, 1, 8, 7, 43, 11, 405, DateTimeKind.Utc).AddTicks(2891));

            migrationBuilder.UpdateData(
                table: "Channels",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 8, 7, 43, 11, 405, DateTimeKind.Utc).AddTicks(2891));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "admin",
                column: "CreatedAt",
                value: new DateTime(2025, 1, 8, 7, 43, 11, 405, DateTimeKind.Utc).AddTicks(2891));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "ChannelMemberships",
                keyColumns: new[] { "ChannelId", "UserId" },
                keyValues: new object[] { 1, "admin" },
                column: "JoinedAt",
                value: new DateTime(2025, 1, 8, 7, 43, 2, 398, DateTimeKind.Utc).AddTicks(210));

            migrationBuilder.UpdateData(
                table: "Channels",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTime(2025, 1, 8, 7, 43, 2, 398, DateTimeKind.Utc).AddTicks(210));

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: "admin",
                column: "CreatedAt",
                value: new DateTime(2025, 1, 8, 7, 43, 2, 398, DateTimeKind.Utc).AddTicks(210));
        }
    }
}
