using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EF_project.Migrations
{
    /// <inheritdoc />
    public partial class Changes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Price",
                table: "Tickets");

            migrationBuilder.AlterColumn<int>(
                name: "StatusId",
                table: "Tickets",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<DateTime>(
                name: "SaleDate",
                table: "Sales",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 15, 21, 7, 51, 243, DateTimeKind.Local).AddTicks(783),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 14, 22, 48, 15, 122, DateTimeKind.Local).AddTicks(947));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "StatusId",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Price",
                table: "Tickets",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<DateTime>(
                name: "SaleDate",
                table: "Sales",
                type: "datetime(6)",
                nullable: false,
                defaultValue: new DateTime(2025, 4, 14, 22, 48, 15, 122, DateTimeKind.Local).AddTicks(947),
                oldClrType: typeof(DateTime),
                oldType: "datetime(6)",
                oldDefaultValue: new DateTime(2025, 4, 15, 21, 7, 51, 243, DateTimeKind.Local).AddTicks(783));
        }
    }
}
