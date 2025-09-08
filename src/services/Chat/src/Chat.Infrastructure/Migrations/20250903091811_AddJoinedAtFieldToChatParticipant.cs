﻿using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QuickChat.Chat.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddJoinedAtFieldToChatParticipant : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "JoinedAt",
                table: "ChatParticipants",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JoinedAt",
                table: "ChatParticipants");
        }
    }
}
