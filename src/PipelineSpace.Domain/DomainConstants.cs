using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Domain
{
    public class DomainConstants
    {
        public static class Obfuscator
        {
            public const string Default = "********************";
        }

        public static class Roles
        {
            public const string GlobalAdmin = "globaladmin";
            public const string OrganizationAdmin = "organizationadmin";
            public const string ProjectAdmin = "projectadmin";
            public const string OrganizationContributor = "organizationcontributor";
            public const string ProjectContributor = "projectcontributor";
        }

        public static class Plans
        {
            public static Guid Free = Guid.Parse("77289EA3-443A-41B3-A50F-2F975D5CFB34");
            public static Guid Admin = Guid.Parse("FB870D7E-E3AB-460E-8870-35CC97276554");
        }

        public static class Environments
        {
            public static string Development = "Development";
            public static string Production = "Production";
        }

        public static class Activities
        {
            public static string PRCRBA = "Configure project";
            public static string PRCLEP = "Create project cloud service connection";
            public static string PRGTEP = "Create project git service connection";
            public static string PREXBA = "Install project extension (AWS)";
            public static string PREXBO = "Install project extension (ARM Outputs)";
            public static string PREXGL = "Install project extension (GitLab)";
            public static string PRACBA = "Activate project";
            public static string PRSTPT = "Set project template";
            public static string PRSTIR = "Import repositories";

            public static string PSPRRQ = "Configure pipe";
            public static string PSCRRP = "Push files to repository";
            public static string PSCRBR = "Create branch";
            public static string PSCRBD = "Create build definition";
            public static string PSCRRD = "Create release definition";
            public static string PSQUDB = "Queue build";
            public static string PSACBA = "Update pipe";
        }
    }
}
