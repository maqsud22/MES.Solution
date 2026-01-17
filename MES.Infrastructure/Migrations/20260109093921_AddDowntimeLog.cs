using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MES.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddDowntimeLog : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionLogs_Operators_OperatorId",
                table: "ProductionLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductionLogs_ProductionLines_ProductionLineId",
                table: "ProductionLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductionLogs_Stations_StationId",
                table: "ProductionLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductionLogs_WorkOrders_WorkOrderId",
                table: "ProductionLogs");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "ProductionLines");

            migrationBuilder.RenameColumn(
                name: "Timestamp",
                table: "ProductionLogs",
                newName: "StartTime");

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualEnd",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "ActualStart",
                table: "WorkOrders",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "IdealCycleTimeSeconds",
                table: "WorkOrders",
                type: "float",
                nullable: false,
                defaultValue: 0.0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "WorkOrders",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<Guid>(
                name: "StationId",
                table: "ProductionLogs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<Guid>(
                name: "OperatorId",
                table: "ProductionLogs",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<DateTime>(
                name: "EndTime",
                table: "ProductionLogs",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "ProductionLines",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "DowntimeLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductionLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Reason = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Comment = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DowntimeLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DowntimeLogs_ProductionLines_ProductionLineId",
                        column: x => x.ProductionLineId,
                        principalTable: "ProductionLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DowntimeLogs_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProductionResults",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ProductionLineId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    WorkOrderId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    GoodCount = table.Column<int>(type: "int", nullable: false),
                    DefectCount = table.Column<int>(type: "int", nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductionResults", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProductionResults_ProductionLines_ProductionLineId",
                        column: x => x.ProductionLineId,
                        principalTable: "ProductionLines",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProductionResults_WorkOrders_WorkOrderId",
                        column: x => x.WorkOrderId,
                        principalTable: "WorkOrders",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DowntimeLogs_ProductionLineId",
                table: "DowntimeLogs",
                column: "ProductionLineId");

            migrationBuilder.CreateIndex(
                name: "IX_DowntimeLogs_WorkOrderId",
                table: "DowntimeLogs",
                column: "WorkOrderId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionResults_ProductionLineId",
                table: "ProductionResults",
                column: "ProductionLineId");

            migrationBuilder.CreateIndex(
                name: "IX_ProductionResults_WorkOrderId",
                table: "ProductionResults",
                column: "WorkOrderId");

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionLogs_Operators_OperatorId",
                table: "ProductionLogs",
                column: "OperatorId",
                principalTable: "Operators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionLogs_ProductionLines_ProductionLineId",
                table: "ProductionLogs",
                column: "ProductionLineId",
                principalTable: "ProductionLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionLogs_Stations_StationId",
                table: "ProductionLogs",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionLogs_WorkOrders_WorkOrderId",
                table: "ProductionLogs",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ProductionLogs_Operators_OperatorId",
                table: "ProductionLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductionLogs_ProductionLines_ProductionLineId",
                table: "ProductionLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductionLogs_Stations_StationId",
                table: "ProductionLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_ProductionLogs_WorkOrders_WorkOrderId",
                table: "ProductionLogs");

            migrationBuilder.DropTable(
                name: "DowntimeLogs");

            migrationBuilder.DropTable(
                name: "ProductionResults");

            migrationBuilder.DropColumn(
                name: "ActualEnd",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "ActualStart",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "IdealCycleTimeSeconds",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "WorkOrders");

            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "ProductionLogs");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "ProductionLines");

            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "ProductionLogs",
                newName: "Timestamp");

            migrationBuilder.AlterColumn<Guid>(
                name: "StationId",
                table: "ProductionLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "OperatorId",
                table: "ProductionLogs",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "ProductionLines",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionLogs_Operators_OperatorId",
                table: "ProductionLogs",
                column: "OperatorId",
                principalTable: "Operators",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionLogs_ProductionLines_ProductionLineId",
                table: "ProductionLogs",
                column: "ProductionLineId",
                principalTable: "ProductionLines",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionLogs_Stations_StationId",
                table: "ProductionLogs",
                column: "StationId",
                principalTable: "Stations",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ProductionLogs_WorkOrders_WorkOrderId",
                table: "ProductionLogs",
                column: "WorkOrderId",
                principalTable: "WorkOrders",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
