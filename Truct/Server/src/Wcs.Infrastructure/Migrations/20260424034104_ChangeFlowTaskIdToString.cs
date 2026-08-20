using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Wcs.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class ChangeFlowTaskIdToString : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Step 1: Drop FK constraints on child tables
            migrationBuilder.DropForeignKey(
                name: "FK_FlowTaskWaitingSets_FlowTasks_FlowTaskId",
                table: "FlowTaskWaitingSets");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowTaskHistory_FlowTasks_FlowTaskId",
                table: "FlowTaskHistory");

            // Step 2: Drop Primary Key on FlowTasks
            migrationBuilder.DropPrimaryKey(
                name: "PK_FlowTasks",
                table: "FlowTasks");

            // Step 3: Alter all columns
            migrationBuilder.AlterColumn<string>(
                name: "FlowTaskId",
                table: "Requests",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<string>(
                name: "FlowTaskId",
                table: "FlowTaskWaitingSets",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "FlowTasks",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AlterColumn<string>(
                name: "FlowTaskId",
                table: "FlowTaskHistory",
                type: "nvarchar(10)",
                maxLength: 10,
                nullable: false,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            // Step 4: Recreate Primary Key
            migrationBuilder.AddPrimaryKey(
                name: "PK_FlowTasks",
                table: "FlowTasks",
                column: "Id");

            // Step 5: Recreate FK constraints
            migrationBuilder.AddForeignKey(
                name: "FK_FlowTaskWaitingSets_FlowTasks_FlowTaskId",
                table: "FlowTaskWaitingSets",
                column: "FlowTaskId",
                principalTable: "FlowTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowTaskHistory_FlowTasks_FlowTaskId",
                table: "FlowTaskHistory",
                column: "FlowTaskId",
                principalTable: "FlowTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlowTaskWaitingSets_FlowTasks_FlowTaskId",
                table: "FlowTaskWaitingSets");

            migrationBuilder.DropForeignKey(
                name: "FK_FlowTaskHistory_FlowTasks_FlowTaskId",
                table: "FlowTaskHistory");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FlowTasks",
                table: "FlowTasks");

            migrationBuilder.AlterColumn<Guid>(
                name: "FlowTaskId",
                table: "Requests",
                type: "uniqueidentifier",
                maxLength: 50,
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50,
                oldNullable: true);

            migrationBuilder.AlterColumn<Guid>(
                name: "FlowTaskId",
                table: "FlowTaskWaitingSets",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<Guid>(
                name: "Id",
                table: "FlowTasks",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<Guid>(
                name: "FlowTaskId",
                table: "FlowTaskHistory",
                type: "uniqueidentifier",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(10)",
                oldMaxLength: 10);

            migrationBuilder.AddPrimaryKey(
                name: "PK_FlowTasks",
                table: "FlowTasks",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FlowTaskWaitingSets_FlowTasks_FlowTaskId",
                table: "FlowTaskWaitingSets",
                column: "FlowTaskId",
                principalTable: "FlowTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_FlowTaskHistory_FlowTasks_FlowTaskId",
                table: "FlowTaskHistory",
                column: "FlowTaskId",
                principalTable: "FlowTasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
