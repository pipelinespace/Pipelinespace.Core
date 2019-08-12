using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PipelineSpace.Infra.Data.SqlServer.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProgrammingLanguage",
                columns: table => new
                {
                    ProgrammingLanguageId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProgrammingLanguage", x => x.ProgrammingLanguageId);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTemplate",
                columns: table => new
                {
                    ProjectTemplateId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    CloudProviderType = table.Column<int>(nullable: false),
                    Logo = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTemplate", x => x.ProjectTemplateId);
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(nullable: false),
                    LastName = table.Column<string>(nullable: false),
                    Email = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProjectServiceTemplate",
                columns: table => new
                {
                    ProjectServiceTemplateId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ServiceCMSType = table.Column<int>(nullable: false),
                    ServiceCPSType = table.Column<int>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Description = table.Column<string>(nullable: false),
                    Url = table.Column<string>(nullable: false),
                    Path = table.Column<string>(nullable: true),
                    Logo = table.Column<string>(nullable: false),
                    PipeType = table.Column<int>(nullable: false),
                    TemplateType = table.Column<int>(nullable: false),
                    TemplateAccess = table.Column<int>(nullable: false),
                    NeedCredentials = table.Column<bool>(nullable: false),
                    ProgrammingLanguageId = table.Column<Guid>(nullable: false),
                    Framework = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectServiceTemplate", x => x.ProjectServiceTemplateId);
                    table.ForeignKey(
                        name: "FK_ProjectServiceTemplate_ProgrammingLanguage_ProgrammingLanguageId",
                        column: x => x.ProgrammingLanguageId,
                        principalTable: "ProgrammingLanguage",
                        principalColumn: "ProgrammingLanguageId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Organization",
                columns: table => new
                {
                    OrganizationId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 100, nullable: false),
                    WebSiteUrl = table.Column<string>(nullable: true),
                    OwnerId = table.Column<string>(maxLength: 450, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Organization", x => x.OrganizationId);
                    table.ForeignKey(
                        name: "FK_Organization_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectServiceTemplateCredential",
                columns: table => new
                {
                    ProjectServiceTemplateId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CMSType = table.Column<int>(nullable: false),
                    AccessId = table.Column<string>(nullable: true),
                    AccessSecret = table.Column<string>(nullable: true),
                    AccessToken = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectServiceTemplateCredential", x => x.ProjectServiceTemplateId);
                    table.ForeignKey(
                        name: "FK_ProjectServiceTemplateCredential_ProjectServiceTemplate_ProjectServiceTemplateId",
                        column: x => x.ProjectServiceTemplateId,
                        principalTable: "ProjectServiceTemplate",
                        principalColumn: "ProjectServiceTemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectServiceTemplateParameter",
                columns: table => new
                {
                    ProjectServiceTemplateParameterId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ProjectServiceTemplateId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    VariableName = table.Column<string>(nullable: false),
                    Scope = table.Column<string>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectServiceTemplateParameter", x => x.ProjectServiceTemplateParameterId);
                    table.ForeignKey(
                        name: "FK_ProjectServiceTemplateParameter_ProjectServiceTemplate_ProjectServiceTemplateId",
                        column: x => x.ProjectServiceTemplateId,
                        principalTable: "ProjectServiceTemplate",
                        principalColumn: "ProjectServiceTemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectTemplateService",
                columns: table => new
                {
                    ProjectTemplateId = table.Column<Guid>(nullable: false),
                    ProjectServiceTemplateId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectTemplateService", x => new { x.ProjectTemplateId, x.ProjectServiceTemplateId, x.Name });
                    table.ForeignKey(
                        name: "FK_ProjectTemplateService_ProjectServiceTemplate_ProjectServiceTemplateId",
                        column: x => x.ProjectServiceTemplateId,
                        principalTable: "ProjectServiceTemplate",
                        principalColumn: "ProjectServiceTemplateId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectTemplateService_ProjectTemplate_ProjectTemplateId",
                        column: x => x.ProjectTemplateId,
                        principalTable: "ProjectTemplate",
                        principalColumn: "ProjectTemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationCMS",
                columns: table => new
                {
                    OrganizationCMSId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    AccountId = table.Column<string>(nullable: false),
                    AccountName = table.Column<string>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    ConnectionType = table.Column<int>(nullable: false),
                    AccessId = table.Column<string>(nullable: false),
                    AccessSecret = table.Column<string>(nullable: true),
                    AccessToken = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationCMS", x => x.OrganizationCMSId);
                    table.ForeignKey(
                        name: "FK_OrganizationCMS_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationCPS",
                columns: table => new
                {
                    OrganizationCPSId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(maxLength: 20, nullable: false),
                    Type = table.Column<int>(nullable: false),
                    AccessId = table.Column<string>(nullable: false),
                    AccessName = table.Column<string>(nullable: true),
                    AccessSecret = table.Column<string>(nullable: true),
                    AccessAppId = table.Column<string>(nullable: true),
                    AccessAppSecret = table.Column<string>(nullable: true),
                    AccessDirectory = table.Column<string>(nullable: true),
                    AccessRegion = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationCPS", x => x.OrganizationCPSId);
                    table.ForeignKey(
                        name: "FK_OrganizationCPS_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationProjectServiceTemplate",
                columns: table => new
                {
                    OrganizationProjectServiceTemplateId = table.Column<Guid>(nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    ProjectServiceTemplateId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationProjectServiceTemplate", x => new { x.OrganizationProjectServiceTemplateId, x.OrganizationId, x.ProjectServiceTemplateId });
                    table.ForeignKey(
                        name: "FK_OrganizationProjectServiceTemplate_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationProjectServiceTemplate_ProjectServiceTemplate_ProjectServiceTemplateId",
                        column: x => x.ProjectServiceTemplateId,
                        principalTable: "ProjectServiceTemplate",
                        principalColumn: "ProjectServiceTemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationUser",
                columns: table => new
                {
                    OrganizationUserId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(maxLength: 450, nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Role = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationUser", x => new { x.OrganizationUserId, x.OrganizationId, x.UserId });
                    table.ForeignKey(
                        name: "FK_OrganizationUser_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationUser_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "OrganizationUserInvitation",
                columns: table => new
                {
                    OrganizationUserInvitationId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(maxLength: 450, nullable: true),
                    UserEmail = table.Column<string>(nullable: true),
                    Role = table.Column<int>(nullable: false),
                    InvitationType = table.Column<int>(nullable: false),
                    InvitationStatus = table.Column<int>(nullable: false),
                    AcceptedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_OrganizationUserInvitation", x => x.OrganizationUserInvitationId);
                    table.ForeignKey(
                        name: "FK_OrganizationUserInvitation_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_OrganizationUserInvitation_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Project",
                columns: table => new
                {
                    ProjectId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    InternalName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(maxLength: 100, nullable: false),
                    ProjectType = table.Column<int>(nullable: false),
                    ProjectVisibility = table.Column<int>(nullable: false),
                    OwnerId = table.Column<string>(maxLength: 450, nullable: false),
                    OrganizationExternalId = table.Column<string>(nullable: true),
                    OrganizationId = table.Column<Guid>(nullable: false),
                    OrganizationCMSId = table.Column<Guid>(nullable: false),
                    OrganizationCPSId = table.Column<Guid>(nullable: true),
                    ProjectTemplateId = table.Column<Guid>(nullable: true),
                    ProjectExternalId = table.Column<string>(nullable: true),
                    ProjectExternalName = table.Column<string>(nullable: true),
                    IsImported = table.Column<bool>(nullable: false),
                    ProjectVSTSFakeName = table.Column<string>(nullable: true),
                    ProjectVSTSFakeId = table.Column<string>(nullable: true),
                    ProjectExternalEndpointId = table.Column<string>(nullable: true),
                    ProjectExternalGitEndpoint = table.Column<string>(nullable: true),
                    AgentPoolId = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Project", x => x.ProjectId);
                    table.ForeignKey(
                        name: "FK_Project_OrganizationCMS_OrganizationCMSId",
                        column: x => x.OrganizationCMSId,
                        principalTable: "OrganizationCMS",
                        principalColumn: "OrganizationCMSId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Project_OrganizationCPS_OrganizationCPSId",
                        column: x => x.OrganizationCPSId,
                        principalTable: "OrganizationCPS",
                        principalColumn: "OrganizationCPSId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Project_Organization_OrganizationId",
                        column: x => x.OrganizationId,
                        principalTable: "Organization",
                        principalColumn: "OrganizationId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Project_User_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Project_ProjectTemplate_ProjectTemplateId",
                        column: x => x.ProjectTemplateId,
                        principalTable: "ProjectTemplate",
                        principalColumn: "ProjectTemplateId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectActivity",
                columns: table => new
                {
                    ProjectActivityId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Log = table.Column<string>(nullable: false),
                    ActivityStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectActivity", x => x.ProjectActivityId);
                    table.ForeignKey(
                        name: "FK_ProjectActivity_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectEnvironment",
                columns: table => new
                {
                    ProjectEnvironmentId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 100, nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Rank = table.Column<int>(nullable: false),
                    RequiresApproval = table.Column<bool>(nullable: false),
                    AutoProvision = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectEnvironment", x => x.ProjectEnvironmentId);
                    table.ForeignKey(
                        name: "FK_ProjectEnvironment_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectFeature",
                columns: table => new
                {
                    ProjectFeatureId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 100, nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    CompletionDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFeature", x => x.ProjectFeatureId);
                    table.ForeignKey(
                        name: "FK_ProjectFeature_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectService",
                columns: table => new
                {
                    ProjectServiceId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    InternalName = table.Column<string>(nullable: true),
                    Description = table.Column<string>(maxLength: 100, nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    ProjectServiceTemplateId = table.Column<Guid>(nullable: false),
                    PipeType = table.Column<int>(nullable: false),
                    ProjectServiceExternalId = table.Column<string>(nullable: true),
                    ProjectServiceExternalUrl = table.Column<string>(nullable: true),
                    ProjectServiceExternalName = table.Column<string>(nullable: true),
                    CommitStageId = table.Column<int>(nullable: true),
                    ReleaseStageId = table.Column<int>(nullable: true),
                    CommitServiceHookId = table.Column<Guid>(nullable: true),
                    ReleaseServiceHookId = table.Column<Guid>(nullable: true),
                    CodeServiceHookId = table.Column<Guid>(nullable: true),
                    ReleaseStartedServiceHookId = table.Column<Guid>(nullable: true),
                    ReleasePendingApprovalServiceHookId = table.Column<Guid>(nullable: true),
                    ReleaseCompletedApprovalServiceHookId = table.Column<Guid>(nullable: true),
                    PipelineStatus = table.Column<int>(nullable: false),
                    LastPipelineBuildStatus = table.Column<int>(nullable: false),
                    LastPipelineReleaseStatus = table.Column<int>(nullable: false),
                    LastBuildEventDate = table.Column<DateTime>(nullable: false),
                    LasReleaseEventDate = table.Column<DateTime>(nullable: false),
                    LastBuildVersionId = table.Column<string>(nullable: true),
                    LastBuildVersionName = table.Column<string>(nullable: true),
                    LastBuildSuccessVersionId = table.Column<string>(nullable: true),
                    LastBuildSuccessVersionName = table.Column<string>(nullable: true),
                    OrganizationCMSId = table.Column<Guid>(nullable: true),
                    BranchName = table.Column<string>(nullable: true),
                    IsImported = table.Column<bool>(nullable: false),
                    ProjectExternalName = table.Column<string>(nullable: true),
                    ProjectExternalId = table.Column<string>(nullable: true),
                    AgentPoolId = table.Column<string>(nullable: false),
                    ProjectBranchServiceExternalUrl = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectService", x => x.ProjectServiceId);
                    table.ForeignKey(
                        name: "FK_ProjectService_OrganizationCMS_OrganizationCMSId",
                        column: x => x.OrganizationCMSId,
                        principalTable: "OrganizationCMS",
                        principalColumn: "OrganizationCMSId",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ProjectService_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectService_ProjectServiceTemplate_ProjectServiceTemplateId",
                        column: x => x.ProjectServiceTemplateId,
                        principalTable: "ProjectServiceTemplate",
                        principalColumn: "ProjectServiceTemplateId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectUser",
                columns: table => new
                {
                    ProjectUserId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(maxLength: 450, nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Role = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUser", x => new { x.ProjectUserId, x.ProjectId, x.UserId });
                    table.ForeignKey(
                        name: "FK_ProjectUser_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectUser_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectUserInvitation",
                columns: table => new
                {
                    ProjectUserInvitationId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(maxLength: 450, nullable: true),
                    UserEmail = table.Column<string>(nullable: true),
                    Role = table.Column<int>(nullable: false),
                    InvitationType = table.Column<int>(nullable: false),
                    InvitationStatus = table.Column<int>(nullable: false),
                    AcceptedDate = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectUserInvitation", x => x.ProjectUserInvitationId);
                    table.ForeignKey(
                        name: "FK_ProjectUserInvitation_Project_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Project",
                        principalColumn: "ProjectId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectUserInvitation_User_UserId",
                        column: x => x.UserId,
                        principalTable: "User",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectEnvironmentVariable",
                columns: table => new
                {
                    ProjectEnvironmentId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectEnvironmentVariable", x => new { x.ProjectEnvironmentId, x.Name });
                    table.ForeignKey(
                        name: "FK_ProjectEnvironmentVariable_ProjectEnvironment_ProjectEnvironmentId",
                        column: x => x.ProjectEnvironmentId,
                        principalTable: "ProjectEnvironment",
                        principalColumn: "ProjectEnvironmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectFeatureEnvironment",
                columns: table => new
                {
                    ProjectFeatureEnvironmentId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Name = table.Column<string>(maxLength: 50, nullable: false),
                    Description = table.Column<string>(maxLength: 100, nullable: false),
                    ProjectFeatureId = table.Column<Guid>(nullable: false),
                    Type = table.Column<int>(nullable: false),
                    Rank = table.Column<int>(nullable: false),
                    RequiresApproval = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFeatureEnvironment", x => x.ProjectFeatureEnvironmentId);
                    table.ForeignKey(
                        name: "FK_ProjectFeatureEnvironment_ProjectFeature_ProjectFeatureId",
                        column: x => x.ProjectFeatureId,
                        principalTable: "ProjectFeature",
                        principalColumn: "ProjectFeatureId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectFeatureService",
                columns: table => new
                {
                    ProjectFeatureServiceId = table.Column<Guid>(nullable: false),
                    ProjectFeatureId = table.Column<Guid>(nullable: false),
                    ProjectServiceId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    CommitStageId = table.Column<int>(nullable: true),
                    ReleaseStageId = table.Column<int>(nullable: true),
                    CommitServiceHookId = table.Column<Guid>(nullable: true),
                    ReleaseServiceHookId = table.Column<Guid>(nullable: true),
                    CodeServiceHookId = table.Column<Guid>(nullable: true),
                    ReleaseStartedServiceHookId = table.Column<Guid>(nullable: true),
                    ReleasePendingApprovalServiceHookId = table.Column<Guid>(nullable: true),
                    ReleaseCompletedApprovalServiceHookId = table.Column<Guid>(nullable: true),
                    PipelineStatus = table.Column<int>(nullable: false),
                    LastPipelineBuildStatus = table.Column<int>(nullable: false),
                    LastPipelineReleaseStatus = table.Column<int>(nullable: false),
                    LastBuildEventDate = table.Column<DateTime>(nullable: false),
                    LasReleaseEventDate = table.Column<DateTime>(nullable: false),
                    LastBuildVersionId = table.Column<string>(nullable: true),
                    LastBuildVersionName = table.Column<string>(nullable: true),
                    LastBuildSuccessVersionId = table.Column<string>(nullable: true),
                    LastBuildSuccessVersionName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFeatureService", x => new { x.ProjectFeatureServiceId, x.ProjectFeatureId, x.ProjectServiceId });
                    table.ForeignKey(
                        name: "FK_ProjectFeatureService_ProjectFeature_ProjectFeatureId",
                        column: x => x.ProjectFeatureId,
                        principalTable: "ProjectFeature",
                        principalColumn: "ProjectFeatureId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectFeatureService_ProjectService_ProjectServiceId",
                        column: x => x.ProjectServiceId,
                        principalTable: "ProjectService",
                        principalColumn: "ProjectServiceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectServiceActivity",
                columns: table => new
                {
                    ProjectServiceActivityId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    ProjectServiceId = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Log = table.Column<string>(nullable: false),
                    ActivityStatus = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectServiceActivity", x => x.ProjectServiceActivityId);
                    table.ForeignKey(
                        name: "FK_ProjectServiceActivity_ProjectService_ProjectServiceId",
                        column: x => x.ProjectServiceId,
                        principalTable: "ProjectService",
                        principalColumn: "ProjectServiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectServiceDelivery",
                columns: table => new
                {
                    ProjectServiceDeliveryId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ProjectServiceId = table.Column<Guid>(nullable: false),
                    VersionId = table.Column<int>(nullable: false),
                    VersionName = table.Column<string>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    DeliveryDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectServiceDelivery", x => x.ProjectServiceDeliveryId);
                    table.ForeignKey(
                        name: "FK_ProjectServiceDelivery_ProjectService_ProjectServiceId",
                        column: x => x.ProjectServiceId,
                        principalTable: "ProjectService",
                        principalColumn: "ProjectServiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectServiceEnvironment",
                columns: table => new
                {
                    ProjectServiceEnvironmentId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ProjectEnvironmentId = table.Column<Guid>(nullable: false),
                    LastStatus = table.Column<string>(nullable: true),
                    LastStatusCode = table.Column<string>(nullable: true),
                    LastVersionId = table.Column<string>(nullable: true),
                    LastVersionName = table.Column<string>(nullable: true),
                    LastSuccessVersionId = table.Column<string>(nullable: true),
                    LastSuccessVersionName = table.Column<string>(nullable: true),
                    LastApprovalId = table.Column<string>(nullable: true),
                    LastEventDate = table.Column<DateTime>(nullable: false),
                    ProjectServiceId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectServiceEnvironment", x => x.ProjectServiceEnvironmentId);
                    table.ForeignKey(
                        name: "FK_ProjectServiceEnvironment_ProjectEnvironment_ProjectEnvironmentId",
                        column: x => x.ProjectEnvironmentId,
                        principalTable: "ProjectEnvironment",
                        principalColumn: "ProjectEnvironmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectServiceEnvironment_ProjectService_ProjectServiceId",
                        column: x => x.ProjectServiceId,
                        principalTable: "ProjectService",
                        principalColumn: "ProjectServiceId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectServiceEvent",
                columns: table => new
                {
                    ProjectServiceEventId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ProjectServiceId = table.Column<Guid>(nullable: false),
                    BaseEventType = table.Column<int>(nullable: false),
                    EventType = table.Column<string>(nullable: false),
                    EventDescription = table.Column<string>(nullable: false),
                    EventStatus = table.Column<string>(nullable: false),
                    EventMessage = table.Column<string>(nullable: false),
                    EventDetailedMessage = table.Column<string>(nullable: false),
                    EventResource = table.Column<string>(nullable: false),
                    EventDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectServiceEvent", x => x.ProjectServiceEventId);
                    table.ForeignKey(
                        name: "FK_ProjectServiceEvent_ProjectService_ProjectServiceId",
                        column: x => x.ProjectServiceId,
                        principalTable: "ProjectService",
                        principalColumn: "ProjectServiceId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectFeatureEnvironmentVariable",
                columns: table => new
                {
                    ProjectFeatureEnvironmentId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFeatureEnvironmentVariable", x => new { x.ProjectFeatureEnvironmentId, x.Name });
                    table.ForeignKey(
                        name: "FK_ProjectFeatureEnvironmentVariable_ProjectFeatureEnvironment_ProjectFeatureEnvironmentId",
                        column: x => x.ProjectFeatureEnvironmentId,
                        principalTable: "ProjectFeatureEnvironment",
                        principalColumn: "ProjectFeatureEnvironmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectFeatureServiceActivity",
                columns: table => new
                {
                    ProjectFeatureServiceActivityId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ProjectId = table.Column<Guid>(nullable: false),
                    ProjectFeatureId = table.Column<Guid>(nullable: false),
                    ProjectServiceId = table.Column<Guid>(nullable: false),
                    Code = table.Column<string>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Log = table.Column<string>(nullable: false),
                    ActivityStatus = table.Column<int>(nullable: false),
                    ProjectFeatureServiceId = table.Column<Guid>(nullable: true),
                    ProjectFeatureServiceProjectFeatureId = table.Column<Guid>(nullable: true),
                    ProjectFeatureServiceProjectServiceId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFeatureServiceActivity", x => x.ProjectFeatureServiceActivityId);
                    table.ForeignKey(
                        name: "FK_ProjectFeatureServiceActivity_ProjectFeatureService_ProjectFeatureServiceId_ProjectFeatureServiceProjectFeatureId_ProjectFea~",
                        columns: x => new { x.ProjectFeatureServiceId, x.ProjectFeatureServiceProjectFeatureId, x.ProjectFeatureServiceProjectServiceId },
                        principalTable: "ProjectFeatureService",
                        principalColumns: new[] { "ProjectFeatureServiceId", "ProjectFeatureId", "ProjectServiceId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectFeatureServiceDelivery",
                columns: table => new
                {
                    ProjectFeatureServiceDeliveryId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ProjectFeatureServiceId = table.Column<Guid>(nullable: false),
                    ProjectFeatureServiceId1 = table.Column<Guid>(nullable: true),
                    ProjectFeatureServiceProjectFeatureId = table.Column<Guid>(nullable: true),
                    ProjectFeatureServiceProjectServiceId = table.Column<Guid>(nullable: true),
                    VersionId = table.Column<int>(nullable: false),
                    VersionName = table.Column<string>(nullable: false),
                    Data = table.Column<string>(nullable: true),
                    DeliveryDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFeatureServiceDelivery", x => x.ProjectFeatureServiceDeliveryId);
                    table.ForeignKey(
                        name: "FK_ProjectFeatureServiceDelivery_ProjectFeatureService_ProjectFeatureServiceId1_ProjectFeatureServiceProjectFeatureId_ProjectFe~",
                        columns: x => new { x.ProjectFeatureServiceId1, x.ProjectFeatureServiceProjectFeatureId, x.ProjectFeatureServiceProjectServiceId },
                        principalTable: "ProjectFeatureService",
                        principalColumns: new[] { "ProjectFeatureServiceId", "ProjectFeatureId", "ProjectServiceId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectFeatureServiceEnvironment",
                columns: table => new
                {
                    ProjectFeatureServiceEnvironmentId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ProjectFeatureEnvironmentId = table.Column<Guid>(nullable: false),
                    LastStatus = table.Column<string>(nullable: true),
                    LastStatusCode = table.Column<string>(nullable: true),
                    LastVersionId = table.Column<string>(nullable: true),
                    LastVersionName = table.Column<string>(nullable: true),
                    LastSuccessVersionId = table.Column<string>(nullable: true),
                    LastSuccessVersionName = table.Column<string>(nullable: true),
                    LastApprovalId = table.Column<string>(nullable: true),
                    LastEventDate = table.Column<DateTime>(nullable: false),
                    ProjectFeatureServiceId = table.Column<Guid>(nullable: true),
                    ProjectFeatureServiceProjectFeatureId = table.Column<Guid>(nullable: true),
                    ProjectFeatureServiceProjectServiceId = table.Column<Guid>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFeatureServiceEnvironment", x => x.ProjectFeatureServiceEnvironmentId);
                    table.ForeignKey(
                        name: "FK_ProjectFeatureServiceEnvironment_ProjectFeatureEnvironment_ProjectFeatureEnvironmentId",
                        column: x => x.ProjectFeatureEnvironmentId,
                        principalTable: "ProjectFeatureEnvironment",
                        principalColumn: "ProjectFeatureEnvironmentId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ProjectFeatureServiceEnvironment_ProjectFeatureService_ProjectFeatureServiceId_ProjectFeatureServiceProjectFeatureId_Project~",
                        columns: x => new { x.ProjectFeatureServiceId, x.ProjectFeatureServiceProjectFeatureId, x.ProjectFeatureServiceProjectServiceId },
                        principalTable: "ProjectFeatureService",
                        principalColumns: new[] { "ProjectFeatureServiceId", "ProjectFeatureId", "ProjectServiceId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectFeatureServiceEvent",
                columns: table => new
                {
                    ProjectFeatureServiceEventId = table.Column<Guid>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    ProjectFeatureServiceId = table.Column<Guid>(nullable: false),
                    ProjectFeatureServiceId1 = table.Column<Guid>(nullable: true),
                    ProjectFeatureServiceProjectFeatureId = table.Column<Guid>(nullable: true),
                    ProjectFeatureServiceProjectServiceId = table.Column<Guid>(nullable: true),
                    BaseEventType = table.Column<int>(nullable: false),
                    EventType = table.Column<string>(nullable: false),
                    EventDescription = table.Column<string>(nullable: false),
                    EventStatus = table.Column<string>(nullable: false),
                    EventMessage = table.Column<string>(nullable: false),
                    EventDetailedMessage = table.Column<string>(nullable: false),
                    EventResource = table.Column<string>(nullable: false),
                    EventDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFeatureServiceEvent", x => x.ProjectFeatureServiceEventId);
                    table.ForeignKey(
                        name: "FK_ProjectFeatureServiceEvent_ProjectFeatureService_ProjectFeatureServiceId1_ProjectFeatureServiceProjectFeatureId_ProjectFeatu~",
                        columns: x => new { x.ProjectFeatureServiceId1, x.ProjectFeatureServiceProjectFeatureId, x.ProjectFeatureServiceProjectServiceId },
                        principalTable: "ProjectFeatureService",
                        principalColumns: new[] { "ProjectFeatureServiceId", "ProjectFeatureId", "ProjectServiceId" },
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProjectServiceEnvironmentVariable",
                columns: table => new
                {
                    ProjectServiceEnvironmentId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectServiceEnvironmentVariable", x => new { x.ProjectServiceEnvironmentId, x.Name });
                    table.ForeignKey(
                        name: "FK_ProjectServiceEnvironmentVariable_ProjectServiceEnvironment_ProjectServiceEnvironmentId",
                        column: x => x.ProjectServiceEnvironmentId,
                        principalTable: "ProjectServiceEnvironment",
                        principalColumn: "ProjectServiceEnvironmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "ProjectFeatureServiceEnvironmentVariable",
                columns: table => new
                {
                    ProjectFeatureServiceEnvironmentId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    CreationDate = table.Column<DateTime>(nullable: false),
                    CreatedBy = table.Column<string>(maxLength: 450, nullable: false),
                    LastModificationDate = table.Column<DateTime>(nullable: true),
                    ModifiedBy = table.Column<string>(maxLength: 450, nullable: true),
                    InactivationDate = table.Column<DateTime>(nullable: true),
                    InactivatedBy = table.Column<string>(maxLength: 450, nullable: true),
                    DeletionDate = table.Column<DateTime>(nullable: true),
                    DeletedBy = table.Column<string>(maxLength: 450, nullable: true),
                    Status = table.Column<int>(nullable: false),
                    Value = table.Column<string>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProjectFeatureServiceEnvironmentVariable", x => new { x.ProjectFeatureServiceEnvironmentId, x.Name });
                    table.ForeignKey(
                        name: "FK_ProjectFeatureServiceEnvironmentVariable_ProjectFeatureServiceEnvironment_ProjectFeatureServiceEnvironmentId",
                        column: x => x.ProjectFeatureServiceEnvironmentId,
                        principalTable: "ProjectFeatureServiceEnvironment",
                        principalColumn: "ProjectFeatureServiceEnvironmentId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Organization_OwnerId",
                table: "Organization",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationCMS_OrganizationId",
                table: "OrganizationCMS",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationCPS_OrganizationId",
                table: "OrganizationCPS",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationProjectServiceTemplate_OrganizationId",
                table: "OrganizationProjectServiceTemplate",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationProjectServiceTemplate_ProjectServiceTemplateId",
                table: "OrganizationProjectServiceTemplate",
                column: "ProjectServiceTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUser_OrganizationId",
                table: "OrganizationUser",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUser_UserId",
                table: "OrganizationUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUserInvitation_OrganizationId",
                table: "OrganizationUserInvitation",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_OrganizationUserInvitation_UserId",
                table: "OrganizationUserInvitation",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_OrganizationCMSId",
                table: "Project",
                column: "OrganizationCMSId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_OrganizationCPSId",
                table: "Project",
                column: "OrganizationCPSId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_OrganizationId",
                table: "Project",
                column: "OrganizationId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_OwnerId",
                table: "Project",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_Project_ProjectTemplateId",
                table: "Project",
                column: "ProjectTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectActivity_ProjectId",
                table: "ProjectActivity",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectEnvironment_ProjectId",
                table: "ProjectEnvironment",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFeature_ProjectId",
                table: "ProjectFeature",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFeatureEnvironment_ProjectFeatureId",
                table: "ProjectFeatureEnvironment",
                column: "ProjectFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFeatureService_ProjectFeatureId",
                table: "ProjectFeatureService",
                column: "ProjectFeatureId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFeatureService_ProjectServiceId",
                table: "ProjectFeatureService",
                column: "ProjectServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFeatureServiceActivity_ProjectFeatureServiceId_ProjectFeatureServiceProjectFeatureId_ProjectFeatureServiceProjectServ~",
                table: "ProjectFeatureServiceActivity",
                columns: new[] { "ProjectFeatureServiceId", "ProjectFeatureServiceProjectFeatureId", "ProjectFeatureServiceProjectServiceId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFeatureServiceDelivery_ProjectFeatureServiceId1_ProjectFeatureServiceProjectFeatureId_ProjectFeatureServiceProjectSer~",
                table: "ProjectFeatureServiceDelivery",
                columns: new[] { "ProjectFeatureServiceId1", "ProjectFeatureServiceProjectFeatureId", "ProjectFeatureServiceProjectServiceId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFeatureServiceEnvironment_ProjectFeatureEnvironmentId",
                table: "ProjectFeatureServiceEnvironment",
                column: "ProjectFeatureEnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFeatureServiceEnvironment_ProjectFeatureServiceId_ProjectFeatureServiceProjectFeatureId_ProjectFeatureServiceProjectS~",
                table: "ProjectFeatureServiceEnvironment",
                columns: new[] { "ProjectFeatureServiceId", "ProjectFeatureServiceProjectFeatureId", "ProjectFeatureServiceProjectServiceId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectFeatureServiceEvent_ProjectFeatureServiceId1_ProjectFeatureServiceProjectFeatureId_ProjectFeatureServiceProjectServic~",
                table: "ProjectFeatureServiceEvent",
                columns: new[] { "ProjectFeatureServiceId1", "ProjectFeatureServiceProjectFeatureId", "ProjectFeatureServiceProjectServiceId" });

            migrationBuilder.CreateIndex(
                name: "IX_ProjectService_OrganizationCMSId",
                table: "ProjectService",
                column: "OrganizationCMSId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectService_ProjectId",
                table: "ProjectService",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectService_ProjectServiceTemplateId",
                table: "ProjectService",
                column: "ProjectServiceTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectServiceActivity_ProjectServiceId",
                table: "ProjectServiceActivity",
                column: "ProjectServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectServiceDelivery_ProjectServiceId",
                table: "ProjectServiceDelivery",
                column: "ProjectServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectServiceEnvironment_ProjectEnvironmentId",
                table: "ProjectServiceEnvironment",
                column: "ProjectEnvironmentId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectServiceEnvironment_ProjectServiceId",
                table: "ProjectServiceEnvironment",
                column: "ProjectServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectServiceEvent_ProjectServiceId",
                table: "ProjectServiceEvent",
                column: "ProjectServiceId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectServiceTemplate_ProgrammingLanguageId",
                table: "ProjectServiceTemplate",
                column: "ProgrammingLanguageId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectServiceTemplateParameter_ProjectServiceTemplateId",
                table: "ProjectServiceTemplateParameter",
                column: "ProjectServiceTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectTemplateService_ProjectServiceTemplateId",
                table: "ProjectTemplateService",
                column: "ProjectServiceTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUser_ProjectId",
                table: "ProjectUser",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUser_UserId",
                table: "ProjectUser",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUserInvitation_ProjectId",
                table: "ProjectUserInvitation",
                column: "ProjectId");

            migrationBuilder.CreateIndex(
                name: "IX_ProjectUserInvitation_UserId",
                table: "ProjectUserInvitation",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "OrganizationProjectServiceTemplate");

            migrationBuilder.DropTable(
                name: "OrganizationUser");

            migrationBuilder.DropTable(
                name: "OrganizationUserInvitation");

            migrationBuilder.DropTable(
                name: "ProjectActivity");

            migrationBuilder.DropTable(
                name: "ProjectEnvironmentVariable");

            migrationBuilder.DropTable(
                name: "ProjectFeatureEnvironmentVariable");

            migrationBuilder.DropTable(
                name: "ProjectFeatureServiceActivity");

            migrationBuilder.DropTable(
                name: "ProjectFeatureServiceDelivery");

            migrationBuilder.DropTable(
                name: "ProjectFeatureServiceEnvironmentVariable");

            migrationBuilder.DropTable(
                name: "ProjectFeatureServiceEvent");

            migrationBuilder.DropTable(
                name: "ProjectServiceActivity");

            migrationBuilder.DropTable(
                name: "ProjectServiceDelivery");

            migrationBuilder.DropTable(
                name: "ProjectServiceEnvironmentVariable");

            migrationBuilder.DropTable(
                name: "ProjectServiceEvent");

            migrationBuilder.DropTable(
                name: "ProjectServiceTemplateCredential");

            migrationBuilder.DropTable(
                name: "ProjectServiceTemplateParameter");

            migrationBuilder.DropTable(
                name: "ProjectTemplateService");

            migrationBuilder.DropTable(
                name: "ProjectUser");

            migrationBuilder.DropTable(
                name: "ProjectUserInvitation");

            migrationBuilder.DropTable(
                name: "ProjectFeatureServiceEnvironment");

            migrationBuilder.DropTable(
                name: "ProjectServiceEnvironment");

            migrationBuilder.DropTable(
                name: "ProjectFeatureEnvironment");

            migrationBuilder.DropTable(
                name: "ProjectFeatureService");

            migrationBuilder.DropTable(
                name: "ProjectEnvironment");

            migrationBuilder.DropTable(
                name: "ProjectFeature");

            migrationBuilder.DropTable(
                name: "ProjectService");

            migrationBuilder.DropTable(
                name: "Project");

            migrationBuilder.DropTable(
                name: "ProjectServiceTemplate");

            migrationBuilder.DropTable(
                name: "OrganizationCMS");

            migrationBuilder.DropTable(
                name: "OrganizationCPS");

            migrationBuilder.DropTable(
                name: "ProjectTemplate");

            migrationBuilder.DropTable(
                name: "ProgrammingLanguage");

            migrationBuilder.DropTable(
                name: "Organization");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
