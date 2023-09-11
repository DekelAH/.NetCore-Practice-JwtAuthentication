using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CitiesManager.WebApi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    CityID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CityName = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.CityID);
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "CityID", "CityName" },
                values: new object[,]
                {
                    { new Guid("3377b955-7aad-46a1-8e42-a4564fd1c664"), "Jerusalem" },
                    { new Guid("bc9e17c6-a6cf-47a3-9c16-6cdfed114723"), "Beirut" },
                    { new Guid("d4d68d61-2e25-40ee-9d62-49a71586248d"), "Rome" },
                    { new Guid("dd2a0f96-f9e9-40ec-9719-4d88d4bc1a23"), "Athens" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cities");
        }
    }
}
