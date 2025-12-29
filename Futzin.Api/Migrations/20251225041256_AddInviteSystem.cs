using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Futzin.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddInviteSystem : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "InviteToken",
                table: "Peladas",
                type: "TEXT",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsPublic",
                table: "Peladas",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.CreateTable(
                name: "PeladaInvites",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PeladaId = table.Column<int>(type: "INTEGER", nullable: false),
                    InvitedUserId = table.Column<int>(type: "INTEGER", nullable: false),
                    InvitedAt = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsAccepted = table.Column<bool>(type: "INTEGER", nullable: false),
                    AcceptedAt = table.Column<DateTime>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PeladaInvites", x => x.Id);
                    table.ForeignKey(
                        name: "FK_PeladaInvites_Peladas_PeladaId",
                        column: x => x.PeladaId,
                        principalTable: "Peladas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PeladaInvites_Users_InvitedUserId",
                        column: x => x.InvitedUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Peladas_InviteToken",
                table: "Peladas",
                column: "InviteToken",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PeladaInvites_InvitedUserId",
                table: "PeladaInvites",
                column: "InvitedUserId");

            migrationBuilder.CreateIndex(
                name: "IX_PeladaInvites_PeladaId_InvitedUserId",
                table: "PeladaInvites",
                columns: new[] { "PeladaId", "InvitedUserId" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PeladaInvites");

            migrationBuilder.DropIndex(
                name: "IX_Peladas_InviteToken",
                table: "Peladas");

            migrationBuilder.DropColumn(
                name: "InviteToken",
                table: "Peladas");

            migrationBuilder.DropColumn(
                name: "IsPublic",
                table: "Peladas");
        }
    }
}
