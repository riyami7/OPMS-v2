using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OperationalPlanMS.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ExternalOrganizationalUnits",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ParentId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Code = table.Column<string>(type: "nvarchar(110)", maxLength: 110, nullable: true),
                    ArabicName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    ArabicUnitName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    LastSyncAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalOrganizationalUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ExternalOrganizationalUnits_ExternalOrganizationalUnits_ParentId",
                        column: x => x.ParentId,
                        principalTable: "ExternalOrganizationalUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FiscalYears",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Year = table.Column<int>(type: "int", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    StartDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsCurrent = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FiscalYears", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Permissions = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Roles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ADUsername = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    FullNameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    FullNameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProfileImage = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    RoleId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsStepApprover = table.Column<bool>(type: "bit", nullable: false),
                    LastLoginAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    EmployeeRank = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    EmployeePosition = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    BranchName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ExternalUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalUnitName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_ExternalOrganizationalUnits_ExternalUnitId",
                        column: x => x.ExternalUnitId,
                        principalTable: "ExternalOrganizationalUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Users_Roles_RoleId",
                        column: x => x.RoleId,
                        principalTable: "Roles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EntityType = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    EntityId = table.Column<int>(type: "int", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Details = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_AuditLogs_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ChatConversations",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastMessageAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatConversations", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatConversations_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "CoreValues",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    MeaningAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MeaningEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedById = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CoreValues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CoreValues_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_CoreValues_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FinancialCosts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedById = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FinancialCosts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FinancialCosts_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_FinancialCosts_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Initiatives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    PlannedStartDate = table.Column<DateTime>(type: "date", nullable: false),
                    PlannedEndDate = table.Column<DateTime>(type: "date", nullable: false),
                    ProgressPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    ActualStartDate = table.Column<DateTime>(type: "date", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "date", nullable: true),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    StrategicObjective = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ExternalUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalUnitName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    TenantId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    SupervisorEmpNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    SupervisorName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    SupervisorRank = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    FiscalYearId = table.Column<int>(type: "int", nullable: false),
                    SupervisorId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedById = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Initiatives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Initiatives_ExternalOrganizationalUnits_ExternalUnitId",
                        column: x => x.ExternalUnitId,
                        principalTable: "ExternalOrganizationalUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Initiatives_FiscalYears_FiscalYearId",
                        column: x => x.FiscalYearId,
                        principalTable: "FiscalYears",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Initiatives_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Initiatives_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Initiatives_Users_SupervisorId",
                        column: x => x.SupervisorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Notifications",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Url = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    Icon = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    IsRead = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReadAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Notifications", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Notifications_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationalUnitSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ExternalUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalUnitName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    VisionAr = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    VisionEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MissionAr = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MissionEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedById = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationalUnitSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_OrganizationalUnitSettings_ExternalOrganizationalUnits_ExternalUnitId",
                        column: x => x.ExternalUnitId,
                        principalTable: "ExternalOrganizationalUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_OrganizationalUnitSettings_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationalUnitSettings_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StrategicAxes",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedById = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrategicAxes", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StrategicAxes_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StrategicAxes_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupportingEntities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedById = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportingEntities", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportingEntities_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SupportingEntities_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "SystemSettings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    VisionAr = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    VisionEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MissionAr = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    MissionEn = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    LastModifiedById = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsChatbotEnabled = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SystemSettings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SystemSettings_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ChatMessages",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ConversationId = table.Column<int>(type: "int", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Content = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ChatMessages", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ChatMessages_ChatConversations_ConversationId",
                        column: x => x.ConversationId,
                        principalTable: "ChatConversations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "InitiativeAccess",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    InitiativeId = table.Column<int>(type: "int", nullable: false),
                    AccessLevel = table.Column<int>(type: "int", nullable: false),
                    GrantedById = table.Column<int>(type: "int", nullable: true),
                    GrantedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InitiativeAccess", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InitiativeAccess_Initiatives_InitiativeId",
                        column: x => x.InitiativeId,
                        principalTable: "Initiatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_InitiativeAccess_Users_GrantedById",
                        column: x => x.GrantedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_InitiativeAccess_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StrategicObjectives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StrategicAxisId = table.Column<int>(type: "int", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedById = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StrategicObjectives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StrategicObjectives_StrategicAxes_StrategicAxisId",
                        column: x => x.StrategicAxisId,
                        principalTable: "StrategicAxes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StrategicObjectives_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StrategicObjectives_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "MainObjectives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StrategicObjectiveId = table.Column<int>(type: "int", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedById = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MainObjectives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MainObjectives_StrategicObjectives_StrategicObjectiveId",
                        column: x => x.StrategicObjectiveId,
                        principalTable: "StrategicObjectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MainObjectives_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_MainObjectives_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SubObjectives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    MainObjectiveId = table.Column<int>(type: "int", nullable: false),
                    ExternalUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalUnitName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedById = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubObjectives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SubObjectives_ExternalOrganizationalUnits_ExternalUnitId",
                        column: x => x.ExternalUnitId,
                        principalTable: "ExternalOrganizationalUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_SubObjectives_MainObjectives_MainObjectiveId",
                        column: x => x.MainObjectiveId,
                        principalTable: "MainObjectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubObjectives_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SubObjectives_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Code = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ProjectNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    OperationalGoal = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Priority = table.Column<int>(type: "int", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    PlannedStartDate = table.Column<DateTime>(type: "date", nullable: true),
                    PlannedEndDate = table.Column<DateTime>(type: "date", nullable: true),
                    ActualStartDate = table.Column<DateTime>(type: "date", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "date", nullable: true),
                    ProgressPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Budget = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ActualCost = table.Column<decimal>(type: "decimal(18,2)", nullable: true),
                    ExpectedOutcomes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KPIs = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    RiskNotes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExternalUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalUnitName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    ProjectManagerEmpNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    ProjectManagerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ProjectManagerRank = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DeputyManagerEmpNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    DeputyManagerName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DeputyManagerRank = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    SubObjectiveId = table.Column<int>(type: "int", nullable: true),
                    FinancialCostId = table.Column<int>(type: "int", nullable: true),
                    InitiativeId = table.Column<int>(type: "int", nullable: false),
                    ProjectManagerId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedById = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Projects_ExternalOrganizationalUnits_ExternalUnitId",
                        column: x => x.ExternalUnitId,
                        principalTable: "ExternalOrganizationalUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Projects_FinancialCosts_FinancialCostId",
                        column: x => x.FinancialCostId,
                        principalTable: "FinancialCosts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Projects_Initiatives_InitiativeId",
                        column: x => x.InitiativeId,
                        principalTable: "Initiatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Projects_SubObjectives_SubObjectiveId",
                        column: x => x.SubObjectiveId,
                        principalTable: "SubObjectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Projects_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Projects_Users_ProjectManagerId",
                        column: x => x.ProjectManagerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Documents",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Category = table.Column<int>(type: "int", nullable: false),
                    DescriptionAr = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    PreviousVersionId = table.Column<int>(type: "int", nullable: true),
                    InitiativeId = table.Column<int>(type: "int", nullable: true),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    UploadedById = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Documents", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Documents_Documents_PreviousVersionId",
                        column: x => x.PreviousVersionId,
                        principalTable: "Documents",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_Initiatives_InitiativeId",
                        column: x => x.InitiativeId,
                        principalTable: "Initiatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Documents_Users_UploadedById",
                        column: x => x.UploadedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Milestones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DueDate = table.Column<DateTime>(type: "date", nullable: false),
                    CompletedDate = table.Column<DateTime>(type: "date", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    MilestoneType = table.Column<int>(type: "int", nullable: false),
                    Deliverable = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Milestones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Milestones_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Milestones_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectKPIs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    KPIText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    TargetValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    ActualValue = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectKPIs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectKPIs_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectRequirements",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    RequirementText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectRequirements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectRequirements_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectStatusChanges",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    OldStatus = table.Column<int>(type: "int", nullable: false),
                    NewStatus = table.Column<int>(type: "int", nullable: false),
                    ObstacleType = table.Column<int>(type: "int", nullable: true),
                    ObstacleDescription = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    Reason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    ActionTaken = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    ExpectedResumeDate = table.Column<DateTime>(type: "date", nullable: true),
                    ChangedById = table.Column<int>(type: "int", nullable: false),
                    ChangedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectStatusChanges", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectStatusChanges_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectStatusChanges_Users_ChangedById",
                        column: x => x.ChangedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "ProjectSubObjectives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    SubObjectiveId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSubObjectives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectSubObjectives_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectSubObjectives_SubObjectives_SubObjectiveId",
                        column: x => x.SubObjectiveId,
                        principalTable: "SubObjectives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectSupportingUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    SupportingEntityId = table.Column<int>(type: "int", nullable: true),
                    ExternalUnitId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ExternalUnitName = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: true),
                    RepresentativeEmpNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    RepresentativeName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    RepresentativeRank = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectSupportingUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectSupportingUnits_ExternalOrganizationalUnits_ExternalUnitId",
                        column: x => x.ExternalUnitId,
                        principalTable: "ExternalOrganizationalUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_ProjectSupportingUnits_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectSupportingUnits_SupportingEntities_SupportingEntityId",
                        column: x => x.SupportingEntityId,
                        principalTable: "SupportingEntities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "ProjectYearTargets",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    Year = table.Column<int>(type: "int", nullable: false),
                    TargetPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectYearTargets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProjectYearTargets_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Steps",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StepNumber = table.Column<int>(type: "int", nullable: false),
                    NameAr = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    NameEn = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    DescriptionAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DescriptionEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<int>(type: "int", nullable: false),
                    PlannedStartDate = table.Column<DateTime>(type: "date", nullable: false),
                    PlannedEndDate = table.Column<DateTime>(type: "date", nullable: false),
                    ActualStartDate = table.Column<DateTime>(type: "date", nullable: true),
                    ActualEndDate = table.Column<DateTime>(type: "date", nullable: true),
                    ProgressPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Weight = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    Notes = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ProjectId = table.Column<int>(type: "int", nullable: false),
                    InitiativeId = table.Column<int>(type: "int", nullable: true),
                    AssignedToId = table.Column<int>(type: "int", nullable: true),
                    AssignedToEmpNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: true),
                    AssignedToName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    AssignedToRank = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    DependsOnStepId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedById = table.Column<int>(type: "int", nullable: true),
                    LastModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    ApprovalStatus = table.Column<int>(type: "int", nullable: false),
                    RejectionReason = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    CompletionDetails = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ApprovedById = table.Column<int>(type: "int", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    SubmittedForApprovalAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    ApproverNotes = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Steps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Steps_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Steps_Steps_DependsOnStepId",
                        column: x => x.DependsOnStepId,
                        principalTable: "Steps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Steps_Users_ApprovedById",
                        column: x => x.ApprovedById,
                        principalTable: "Users",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Steps_Users_AssignedToId",
                        column: x => x.AssignedToId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_Steps_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Steps_Users_LastModifiedById",
                        column: x => x.LastModifiedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "SupportingUnitRepresentatives",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProjectSupportingUnitId = table.Column<int>(type: "int", nullable: false),
                    EmpNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Rank = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SupportingUnitRepresentatives", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SupportingUnitRepresentatives_ProjectSupportingUnits_ProjectSupportingUnitId",
                        column: x => x.ProjectSupportingUnitId,
                        principalTable: "ProjectSupportingUnits",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProgressUpdates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UpdateType = table.Column<int>(type: "int", nullable: false),
                    ProgressPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    PreviousPercentage = table.Column<decimal>(type: "decimal(5,2)", nullable: false),
                    NotesAr = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NotesEn = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Challenges = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NextSteps = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Quarter = table.Column<int>(type: "int", nullable: true),
                    PeriodStart = table.Column<DateTime>(type: "date", nullable: true),
                    PeriodEnd = table.Column<DateTime>(type: "date", nullable: true),
                    IsQuarterlyReport = table.Column<bool>(type: "bit", nullable: false),
                    InitiativeId = table.Column<int>(type: "int", nullable: true),
                    ProjectId = table.Column<int>(type: "int", nullable: true),
                    StepId = table.Column<int>(type: "int", nullable: true),
                    CreatedById = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgressUpdates", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProgressUpdates_Initiatives_InitiativeId",
                        column: x => x.InitiativeId,
                        principalTable: "Initiatives",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgressUpdates_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgressUpdates_Steps_StepId",
                        column: x => x.StepId,
                        principalTable: "Steps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProgressUpdates_Users_CreatedById",
                        column: x => x.CreatedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "StepAttachments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StepId = table.Column<int>(type: "int", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    OriginalFileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    ContentType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    FileSize = table.Column<long>(type: "bigint", nullable: false),
                    FilePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    UploadedById = table.Column<int>(type: "int", nullable: false),
                    UploadedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepAttachments", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepAttachments_Steps_StepId",
                        column: x => x.StepId,
                        principalTable: "Steps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StepAttachments_Users_UploadedById",
                        column: x => x.UploadedById,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StepKPIs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StepId = table.Column<int>(type: "int", nullable: false),
                    Indicator = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepKPIs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepKPIs_Steps_StepId",
                        column: x => x.StepId,
                        principalTable: "Steps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StepSupportingUnits",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StepId = table.Column<int>(type: "int", nullable: false),
                    ProjectSupportingUnitId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepSupportingUnits", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepSupportingUnits_ProjectSupportingUnits_ProjectSupportingUnitId",
                        column: x => x.ProjectSupportingUnitId,
                        principalTable: "ProjectSupportingUnits",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_StepSupportingUnits_Steps_StepId",
                        column: x => x.StepId,
                        principalTable: "Steps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StepTeamMembers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StepId = table.Column<int>(type: "int", nullable: false),
                    EmpNumber = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Rank = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    Role = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StepTeamMembers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StepTeamMembers_Steps_StepId",
                        column: x => x.StepId,
                        principalTable: "Steps",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_UserId",
                table: "AuditLogs",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatConversations_UserId",
                table: "ChatConversations",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ChatMessages_ConversationId",
                table: "ChatMessages",
                column: "ConversationId");

            migrationBuilder.CreateIndex(
                name: "IX_CoreValues_CreatedById",
                table: "CoreValues",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_CoreValues_LastModifiedById",
                table: "CoreValues",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_InitiativeId",
                table: "Documents",
                column: "InitiativeId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_PreviousVersionId",
                table: "Documents",
                column: "PreviousVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_ProjectId",
                table: "Documents",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Documents_UploadedById",
                table: "Documents",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_ExternalOrganizationalUnits_ParentId",
                table: "ExternalOrganizationalUnits",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialCosts_CreatedById",
                table: "FinancialCosts",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_FinancialCosts_LastModifiedById",
                table: "FinancialCosts",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_InitiativeAccess_GrantedById",
                table: "InitiativeAccess",
                column: "GrantedById");

            migrationBuilder.CreateIndex(
                name: "IX_InitiativeAccess_InitiativeId",
                table: "InitiativeAccess",
                column: "InitiativeId");

            migrationBuilder.CreateIndex(
                name: "IX_InitiativeAccess_UserId_InitiativeId",
                table: "InitiativeAccess",
                columns: new[] { "UserId", "InitiativeId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Initiatives_Code",
                table: "Initiatives",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Initiatives_CreatedById",
                table: "Initiatives",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Initiatives_ExternalUnitId",
                table: "Initiatives",
                column: "ExternalUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Initiatives_FiscalYearId",
                table: "Initiatives",
                column: "FiscalYearId");

            migrationBuilder.CreateIndex(
                name: "IX_Initiatives_LastModifiedById",
                table: "Initiatives",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Initiatives_SupervisorId",
                table: "Initiatives",
                column: "SupervisorId");

            migrationBuilder.CreateIndex(
                name: "IX_MainObjectives_Code",
                table: "MainObjectives",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MainObjectives_CreatedById",
                table: "MainObjectives",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_MainObjectives_LastModifiedById",
                table: "MainObjectives",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_MainObjectives_StrategicObjectiveId",
                table: "MainObjectives",
                column: "StrategicObjectiveId");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_CreatedById",
                table: "Milestones",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Milestones_ProjectId",
                table: "Milestones",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Notifications_UserId",
                table: "Notifications",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalUnitSettings_CreatedById",
                table: "OrganizationalUnitSettings",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalUnitSettings_ExternalUnitId",
                table: "OrganizationalUnitSettings",
                column: "ExternalUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationalUnitSettings_LastModifiedById",
                table: "OrganizationalUnitSettings",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressUpdates_CreatedById",
                table: "ProgressUpdates",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressUpdates_InitiativeId",
                table: "ProgressUpdates",
                column: "InitiativeId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressUpdates_ProjectId",
                table: "ProgressUpdates",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProgressUpdates_StepId",
                table: "ProgressUpdates",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectKPIs_ProjectId",
                table: "ProjectKPIs",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectRequirements_ProjectId",
                table: "ProjectRequirements",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_Code",
                table: "Projects",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Projects_CreatedById",
                table: "Projects",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ExternalUnitId",
                table: "Projects",
                column: "ExternalUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_FinancialCostId",
                table: "Projects",
                column: "FinancialCostId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_InitiativeId",
                table: "Projects",
                column: "InitiativeId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_LastModifiedById",
                table: "Projects",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_ProjectManagerId",
                table: "Projects",
                column: "ProjectManagerId");

            migrationBuilder.CreateIndex(
                name: "IX_Projects_SubObjectiveId",
                table: "Projects",
                column: "SubObjectiveId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStatusChanges_ChangedById",
                table: "ProjectStatusChanges",
                column: "ChangedById");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectStatusChanges_ProjectId",
                table: "ProjectStatusChanges",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSubObjectives_ProjectId_SubObjectiveId",
                table: "ProjectSubObjectives",
                columns: new[] { "ProjectId", "SubObjectiveId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSubObjectives_SubObjectiveId",
                table: "ProjectSubObjectives",
                column: "SubObjectiveId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSupportingUnits_ExternalUnitId",
                table: "ProjectSupportingUnits",
                column: "ExternalUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSupportingUnits_ProjectId",
                table: "ProjectSupportingUnits",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectSupportingUnits_SupportingEntityId",
                table: "ProjectSupportingUnits",
                column: "SupportingEntityId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectYearTargets_ProjectId_Year",
                table: "ProjectYearTargets",
                columns: new[] { "ProjectId", "Year" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Roles_Code",
                table: "Roles",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StepAttachments_StepId",
                table: "StepAttachments",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_StepAttachments_UploadedById",
                table: "StepAttachments",
                column: "UploadedById");

            migrationBuilder.CreateIndex(
                name: "IX_StepKPIs_StepId",
                table: "StepKPIs",
                column: "StepId");

            migrationBuilder.CreateIndex(
                name: "IX_Steps_ApprovedById",
                table: "Steps",
                column: "ApprovedById");

            migrationBuilder.CreateIndex(
                name: "IX_Steps_AssignedToId",
                table: "Steps",
                column: "AssignedToId");

            migrationBuilder.CreateIndex(
                name: "IX_Steps_CreatedById",
                table: "Steps",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_Steps_DependsOnStepId",
                table: "Steps",
                column: "DependsOnStepId");

            migrationBuilder.CreateIndex(
                name: "IX_Steps_LastModifiedById",
                table: "Steps",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Steps_ProjectId",
                table: "Steps",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_StepSupportingUnits_ProjectSupportingUnitId",
                table: "StepSupportingUnits",
                column: "ProjectSupportingUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_StepSupportingUnits_StepId_ProjectSupportingUnitId",
                table: "StepSupportingUnits",
                columns: new[] { "StepId", "ProjectSupportingUnitId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StepTeamMembers_StepId_EmpNumber",
                table: "StepTeamMembers",
                columns: new[] { "StepId", "EmpNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StrategicAxes_Code",
                table: "StrategicAxes",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StrategicAxes_CreatedById",
                table: "StrategicAxes",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StrategicAxes_LastModifiedById",
                table: "StrategicAxes",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_StrategicObjectives_Code",
                table: "StrategicObjectives",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_StrategicObjectives_CreatedById",
                table: "StrategicObjectives",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_StrategicObjectives_LastModifiedById",
                table: "StrategicObjectives",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_StrategicObjectives_StrategicAxisId",
                table: "StrategicObjectives",
                column: "StrategicAxisId");

            migrationBuilder.CreateIndex(
                name: "IX_SubObjectives_Code",
                table: "SubObjectives",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SubObjectives_CreatedById",
                table: "SubObjectives",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubObjectives_ExternalUnitId",
                table: "SubObjectives",
                column: "ExternalUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_SubObjectives_LastModifiedById",
                table: "SubObjectives",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_SubObjectives_MainObjectiveId",
                table: "SubObjectives",
                column: "MainObjectiveId");

            migrationBuilder.CreateIndex(
                name: "IX_SupportingEntities_Code",
                table: "SupportingEntities",
                column: "Code",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SupportingEntities_CreatedById",
                table: "SupportingEntities",
                column: "CreatedById");

            migrationBuilder.CreateIndex(
                name: "IX_SupportingEntities_IsActive",
                table: "SupportingEntities",
                column: "IsActive");

            migrationBuilder.CreateIndex(
                name: "IX_SupportingEntities_LastModifiedById",
                table: "SupportingEntities",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_SupportingUnitRepresentatives_ProjectSupportingUnitId_EmpNumber",
                table: "SupportingUnitRepresentatives",
                columns: new[] { "ProjectSupportingUnitId", "EmpNumber" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SystemSettings_LastModifiedById",
                table: "SystemSettings",
                column: "LastModifiedById");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ADUsername",
                table: "Users",
                column: "ADUsername",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_ExternalUnitId",
                table: "Users",
                column: "ExternalUnitId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_RoleId",
                table: "Users",
                column: "RoleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "ChatMessages");

            migrationBuilder.DropTable(
                name: "CoreValues");

            migrationBuilder.DropTable(
                name: "Documents");

            migrationBuilder.DropTable(
                name: "InitiativeAccess");

            migrationBuilder.DropTable(
                name: "Milestones");

            migrationBuilder.DropTable(
                name: "Notifications");

            migrationBuilder.DropTable(
                name: "OrganizationalUnitSettings");

            migrationBuilder.DropTable(
                name: "ProgressUpdates");

            migrationBuilder.DropTable(
                name: "ProjectKPIs");

            migrationBuilder.DropTable(
                name: "ProjectRequirements");

            migrationBuilder.DropTable(
                name: "ProjectStatusChanges");

            migrationBuilder.DropTable(
                name: "ProjectSubObjectives");

            migrationBuilder.DropTable(
                name: "ProjectYearTargets");

            migrationBuilder.DropTable(
                name: "StepAttachments");

            migrationBuilder.DropTable(
                name: "StepKPIs");

            migrationBuilder.DropTable(
                name: "StepSupportingUnits");

            migrationBuilder.DropTable(
                name: "StepTeamMembers");

            migrationBuilder.DropTable(
                name: "SupportingUnitRepresentatives");

            migrationBuilder.DropTable(
                name: "SystemSettings");

            migrationBuilder.DropTable(
                name: "ChatConversations");

            migrationBuilder.DropTable(
                name: "Steps");

            migrationBuilder.DropTable(
                name: "ProjectSupportingUnits");

            migrationBuilder.DropTable(
                name: "Projects");

            migrationBuilder.DropTable(
                name: "SupportingEntities");

            migrationBuilder.DropTable(
                name: "FinancialCosts");

            migrationBuilder.DropTable(
                name: "Initiatives");

            migrationBuilder.DropTable(
                name: "SubObjectives");

            migrationBuilder.DropTable(
                name: "FiscalYears");

            migrationBuilder.DropTable(
                name: "MainObjectives");

            migrationBuilder.DropTable(
                name: "StrategicObjectives");

            migrationBuilder.DropTable(
                name: "StrategicAxes");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "ExternalOrganizationalUnits");

            migrationBuilder.DropTable(
                name: "Roles");
        }
    }
}
