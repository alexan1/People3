using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace People3.Data.Migrations
{
    public partial class rating1 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Rate",
                table: "Rating",
                type: "int",
                nullable: false,
                oldClrType: typeof(decimal));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "Rate",
                table: "Rating",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }
    }
}
