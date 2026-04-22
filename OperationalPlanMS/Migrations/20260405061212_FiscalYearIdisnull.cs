using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OperationalPlanMS.Migrations
{
    /// <inheritdoc />
    public partial class FiscalYearIdisnull : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FiscalYearId",
                table: "Initiatives",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "FiscalYearId",
                table: "Initiatives",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);
        }
    }
}
