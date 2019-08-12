using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using PipelineSpace.Domain.Models;
using PipelineSpace.Infra.Data.SqlServer.Extensions;

namespace PipelineSpace.Infra.Data.SqlServer.Contexts
{
    public class PipelineSpaceDbContext : DbContext
    {
        public PipelineSpaceDbContext(DbContextOptions options) :
           base(options)
        {
            
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Organization> Organizations { get; set; }
        public DbSet<Project> Projects { get; set; }
        public DbSet<ProjectService> ProjectServices { get; set; }
        public DbSet<ProjectFeature> ProjectFeatures { get; set; }
        public DbSet<ProjectFeatureService> ProjectFeatureServices { get; set; }
        public DbSet<ProjectEnvironment> ProjectEnvironments { get; set; }
        public DbSet<ProjectActivity> ProjectActivities { get; set; }
        public DbSet<ProjectServiceActivity> ProjectServiceActivities { get; set; }
        public DbSet<ProjectFeatureServiceActivity> ProjectFeatureServiceActivities { get; set; }
        
        public DbSet<ProjectTemplate> ProjectTemplates { get; set; }
        public DbSet<ProjectServiceTemplate> ProjectServiceTemplates { get; set; }

        public DbSet<OrganizationCMS> OrganizationCMSs { get; set; }
        public DbSet<OrganizationCPS> OrganizationCPSs { get; set; }

        public DbSet<OrganizationUserInvitation> OrganizationUserInvitations { get; set; }
        public DbSet<ProjectUserInvitation> ProjectUserInvitations { get; set; }

        public DbSet<ProgrammingLanguage> ProgrammingLanguages { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Organization>()
               .HasOne(p => p.Owner)
               .WithMany(b=> b.Organizations)
               .IsRequired().OnDelete(DeleteBehavior.Restrict)
               .HasForeignKey(p => p.OwnerId);

            modelBuilder.Entity<Project>()
               .HasOne(p => p.Owner)
               .WithMany(b => b.Projects)
               .IsRequired().OnDelete(DeleteBehavior.Restrict)
               .HasForeignKey(p => p.OwnerId);

            modelBuilder.Entity<OrganizationUser>().HasKey(x => new { x.OrganizationUserId, x.OrganizationId, x.UserId });

            modelBuilder.Entity<OrganizationUser>()
               .HasOne(pt => pt.Organization)
               .WithMany(p => p.Users)
               .HasForeignKey(pt => pt.OrganizationId);

            modelBuilder.Entity<OrganizationUser>()
               .HasOne(pt => pt.User)
               .WithMany(p => p.OrganizationUsers)
               .HasForeignKey(pt => pt.UserId);

            modelBuilder.Entity<ProjectUser>().HasKey(x => new { x.ProjectUserId, x.ProjectId, x.UserId });

            modelBuilder.Entity<ProjectUser>()
               .HasOne(pt => pt.Project)
               .WithMany(p => p.Users)
               .HasForeignKey(pt => pt.ProjectId);

            modelBuilder.Entity<ProjectUser>()
               .HasOne(pt => pt.User)
               .WithMany(p => p.ProjectUsers)
               .HasForeignKey(pt => pt.UserId);

            modelBuilder.Entity<ProjectService>()
            .HasOne(p => p.ProjectServiceTemplate)
            .WithMany(p => p.ProjectServices)
            .HasForeignKey(p => p.ProjectServiceTemplateId);

            modelBuilder.Entity<OrganizationCMS>()
            .HasOne(p => p.Organization)
            .WithMany(p => p.ConfigurationManagementServices)
            .HasForeignKey(p => p.OrganizationId);

            modelBuilder.Entity<OrganizationCPS>()
            .HasOne(p => p.Organization)
            .WithMany(p => p.CloudProviderServices)
            .HasForeignKey(p => p.OrganizationId);
            
            modelBuilder.Entity<Project>()
            .HasOne(p => p.OrganizationCMS)
            .WithMany(p => p.Projects)
            .IsRequired().OnDelete(DeleteBehavior.Restrict)
            .HasForeignKey(p => p.OrganizationCMSId);

            modelBuilder.Entity<Project>()
            .HasOne(p => p.OrganizationCPS)
            .WithMany(p => p.Projects)
            .OnDelete(DeleteBehavior.Restrict)
            .HasForeignKey(p => p.OrganizationCPSId);
            
            modelBuilder.Entity<ProjectFeatureService>().HasKey(x => new { x.ProjectFeatureServiceId, x.ProjectFeatureId, x.ProjectServiceId });

            modelBuilder.Entity<ProjectFeatureService>()
               .HasOne(pt => pt.ProjectFeature)
               .WithMany(p => p.Services)
               .HasForeignKey(pt => pt.ProjectFeatureId);

            modelBuilder.Entity<ProjectFeatureService>()
               .HasOne(pt => pt.ProjectService)
               .WithMany(p => p.Features)
               .OnDelete(DeleteBehavior.Restrict)
               .HasForeignKey(pt => pt.ProjectServiceId);

            modelBuilder.Entity<ProjectEnvironmentVariable>().HasKey(x => new { x.ProjectEnvironmentId, x.Name });

            modelBuilder.Entity<ProjectTemplateService>().HasKey(x => new { x.ProjectTemplateId, x.ProjectServiceTemplateId, x.Name });

            modelBuilder.Entity<ProjectTemplateService>()
               .HasOne(pt => pt.ProjectTemplate)
               .WithMany(p => p.Services)
               .HasForeignKey(pt => pt.ProjectTemplateId);

            modelBuilder.Entity<ProjectTemplateService>()
               .HasOne(pt => pt.ProjectServiceTemplate)
               .WithMany(p => p.ProjectTemplateServices)
               .OnDelete(DeleteBehavior.Restrict)
               .HasForeignKey(pt => pt.ProjectServiceTemplateId);

            modelBuilder.Entity<ProjectEnvironmentVariable>().HasKey(x => new { x.ProjectEnvironmentId, x.Name });

            modelBuilder.Entity<ProjectServiceEnvironmentVariable>().HasKey(x => new { x.ProjectServiceEnvironmentId, x.Name });

            modelBuilder.Entity<ProjectFeatureEnvironmentVariable>().HasKey(x => new { x.ProjectFeatureEnvironmentId, x.Name });

            modelBuilder.Entity<ProjectFeatureServiceEnvironmentVariable>().HasKey(x => new { x.ProjectFeatureServiceEnvironmentId, x.Name });

            //OrganizationProjectServiceTemplate
            modelBuilder.Entity<OrganizationProjectServiceTemplate>().HasKey(x => new { x.OrganizationProjectServiceTemplateId, x.OrganizationId, x.ProjectServiceTemplateId });

            modelBuilder.Entity<OrganizationProjectServiceTemplate>()
               .HasOne(pt => pt.ProjectServiceTemplate)
               .WithMany(p => p.Organizations)
               .HasForeignKey(pt => pt.ProjectServiceTemplateId);

            modelBuilder.Entity<OrganizationProjectServiceTemplate>()
               .HasOne(pt => pt.Organization)
               .WithMany(p => p.ProjectServiceTemplates)
               .HasForeignKey(pt => pt.OrganizationId);

            modelBuilder.Entity<ProjectServiceTemplate>()
               .HasOne(pt => pt.Credential)
               .WithOne();

            //Query Filters 
            modelBuilder.Entity<Organization>().HasQueryFilter(p => p.Status != EntityStatus.Deleted);
            modelBuilder.Entity<OrganizationCMS>().HasQueryFilter(p => p.Status != EntityStatus.Deleted);
            modelBuilder.Entity<OrganizationCPS>().HasQueryFilter(p => p.Status != EntityStatus.Deleted);
            modelBuilder.Entity<Project>().HasQueryFilter(p => p.Status != EntityStatus.Deleted && p.Organization.Status != EntityStatus.Deleted);
            modelBuilder.Entity<ProjectService>().HasQueryFilter(p => p.Status != EntityStatus.Deleted && p.Project.Status != EntityStatus.Deleted);
            modelBuilder.Entity<ProjectEnvironment>().HasQueryFilter(p => p.Status != EntityStatus.Deleted && p.Project.Status != EntityStatus.Deleted);
            modelBuilder.Entity<ProjectFeature>().HasQueryFilter(p => p.Status != EntityStatus.Deleted && p.Project.Status != EntityStatus.Deleted);
            modelBuilder.Entity<ProjectFeatureService>().HasQueryFilter(p => p.Status != EntityStatus.Deleted && p.ProjectFeature.Status != EntityStatus.Deleted);
            modelBuilder.Entity<ProjectTemplate>().HasQueryFilter(p => p.Status != EntityStatus.Deleted);
            modelBuilder.Entity<ProjectServiceTemplate>().HasQueryFilter(p => p.Status != EntityStatus.Deleted);
            modelBuilder.Entity<OrganizationUser>().HasQueryFilter(p => p.Status != EntityStatus.Deleted && p.Organization.Status != EntityStatus.Deleted);
            modelBuilder.Entity<ProjectUser>().HasQueryFilter(p => p.Status != EntityStatus.Deleted && p.Project.Status != EntityStatus.Deleted && p.Project.Organization.Status != EntityStatus.Deleted);

            modelBuilder.Entity<ProjectServiceEnvironment>().HasQueryFilter(p => p.Status != EntityStatus.Deleted && p.ProjectEnvironment.Status != EntityStatus.Deleted);

            modelBuilder.Entity<ProjectFeatureServiceEnvironment>().HasQueryFilter(p => p.Status != EntityStatus.Deleted && p.ProjectFeatureEnvironment.Status != EntityStatus.Deleted);

            modelBuilder.Entity<OrganizationProjectServiceTemplate>().HasQueryFilter(p => p.Status != EntityStatus.Deleted);
            
            modelBuilder.RemovePluralizingTableNameConvention();
            base.OnModelCreating(modelBuilder);
        }
    }
}
