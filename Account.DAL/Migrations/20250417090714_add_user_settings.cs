using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Account.DAL.Migrations
{
    /// <inheritdoc />
    public partial class add_user_settings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserSettings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    DarkMode = table.Column<bool>(type: "boolean", nullable: false),
                    UserId = table.Column<Guid>(type: "uuid", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserSettings_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserSettings_UserId",
                table: "UserSettings",
                column: "UserId",
                unique: true);

            migrationBuilder.Sql(@"
        INSERT INTO ""UserSettings"" (""Id"", ""DarkMode"", ""UserId"")
        SELECT 
            gen_random_uuid(), 
            false, 
            ""Id""
        FROM ""AspNetUsers""
        WHERE NOT EXISTS (
            SELECT 1 FROM ""UserSettings"" WHERE ""UserId"" = ""AspNetUsers"".""Id""
        )
    ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserSettings");
        }
    }
}
