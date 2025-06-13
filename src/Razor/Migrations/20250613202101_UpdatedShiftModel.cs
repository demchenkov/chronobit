using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Razor.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedShiftModel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_AspNetUsers_UserId",
                table: "Shifts");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Shifts",
                newName: "WorkerId");

            migrationBuilder.RenameIndex(
                name: "IX_Shifts_UserId",
                table: "Shifts",
                newName: "IX_Shifts_WorkerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_Workers_WorkerId",
                table: "Shifts",
                column: "WorkerId",
                principalTable: "Workers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Shifts_Workers_WorkerId",
                table: "Shifts");

            migrationBuilder.RenameColumn(
                name: "WorkerId",
                table: "Shifts",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Shifts_WorkerId",
                table: "Shifts",
                newName: "IX_Shifts_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Shifts_AspNetUsers_UserId",
                table: "Shifts",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
