using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoffeeShop.Migrations
{
    public partial class FixIdName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomersId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Employees_EmployeesId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "totalPrice",
                table: "Orders",
                newName: "TotalPrice");

            migrationBuilder.RenameColumn(
                name: "dateOrdered",
                table: "Orders",
                newName: "DateOrdered");

            migrationBuilder.RenameColumn(
                name: "EmployeesId",
                table: "Orders",
                newName: "EmployeeId");

            migrationBuilder.RenameColumn(
                name: "CustomersId",
                table: "Orders",
                newName: "CustomerId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_EmployeesId",
                table: "Orders",
                newName: "IX_Orders_EmployeeId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CustomersId",
                table: "Orders",
                newName: "IX_Orders_CustomerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders",
                column: "CustomerId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Employees_EmployeeId",
                table: "Orders",
                column: "EmployeeId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Customers_CustomerId",
                table: "Orders");

            migrationBuilder.DropForeignKey(
                name: "FK_Orders_Employees_EmployeeId",
                table: "Orders");

            migrationBuilder.RenameColumn(
                name: "TotalPrice",
                table: "Orders",
                newName: "totalPrice");

            migrationBuilder.RenameColumn(
                name: "DateOrdered",
                table: "Orders",
                newName: "dateOrdered");

            migrationBuilder.RenameColumn(
                name: "EmployeeId",
                table: "Orders",
                newName: "EmployeesId");

            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Orders",
                newName: "CustomersId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_EmployeeId",
                table: "Orders",
                newName: "IX_Orders_EmployeesId");

            migrationBuilder.RenameIndex(
                name: "IX_Orders_CustomerId",
                table: "Orders",
                newName: "IX_Orders_CustomersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Customers_CustomersId",
                table: "Orders",
                column: "CustomersId",
                principalTable: "Customers",
                principalColumn: "CustomerId",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Orders_Employees_EmployeesId",
                table: "Orders",
                column: "EmployeesId",
                principalTable: "Employees",
                principalColumn: "EmployeeId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
