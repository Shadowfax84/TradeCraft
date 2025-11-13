using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TC_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddtablesforSimulation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Holdings",
                columns: table => new
                {
                    PortfolioId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TickerSymbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    QuantityOwned = table.Column<int>(type: "int", nullable: false),
                    PurchasePrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    LastUpdated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Holdings", x => x.PortfolioId);
                    table.ForeignKey(
                        name: "FK_Holdings_CompanysList_TickerSymbol",
                        column: x => x.TickerSymbol,
                        principalTable: "CompanysList",
                        principalColumn: "TickerSymbol",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Holdings_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Orders",
                columns: table => new
                {
                    OrderID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderType = table.Column<int>(type: "int", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TickerSymbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    OrderStatus = table.Column<int>(type: "int", nullable: false),
                    CreatedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExecutedTimestamp = table.Column<DateTime>(type: "datetime2", nullable: true),
                    RemainingQuantity = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Orders", x => x.OrderID);
                    table.ForeignKey(
                        name: "FK_Orders_CompanysList_TickerSymbol",
                        column: x => x.TickerSymbol,
                        principalTable: "CompanysList",
                        principalColumn: "TickerSymbol",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Orders_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Trades",
                columns: table => new
                {
                    TradeId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    BuyerUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    SellerUserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    TickerSymbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Trades", x => x.TradeId);
                    table.ForeignKey(
                        name: "FK_Trades_CompanysList_TickerSymbol",
                        column: x => x.TickerSymbol,
                        principalTable: "CompanysList",
                        principalColumn: "TickerSymbol",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Trades_Users_BuyerUserId",
                        column: x => x.BuyerUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Trades_Users_SellerUserId",
                        column: x => x.SellerUserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Holdings_TickerSymbol",
                table: "Holdings",
                column: "TickerSymbol");

            migrationBuilder.CreateIndex(
                name: "IX_Holdings_UserId_TickerSymbol",
                table: "Holdings",
                columns: new[] { "UserId", "TickerSymbol" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Orders_TickerSymbol",
                table: "Orders",
                column: "TickerSymbol");

            migrationBuilder.CreateIndex(
                name: "IX_Orders_UserID",
                table: "Orders",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_BuyerUserId",
                table: "Trades",
                column: "BuyerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_SellerUserId",
                table: "Trades",
                column: "SellerUserId");

            migrationBuilder.CreateIndex(
                name: "IX_Trades_TickerSymbol",
                table: "Trades",
                column: "TickerSymbol");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Holdings");

            migrationBuilder.DropTable(
                name: "Orders");

            migrationBuilder.DropTable(
                name: "Trades");
        }
    }
}
