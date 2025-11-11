using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TC_Backend.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigrations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CompanysList",
                columns: table => new
                {
                    ClId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    CompanyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TickerSymbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanysList", x => x.ClId);
                    table.UniqueConstraint("AK_CompanysList_TickerSymbol", x => x.TickerSymbol);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    RoleId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RoleName = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RequiredPoints = table.Column<int>(type: "int", nullable: false),
                    RoleLevel = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.RoleId);
                });

            migrationBuilder.CreateTable(
                name: "CompanyProfiles",
                columns: table => new
                {
                    CPId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TickerSymbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Phone = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Website = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Sector = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Industry = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CntEmployees = table.Column<long>(type: "bigint", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CompanyProfiles", x => x.CPId);
                    table.ForeignKey(
                        name: "FK_CompanyProfiles_CompanysList_TickerSymbol",
                        column: x => x.TickerSymbol,
                        principalTable: "CompanysList",
                        principalColumn: "TickerSymbol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FinancialReports",
                columns: table => new
                {
                    FinancialReportId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ReportLabel = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    TickerSymbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    TotalRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    CostOfRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    GrossProfit = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OperatingExpense = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OperatingIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NetNonOperatingInterestIncomeExpense = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    OtherIncomeExpense = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    PretaxIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TaxProvision = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NetIncomeCommonStockholders = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DilutedNIAvailableToComStockholders = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BasicEPS = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DilutedEPS = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    BasicAverageShares = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    DilutedAverageShares = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalOperatingIncomeAsReported = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalExpenses = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NetIncomeFromContinuingAndDiscontinuedOperation = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NormalizedIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InterestIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    InterestExpense = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NetInterestIncome = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EBIT = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    EBITDA = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReconciledCostOfRevenue = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ReconciledDepreciation = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NetIncomeFromContinuingOperationNetMinorityInterest = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalUnusualItemsExcludingGoodwill = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TotalUnusualItems = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    NormalizedEBITDA = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TaxRateForCalcs = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    TaxEffectOfUnusualItems = table.Column<decimal>(type: "decimal(18,2)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialReports", x => x.FinancialReportId);
                    table.ForeignKey(
                        name: "FK_FinancialReports_CompanysList_TickerSymbol",
                        column: x => x.TickerSymbol,
                        principalTable: "CompanysList",
                        principalColumn: "TickerSymbol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StockRecords",
                columns: table => new
                {
                    RecordId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TickerSymbol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Open = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    High = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Low = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Close = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    AdjustedClose = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    Volume = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StockRecords", x => x.RecordId);
                    table.ForeignKey(
                        name: "FK_StockRecords_CompanysList_TickerSymbol",
                        column: x => x.TickerSymbol,
                        principalTable: "CompanysList",
                        principalColumn: "TickerSymbol",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GameModules",
                columns: table => new
                {
                    ModuleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    RequiredPoints = table.Column<int>(type: "int", nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameModules", x => x.ModuleID);
                    table.ForeignKey(
                        name: "FK_GameModules_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoleID = table.Column<int>(type: "int", nullable: false),
                    LastLogin = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    ProfilePictureUrl = table.Column<string>(type: "nvarchar(2083)", maxLength: 2083, nullable: true),
                    ProfilePicturePublicId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedUserName = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NormalizedEmail = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    EmailConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SecurityStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ConcurrencyStamp = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PhoneNumberConfirmed = table.Column<bool>(type: "bit", nullable: false),
                    TwoFactorEnabled = table.Column<bool>(type: "bit", nullable: false),
                    LockoutEnd = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    LockoutEnabled = table.Column<bool>(type: "bit", nullable: false),
                    AccessFailedCount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.CheckConstraint("CK_User_Username_validat", "Username NOT LIKE '%[^a-z0-9_]%'");
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleID",
                        column: x => x.RoleID,
                        principalTable: "Roles",
                        principalColumn: "RoleId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GameSubModules",
                columns: table => new
                {
                    SubModuleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ModuleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubModuleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Points = table.Column<int>(type: "int", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    IsSkippable = table.Column<bool>(type: "bit", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GameSubModules", x => x.SubModuleID);
                    table.ForeignKey(
                        name: "FK_GameSubModules_GameModules_ModuleID",
                        column: x => x.ModuleID,
                        principalTable: "GameModules",
                        principalColumn: "ModuleID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserFiles",
                columns: table => new
                {
                    FileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Url = table.Column<string>(type: "nvarchar(2083)", maxLength: 2083, nullable: false),
                    PublicId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    UploadedOn = table.Column<DateTime>(type: "datetime2", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true),
                    FileType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFiles", x => x.FileId);
                    table.ForeignKey(
                        name: "FK_UserFiles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserProgresses",
                columns: table => new
                {
                    ProgressID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserID = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    ModuleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubmoduleID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    PointsEarned = table.Column<int>(type: "int", nullable: false),
                    CompletionDate = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserProgresses", x => x.ProgressID);
                    table.ForeignKey(
                        name: "FK_UserProgresses_GameModules_ModuleID",
                        column: x => x.ModuleID,
                        principalTable: "GameModules",
                        principalColumn: "ModuleID",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserProgresses_GameSubModules_SubmoduleID",
                        column: x => x.SubmoduleID,
                        principalTable: "GameSubModules",
                        principalColumn: "SubModuleID");
                    table.ForeignKey(
                        name: "FK_UserProgresses_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CompanyProfiles_TickerSymbol",
                table: "CompanyProfiles",
                column: "TickerSymbol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CompanysList_TickerSymbol",
                table: "CompanysList",
                column: "TickerSymbol",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FinancialReports_TickerSymbol",
                table: "FinancialReports",
                column: "TickerSymbol");

            migrationBuilder.CreateIndex(
                name: "IX_GameModules_RoleID",
                table: "GameModules",
                column: "RoleID");

            migrationBuilder.CreateIndex(
                name: "IX_GameSubModules_ModuleID",
                table: "GameSubModules",
                column: "ModuleID");

            migrationBuilder.CreateIndex(
                name: "IX_Roles_RoleName",
                table: "Roles",
                column: "RoleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockRecords_Date_TickerSymbol",
                table: "StockRecords",
                columns: new[] { "Date", "TickerSymbol" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StockRecords_TickerSymbol",
                table: "StockRecords",
                column: "TickerSymbol");

            migrationBuilder.CreateIndex(
                name: "IX_UserFiles_PublicId",
                table: "UserFiles",
                column: "PublicId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_UserFiles_UserId",
                table: "UserFiles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserProgresses_ModuleID",
                table: "UserProgresses",
                column: "ModuleID");

            migrationBuilder.CreateIndex(
                name: "IX_UserProgresses_SubmoduleID",
                table: "UserProgresses",
                column: "SubmoduleID");

            migrationBuilder.CreateIndex(
                name: "IX_UserProgresses_UserID_ModuleID_SubmoduleID",
                table: "UserProgresses",
                columns: new[] { "UserID", "ModuleID", "SubmoduleID" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedEmail",
                table: "Users",
                column: "NormalizedEmail",
                unique: true,
                filter: "[NormalizedEmail] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_NormalizedUserName",
                table: "Users",
                column: "NormalizedUserName",
                unique: true,
                filter: "[NormalizedUserName] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleID",
                table: "Users",
                column: "RoleID");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CompanyProfiles");

            migrationBuilder.DropTable(
                name: "FinancialReports");

            migrationBuilder.DropTable(
                name: "StockRecords");

            migrationBuilder.DropTable(
                name: "UserFiles");

            migrationBuilder.DropTable(
                name: "UserProgresses");

            migrationBuilder.DropTable(
                name: "CompanysList");

            migrationBuilder.DropTable(
                name: "GameSubModules");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "GameModules");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
