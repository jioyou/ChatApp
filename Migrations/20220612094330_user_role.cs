using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ChatApp.Migrations
{
    public partial class user_role : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "70b6acb8-2462-49e9-8a6f-1b65b4563527", "4f60e799-c7ab-4a19-a759-e15e09a788e0", "Admin", null });

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[] { "75929734-694e-45d1-be3e-dfc9a8a441a8", "b9b8a5ee-96a9-4516-9887-6bc65bda5e0b", "Normal", null });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "70b6acb8-2462-49e9-8a6f-1b65b4563527");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "75929734-694e-45d1-be3e-dfc9a8a441a8");
        }
    }
}
