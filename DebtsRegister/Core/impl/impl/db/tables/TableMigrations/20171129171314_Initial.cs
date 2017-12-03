using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;

namespace DebtsRegister.Core.TableMigrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "CurrentDebts",
                columns: table => new
                {
                    CreditorId = table.Column<string>(maxLength: 20, nullable: false),
                    DebtorId = table.Column<string>(maxLength: 20, nullable: false),
                    DebtTotal = table.Column<decimal>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentDebts", x => new { x.CreditorId, x.DebtorId });
                });

            migrationBuilder.CreateTable(
                name: "CurrentPeopleStatisticsDocuments",
                columns: table => new
                {
                    DocumentName = table.Column<string>(maxLength: 45, nullable: false),
                    DocumentId = table.Column<string>(maxLength: 24, nullable: true),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentPeopleStatisticsDocuments", x => x.DocumentName);
                });

            migrationBuilder.CreateTable(
                name: "CurrentStatisticsPerPerson",
                columns: table => new
                {
                    PersonId = table.Column<string>(maxLength: 20, nullable: false),
                    HistoricalDebtAverageThroughCasesOfTaking = table.Column<decimal>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentStatisticsPerPerson", x => x.PersonId);
                });

            migrationBuilder.CreateTable(
                name: "CurrentTotalsPerPerson",
                columns: table => new
                {
                    PersonId = table.Column<string>(maxLength: 20, nullable: false),
                    DueDebtsTotal = table.Column<decimal>(nullable: false),
                    HistoricalCountOfCreditsTaken = table.Column<int>(nullable: false),
                    HistoricallyCreditedInTotal = table.Column<decimal>(nullable: false),
                    HistoricallyOwedInTotal = table.Column<decimal>(nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CurrentTotalsPerPerson", x => x.PersonId);
                });

            migrationBuilder.CreateTable(
                name: "DebtDeals",
                columns: table => new
                {
                    Id = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<decimal>(nullable: false),
                    GiverId = table.Column<string>(maxLength: 20, nullable: true),
                    TakerId = table.Column<string>(maxLength: 20, nullable: true),
                    Time = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtDeals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DebtDealsAnalysis",
                columns: table => new
                {
                    DebtDealId = table.Column<long>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    IsPayback = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DebtDealsAnalysis", x => x.DebtDealId);
                });

            migrationBuilder.CreateTable(
                name: "People",
                columns: table => new
                {
                    Id = table.Column<string>(maxLength: 20, nullable: false),
                    Name = table.Column<string>(maxLength: 100, nullable: false),
                    Surname = table.Column<string>(maxLength: 100, nullable: false),
                    Timestamp = table.Column<byte[]>(rowVersion: true, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_People", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CurrentDebts_CreditorId",
                table: "CurrentDebts",
                column: "CreditorId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrentDebts_DebtorId",
                table: "CurrentDebts",
                column: "DebtorId");

            migrationBuilder.CreateIndex(
                name: "IX_CurrentTotalsPerPerson_DueDebtsTotal",
                table: "CurrentTotalsPerPerson",
                column: "DueDebtsTotal");

            migrationBuilder.CreateIndex(
                name: "IX_DebtDeals_GiverId",
                table: "DebtDeals",
                column: "GiverId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtDeals_TakerId",
                table: "DebtDeals",
                column: "TakerId");

            migrationBuilder.CreateIndex(
                name: "IX_DebtDeals_Time",
                table: "DebtDeals",
                column: "Time");

            migrationBuilder.CreateIndex(
                name: "IX_People_Name",
                table: "People",
                column: "Name");

            migrationBuilder.CreateIndex(
                name: "IX_People_Surname",
                table: "People",
                column: "Surname");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CurrentDebts");

            migrationBuilder.DropTable(
                name: "CurrentPeopleStatisticsDocuments");

            migrationBuilder.DropTable(
                name: "CurrentStatisticsPerPerson");

            migrationBuilder.DropTable(
                name: "CurrentTotalsPerPerson");

            migrationBuilder.DropTable(
                name: "DebtDeals");

            migrationBuilder.DropTable(
                name: "DebtDealsAnalysis");

            migrationBuilder.DropTable(
                name: "People");
        }
    }
}
