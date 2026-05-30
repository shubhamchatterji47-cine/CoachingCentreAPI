using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoachingAPI.Migrations
{
    /// <inheritdoc />
    public partial class admincontrller : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$G4lx3zbxSZiyO6z4zyZI7.mtwaoFkdtobftg88tI3Lgy9M/ZntIGW");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$ZdMR2k.HnsplYzsmG1YQWO7PsSPiKH9Skj4n5O2BEULJe9q080/iO");
        }
    }
}
