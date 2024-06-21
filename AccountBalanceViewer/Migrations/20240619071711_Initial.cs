using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AccountBalanceViewer.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccountBalances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RnD = table.Column<double>(type: "float", nullable: false),
                    Canteen = table.Column<double>(type: "float", nullable: false),
                    CEOCarExpenses = table.Column<double>(type: "float", nullable: false),
                    Marketing = table.Column<double>(type: "float", nullable: false),
                    ParkingFines = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccountBalances", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccountBalances");
        }
    }
}
