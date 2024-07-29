using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SoPro24Team03.Migrations
{
    /// <inheritdoc />
    public partial class InititalCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcedureTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    OrderedTaskTemplateIds = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcedureTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: true),
                    IsAdmin = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    UserName = table.Column<string>(type: "TEXT", nullable: false),
                    FirstName = table.Column<string>(type: "TEXT", nullable: false),
                    LastName = table.Column<string>(type: "TEXT", nullable: false),
                    DateOfBirth = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ContractStart = table.Column<DateTime>(type: "TEXT", nullable: false),
                    ContractType = table.Column<int>(type: "INTEGER", nullable: false),
                    EmailAddress = table.Column<string>(type: "TEXT", nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    DepartmentId = table.Column<int>(type: "INTEGER", nullable: false),
                    isArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    isSuspended = table.Column<bool>(type: "INTEGER", nullable: false),
                    changedInitialPassword = table.Column<bool>(type: "INTEGER", nullable: false),
                    Notes = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcedureTemplateRole",
                columns: table => new
                {
                    ProcedureTemplateId = table.Column<int>(type: "INTEGER", nullable: false),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcedureTemplateRole", x => new { x.ProcedureTemplateId, x.RoleId });
                    table.ForeignKey(
                        name: "FK_ProcedureTemplateRole_ProcedureTemplates_ProcedureTemplateId",
                        column: x => x.ProcedureTemplateId,
                        principalTable: "ProcedureTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcedureTemplateRole_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Procedures",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    TargetDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    NumTasksDone = table.Column<int>(type: "INTEGER", nullable: false),
                    NumTasksTotal = table.Column<int>(type: "INTEGER", nullable: false),
                    CreatedContractType = table.Column<int>(type: "INTEGER", nullable: true),
                    OrderedTaskInstIds = table.Column<string>(type: "TEXT", nullable: false),
                    TemplateId = table.Column<int>(type: "INTEGER", nullable: false),
                    FutureDepartmentId = table.Column<int>(type: "INTEGER", nullable: true),
                    RespId = table.Column<int>(type: "INTEGER", nullable: false),
                    ReferId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Procedures", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Procedures_Departments_FutureDepartmentId",
                        column: x => x.FutureDepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Procedures_ProcedureTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "ProcedureTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Procedures_Users_ReferId",
                        column: x => x.ReferId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Procedures_Users_RespId",
                        column: x => x.RespId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RoleUser",
                columns: table => new
                {
                    RolesId = table.Column<int>(type: "INTEGER", nullable: false),
                    usersId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RoleUser", x => new { x.RolesId, x.usersId });
                    table.ForeignKey(
                        name: "FK_RoleUser_Roles_RolesId",
                        column: x => x.RolesId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RoleUser_Users_usersId",
                        column: x => x.usersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskResponsibles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    RoleId = table.Column<int>(type: "INTEGER", nullable: true),
                    UserId = table.Column<int>(type: "INTEGER", nullable: true),
                    TaskRespType = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskResponsibles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskResponsibles_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TaskResponsibles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskTemplates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    Instruction = table.Column<string>(type: "TEXT", nullable: true),
                    DueType = table.Column<int>(type: "INTEGER", nullable: false),
                    CustomDays = table.Column<int>(type: "INTEGER", nullable: true),
                    TaskResponsibleId = table.Column<int>(type: "INTEGER", nullable: false),
                    ContractTypes = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskTemplates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskTemplates_TaskResponsibles_TaskResponsibleId",
                        column: x => x.TaskResponsibleId,
                        principalTable: "TaskResponsibles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DepartmentTaskTemplate",
                columns: table => new
                {
                    DepartmentsId = table.Column<int>(type: "INTEGER", nullable: false),
                    TaskTemplatesId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DepartmentTaskTemplate", x => new { x.DepartmentsId, x.TaskTemplatesId });
                    table.ForeignKey(
                        name: "FK_DepartmentTaskTemplate_Departments_DepartmentsId",
                        column: x => x.DepartmentsId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DepartmentTaskTemplate_TaskTemplates_TaskTemplatesId",
                        column: x => x.TaskTemplatesId,
                        principalTable: "TaskTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProcedureTemplateTaskTemplate",
                columns: table => new
                {
                    ProcedureTemplateId = table.Column<int>(type: "INTEGER", nullable: false),
                    TaskTemplateId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcedureTemplateTaskTemplate", x => new { x.ProcedureTemplateId, x.TaskTemplateId });
                    table.ForeignKey(
                        name: "FK_ProcedureTemplateTaskTemplate_ProcedureTemplates_ProcedureTemplateId",
                        column: x => x.ProcedureTemplateId,
                        principalTable: "ProcedureTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProcedureTemplateTaskTemplate_TaskTemplates_TaskTemplateId",
                        column: x => x.TaskTemplateId,
                        principalTable: "TaskTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TaskInsts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    TargetDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    IsArchived = table.Column<bool>(type: "INTEGER", nullable: false),
                    Status = table.Column<int>(type: "INTEGER", nullable: false),
                    ProcedureId = table.Column<int>(type: "INTEGER", nullable: false),
                    TemplateId = table.Column<int>(type: "INTEGER", nullable: true),
                    RespId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskInsts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskInsts_Procedures_ProcedureId",
                        column: x => x.ProcedureId,
                        principalTable: "Procedures",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskInsts_TaskResponsibles_RespId",
                        column: x => x.RespId,
                        principalTable: "TaskResponsibles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskInsts_TaskTemplates_TemplateId",
                        column: x => x.TemplateId,
                        principalTable: "TaskTemplates",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "TaskInstNotes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CreationDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    VisibleToOthers = table.Column<bool>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", nullable: false),
                    Text = table.Column<string>(type: "TEXT", nullable: false),
                    AuthorId = table.Column<int>(type: "INTEGER", nullable: false),
                    TaskInstId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TaskInstNotes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TaskInstNotes_TaskInsts_TaskInstId",
                        column: x => x.TaskInstId,
                        principalTable: "TaskInsts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TaskInstNotes_Users_AuthorId",
                        column: x => x.AuthorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DepartmentTaskTemplate_TaskTemplatesId",
                table: "DepartmentTaskTemplate",
                column: "TaskTemplatesId");

            migrationBuilder.CreateIndex(
                name: "IX_Procedures_FutureDepartmentId",
                table: "Procedures",
                column: "FutureDepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Procedures_ReferId",
                table: "Procedures",
                column: "ReferId");

            migrationBuilder.CreateIndex(
                name: "IX_Procedures_RespId",
                table: "Procedures",
                column: "RespId");

            migrationBuilder.CreateIndex(
                name: "IX_Procedures_TemplateId",
                table: "Procedures",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureTemplateRole_RoleId",
                table: "ProcedureTemplateRole",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcedureTemplateTaskTemplate_TaskTemplateId",
                table: "ProcedureTemplateTaskTemplate",
                column: "TaskTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_RoleUser_usersId",
                table: "RoleUser",
                column: "usersId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskInstNotes_AuthorId",
                table: "TaskInstNotes",
                column: "AuthorId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskInstNotes_TaskInstId",
                table: "TaskInstNotes",
                column: "TaskInstId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskInsts_ProcedureId",
                table: "TaskInsts",
                column: "ProcedureId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskInsts_RespId",
                table: "TaskInsts",
                column: "RespId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskInsts_TemplateId",
                table: "TaskInsts",
                column: "TemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskResponsibles_RoleId",
                table: "TaskResponsibles",
                column: "RoleId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskResponsibles_UserId",
                table: "TaskResponsibles",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TaskTemplates_TaskResponsibleId",
                table: "TaskTemplates",
                column: "TaskResponsibleId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentId",
                table: "Users",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DepartmentTaskTemplate");

            migrationBuilder.DropTable(
                name: "ProcedureTemplateRole");

            migrationBuilder.DropTable(
                name: "ProcedureTemplateTaskTemplate");

            migrationBuilder.DropTable(
                name: "RoleUser");

            migrationBuilder.DropTable(
                name: "TaskInstNotes");

            migrationBuilder.DropTable(
                name: "TaskInsts");

            migrationBuilder.DropTable(
                name: "Procedures");

            migrationBuilder.DropTable(
                name: "TaskTemplates");

            migrationBuilder.DropTable(
                name: "ProcedureTemplates");

            migrationBuilder.DropTable(
                name: "TaskResponsibles");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
