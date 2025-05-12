using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Final_Project.Migrations
{
    /// <inheritdoc />
    public partial class AddManagerIdToSalesData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Employees_EmployeeId",
                table: "Sales");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Sales",
                table: "Sales");

            migrationBuilder.RenameTable(
                name: "Sales",
                newName: "SalesData");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_EmployeeId_Quarter_Year",
                table: "SalesData",
                newName: "IX_SalesData_EmployeeId_Quarter_Year");

            migrationBuilder.AddColumn<int>(
                name: "ManagerId",
                table: "SalesData",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_SalesData",
                table: "SalesData",
                column: "SalesDataId");

            migrationBuilder.CreateIndex(
                name: "IX_SalesData_ManagerId",
                table: "SalesData",
                column: "ManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_SalesData_Employees_EmployeeId",
                table: "SalesData",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SalesData_Employees_ManagerId",
                table: "SalesData",
                column: "ManagerId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SalesData_Employees_EmployeeId",
                table: "SalesData");

            migrationBuilder.DropForeignKey(
                name: "FK_SalesData_Employees_ManagerId",
                table: "SalesData");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SalesData",
                table: "SalesData");

            migrationBuilder.DropIndex(
                name: "IX_SalesData_ManagerId",
                table: "SalesData");

            migrationBuilder.DropColumn(
                name: "ManagerId",
                table: "SalesData");

            migrationBuilder.RenameTable(
                name: "SalesData",
                newName: "Sales");

            migrationBuilder.RenameIndex(
                name: "IX_SalesData_EmployeeId_Quarter_Year",
                table: "Sales",
                newName: "IX_Sales_EmployeeId_Quarter_Year");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Sales",
                table: "Sales",
                column: "SalesDataId");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Employees_EmployeeId",
                table: "Sales",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
