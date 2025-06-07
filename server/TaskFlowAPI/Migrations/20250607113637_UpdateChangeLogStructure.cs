using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaskFlowAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateChangeLogStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChangeLogs_Users_ChangedByUserId",
                table: "ChangeLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_CreatedById",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_UpdatedById",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUsers_Users_CreatedById",
                table: "ProjectUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUsers_Users_UpdatedById",
                table: "ProjectUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_AssignedToId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_CreatedById",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_UpdatedById",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskTimeLogs_Tasks_TaskItemId",
                table: "TaskTimeLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskTimeLogs_Users_CreatedById",
                table: "TaskTimeLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskTimeLogs_Users_UpdatedById",
                table: "TaskTimeLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskTimeLogs_Users_UserId",
                table: "TaskTimeLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Workspaces_Users_CreatedById",
                table: "Workspaces");

            migrationBuilder.DropForeignKey(
                name: "FK_Workspaces_Users_UpdatedById",
                table: "Workspaces");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceUsers_Users_CreatedById",
                table: "WorkspaceUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceUsers_Users_UpdatedById",
                table: "WorkspaceUsers");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChangedByUserId",
                table: "ChangeLogs",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "TEXT");

            migrationBuilder.AddForeignKey(
                name: "FK_ChangeLogs_Users_ChangedByUserId",
                table: "ChangeLogs",
                column: "ChangedByUserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_CreatedById",
                table: "Projects",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_UpdatedById",
                table: "Projects",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUsers_Users_CreatedById",
                table: "ProjectUsers",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUsers_Users_UpdatedById",
                table: "ProjectUsers",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_AssignedToId",
                table: "Tasks",
                column: "AssignedToId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_CreatedById",
                table: "Tasks",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_UpdatedById",
                table: "Tasks",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskTimeLogs_Tasks_TaskItemId",
                table: "TaskTimeLogs",
                column: "TaskItemId",
                principalTable: "Tasks",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskTimeLogs_Users_CreatedById",
                table: "TaskTimeLogs",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskTimeLogs_Users_UpdatedById",
                table: "TaskTimeLogs",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_TaskTimeLogs_Users_UserId",
                table: "TaskTimeLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Workspaces_Users_CreatedById",
                table: "Workspaces",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_Workspaces_Users_UpdatedById",
                table: "Workspaces",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceUsers_Users_CreatedById",
                table: "WorkspaceUsers",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceUsers_Users_UpdatedById",
                table: "WorkspaceUsers",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ChangeLogs_Users_ChangedByUserId",
                table: "ChangeLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_CreatedById",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_Projects_Users_UpdatedById",
                table: "Projects");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUsers_Users_CreatedById",
                table: "ProjectUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_ProjectUsers_Users_UpdatedById",
                table: "ProjectUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_AssignedToId",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_CreatedById",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_Users_UpdatedById",
                table: "Tasks");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskTimeLogs_Tasks_TaskItemId",
                table: "TaskTimeLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskTimeLogs_Users_CreatedById",
                table: "TaskTimeLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskTimeLogs_Users_UpdatedById",
                table: "TaskTimeLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_TaskTimeLogs_Users_UserId",
                table: "TaskTimeLogs");

            migrationBuilder.DropForeignKey(
                name: "FK_Workspaces_Users_CreatedById",
                table: "Workspaces");

            migrationBuilder.DropForeignKey(
                name: "FK_Workspaces_Users_UpdatedById",
                table: "Workspaces");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceUsers_Users_CreatedById",
                table: "WorkspaceUsers");

            migrationBuilder.DropForeignKey(
                name: "FK_WorkspaceUsers_Users_UpdatedById",
                table: "WorkspaceUsers");

            migrationBuilder.AlterColumn<Guid>(
                name: "ChangedByUserId",
                table: "ChangeLogs",
                type: "TEXT",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_ChangeLogs_Users_ChangedByUserId",
                table: "ChangeLogs",
                column: "ChangedByUserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_CreatedById",
                table: "Projects",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Projects_Users_UpdatedById",
                table: "Projects",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUsers_Users_CreatedById",
                table: "ProjectUsers",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ProjectUsers_Users_UpdatedById",
                table: "ProjectUsers",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_AssignedToId",
                table: "Tasks",
                column: "AssignedToId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_CreatedById",
                table: "Tasks",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_Users_UpdatedById",
                table: "Tasks",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskTimeLogs_Tasks_TaskItemId",
                table: "TaskTimeLogs",
                column: "TaskItemId",
                principalTable: "Tasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskTimeLogs_Users_CreatedById",
                table: "TaskTimeLogs",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskTimeLogs_Users_UpdatedById",
                table: "TaskTimeLogs",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_TaskTimeLogs_Users_UserId",
                table: "TaskTimeLogs",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Workspaces_Users_CreatedById",
                table: "Workspaces",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Workspaces_Users_UpdatedById",
                table: "Workspaces",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceUsers_Users_CreatedById",
                table: "WorkspaceUsers",
                column: "CreatedById",
                principalTable: "Users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_WorkspaceUsers_Users_UpdatedById",
                table: "WorkspaceUsers",
                column: "UpdatedById",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
