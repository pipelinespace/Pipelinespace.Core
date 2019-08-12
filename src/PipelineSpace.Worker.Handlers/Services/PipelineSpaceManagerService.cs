using LibGit2Sharp;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using PipelineSpace.Domain;
using PipelineSpace.Domain.Models;
using PipelineSpace.Worker.Handlers.Extensions;
using PipelineSpace.Worker.Handlers.Models;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using PipelineSpace.Worker.Monitor;
using Polly;
using Polly.Retry;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Services
{
    public class PipelineSpaceManagerService : IPipelineSpaceManagerService
    {
        readonly IHttpClientWrapperService _httpClientWrapperService;
        readonly RetryPolicy<HttpResponseMessage> _httpRetryPolicy;

        public PipelineSpaceManagerService(IHttpClientWrapperService httpClientWrapperService)
        {
            _httpClientWrapperService = httpClientWrapperService;
            _httpRetryPolicy = Policy.HandleResult<HttpResponseMessage>(r => r.StatusCode == System.Net.HttpStatusCode.Forbidden).WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt) / 2));
        }

        public async Task<string> CreateRepository(CreateRepositoryOptions @options)
        {
            /*PUSH REPOSITORY*******************************************************************************************/

            //Create local folder for the new repository
            string repositoryPath = $"{Path.GetTempPath()}\\{Guid.NewGuid().ToString("n")}\\{@options.ProjectName}\\{@options.RepositoryName}";
            Directory.CreateDirectory(repositoryPath);
            var folder = new DirectoryInfo(repositoryPath);

            //Git - Clone 
            var cloneOptions = new CloneOptions();
            if (@options.TemplateAccess == Domain.Models.Enums.TemplateAccess.System)
            {
                cloneOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.VSTSAccessId, Password = @options.VSTSAccessSecret };
            }
            else
            {
                if (@options.NeedCredentials)
                {
                    if (@options.RepositoryCMSType == ConfigurationManagementService.VSTS 
                        || @options.RepositoryCMSType == ConfigurationManagementService.GitLab
                        || @options.RepositoryCMSType == ConfigurationManagementService.Bitbucket)
                    {
                        cloneOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.RepositoryAccessId, Password = @options.RepositoryAccessSecret };
                    }
                    
                    if (@options.RepositoryCMSType == ConfigurationManagementService.GitHub)
                    {
                        cloneOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.RepositoryAccessToken };
                    }
                }
            }
            Repository.Clone(@options.VSTSRepositoryTemplateUrl, repositoryPath, cloneOptions);

            //Modify the repository
            DirectoryManagerExtensions.DeleteDirectory($"{repositoryPath}\\.git");
            DirectoryManagerExtensions.RenameDirectory(repositoryPath, @options.OrganizationName, @options.ProjectName, @options.ServiceName);

            //Retreiving release definition file
            string releaseDefinition = string.Empty;
            if (File.Exists($"{repositoryPath}\\release\\definition.json"))
            {
                releaseDefinition = File.ReadAllText($"{repositoryPath}\\release\\definition.json");
                DirectoryManagerExtensions.DeleteDirectory($"{repositoryPath}\\release");
            }
            else {
                if (File.Exists($"{repositoryPath}\\release-definition\\definition.json"))
                {
                    releaseDefinition = File.ReadAllText($"{repositoryPath}\\release-definition\\definition.json");
                    DirectoryManagerExtensions.DeleteDirectory($"{repositoryPath}\\release-definition");
                }
            }
            
            
            //Git - Init 
            Repository.Init(folder.FullName);

            //Git - Stage 
            using (var repo = new Repository(folder.FullName))
            {
                Commands.Stage(repo, "*");
            }

            //Git - Commit
            using (var repo = new Repository(folder.FullName))
            {
                // Create the committer's signature and commit
                Signature author = new Signature(@options.GitProviderAccessId, @options.GitProviderAccessId, DateTime.Now);
                Signature committer = author;

                // Commit to the repository
                Commit commit = repo.Commit("Initial", author, committer);
            }

            //Git - Push
            using (var repo = new Repository(folder.FullName))
            {
                var remote = repo.Network.Remotes.FirstOrDefault(r => r.Name == @options.GitProviderAccessId);
                if (remote == null)
                {
                    repo.Network.Remotes.Add(@options.GitProviderAccessId, @options.GitProviderRepositoryUrl);
                    remote = repo.Network.Remotes.FirstOrDefault(r => r.Name == @options.GitProviderAccessId);
                }

                var pushOptions = new PushOptions();

                if (@options.GitProviderType == ConfigurationManagementService.VSTS 
                    || @options.GitProviderType == ConfigurationManagementService.Bitbucket
                    || @options.GitProviderType == ConfigurationManagementService.GitLab)
                {
                    pushOptions.CredentialsProvider = (_url, _user, _cred) =>
                        new UsernamePasswordCredentials { Username = @options.GitProviderAccessId, Password = @options.GitProviderAccessSecret };
                }
                else
                {
                    pushOptions.CredentialsProvider = (_url, _user, _cred) =>
                        new UsernamePasswordCredentials { Username = @options.GitProviderAccessToken, Password = string.Empty };
                }

                repo.Network.Push(remote, options.Branch, pushOptions);
            }

            //delete local folder for the repository
            DirectoryManagerExtensions.DeleteDirectory(repositoryPath);

            return releaseDefinition;
        }

        public async Task CreateOrganizationRepository(CreateOrganizationRepositoryOptions @options)
        {
            /*PUSH REPOSITORY*******************************************************************************************/

            //Create local folder for the new repository
            string repositoryPath = $"{Path.GetTempPath()}\\{Guid.NewGuid().ToString("n")}";
            Directory.CreateDirectory(repositoryPath);
            var folder = new DirectoryInfo(repositoryPath);

            //Git - Clone 
            var cloneOptions = new CloneOptions();
            cloneOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.VSTSAccessId, Password = @options.VSTSAccessSecret };
            Repository.Clone(@options.VSTSRepositoryTemplateUrl, repositoryPath, cloneOptions);

            //Modify the repository
            DirectoryManagerExtensions.DeleteDirectory($"{repositoryPath}\\.git");
            
            //Git - Init 
            Repository.Init(folder.FullName);

            //Git - Stage 
            using (var repo = new Repository(folder.FullName))
            {
                Commands.Stage(repo, "*");
            }

            //Git - Commit
            using (var repo = new Repository(folder.FullName))
            {
                // Create the committer's signature and commit
                Signature author = new Signature(@options.RepositoryAccessId, @options.RepositoryAccessId, DateTime.Now);
                Signature committer = author;

                // Commit to the repository
                Commit commit = repo.Commit("Initial", author, committer);
            }

            //Git - Push
            using (var repo = new Repository(folder.FullName))
            {
                var remote = repo.Network.Remotes.FirstOrDefault(r => r.Name == @options.RepositoryAccessId);
                if (remote == null)
                {
                    repo.Network.Remotes.Add(@options.RepositoryAccessId, @options.RepositoryUrl);
                    remote = repo.Network.Remotes.FirstOrDefault(r => r.Name == @options.RepositoryAccessId);
                }

                var pushOptions = new PushOptions();

                if (@options.NeedCredentials)
                {
                    if (@options.RepositoryCMSType == ConfigurationManagementService.VSTS || @options.RepositoryCMSType == ConfigurationManagementService.Bitbucket)
                    {
                        pushOptions.CredentialsProvider = (_url, _user, _cred) =>
                            new UsernamePasswordCredentials { Username = @options.RepositoryAccessId, Password = @options.RepositoryAccessSecret };
                    }
                    else
                    {
                        pushOptions.CredentialsProvider = (_url, _user, _cred) =>
                            new UsernamePasswordCredentials { Username = @options.RepositoryAccessToken, Password = string.Empty };
                    }
                }
                
                repo.Network.Push(remote, options.Branch, pushOptions);
            }

            //delete local folder for the repository
            DirectoryManagerExtensions.DeleteDirectory(repositoryPath);
        }

        public async Task<string> CreateBranch(CreateBranchOptions @options)
        {
            /*PUSH REPOSITORY*******************************************************************************************/

            //Create local folder for the new repository
            string repositoryPath = $"{Path.GetTempPath()}\\{Guid.NewGuid().ToString("n")}\\{@options.ProjectName}\\{@options.ServiceName}";
            Directory.CreateDirectory(repositoryPath);
            var folder = new DirectoryInfo(repositoryPath);

            //Git - Clone 
            var cloneOptions = new CloneOptions();
            if (@options.TemplateAccess == Domain.Models.Enums.TemplateAccess.System && !@options.IsImported)
            {
                cloneOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.VSTSAccessId, Password = @options.VSTSAccessSecret };
            }
            else
            {
                if (@options.NeedCredentials)
                {
                    if (@options.RepositoryCMSType == ConfigurationManagementService.VSTS)
                    {
                        cloneOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.RepositoryAccessId, Password = @options.RepositoryAccessSecret };
                    }

                    if (@options.RepositoryCMSType == ConfigurationManagementService.GitHub)
                    {
                        cloneOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.RepositoryAccessToken };
                    }
                }
            }
            Repository.Clone(@options.VSTSRepositoryTemplateUrl, repositoryPath, cloneOptions);

            //Retreiving release definition file
            string releaseDefinition = string.Empty;
            if (File.Exists($"{repositoryPath}\\release\\definition.json"))
            {
                releaseDefinition = File.ReadAllText($"{repositoryPath}\\release\\definition.json");
                DirectoryManagerExtensions.DeleteDirectory($"{repositoryPath}\\release");
            }
            else
            {
                if (File.Exists($"{repositoryPath}\\release-definition\\definition.json"))
                {
                    releaseDefinition = File.ReadAllText($"{repositoryPath}\\release-definition\\definition.json");
                    DirectoryManagerExtensions.DeleteDirectory($"{repositoryPath}\\release-definition");
                }
            }

            //Git Provider - Clone 
            var providerCloneOptions = new CloneOptions();
            if(@options.GitProviderType == ConfigurationManagementService.GitHub)
                providerCloneOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.GitProviderAccessToken, Password = string.Empty };
            else if (@options.GitProviderType == ConfigurationManagementService.Bitbucket)
                providerCloneOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.GitProviderAccessId, Password = options.GitProviderAccessToken };
            else
                providerCloneOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.GitProviderAccessId, Password = @options.GitProviderAccessSecret };

            //Create local folder for the new repository
            string providerRepositoryPath = $"{Path.GetTempPath()}\\{Guid.NewGuid().ToString("n")}\\{@options.ProjectName}\\{@options.ServiceName}";
            Directory.CreateDirectory(providerRepositoryPath);
            var providerFolder = new DirectoryInfo(providerRepositoryPath);

            try
            {
                Repository.Clone(@options.GitProviderRepositoryUrl, providerRepositoryPath, providerCloneOptions);
            }
            catch (Exception ex)
            {

                throw;
            }

            //Git - Push
            using (var repo = new Repository(providerRepositoryPath))
            {
                string featureName = @options.FeatureName.ToLower();

                Branch branch = repo.CreateBranch(featureName);
                Branch currentBranch = Commands.Checkout(repo, branch);

                var remote = repo.Network.Remotes.FirstOrDefault(r => r.Name == @options.GitProviderAccessId);
                if (remote == null)
                {
                    repo.Network.Remotes.Add(@options.GitProviderAccessId, @options.GitProviderRepositoryUrl);
                    remote = repo.Network.Remotes.FirstOrDefault(r => r.Name == @options.GitProviderAccessId);
                }

                var pushOptions = new PushOptions();
                if (@options.GitProviderType == ConfigurationManagementService.GitHub)
                    pushOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.GitProviderAccessToken, Password = string.Empty };
                else if (@options.GitProviderType == ConfigurationManagementService.Bitbucket)
                    pushOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.GitProviderAccessId, Password = options.GitProviderAccessToken };
                else
                    pushOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.GitProviderAccessId, Password = @options.GitProviderAccessSecret };

                currentBranch = repo.Branches.Update(currentBranch, b => b.Remote = remote.Name, b => b.UpstreamBranch = currentBranch.CanonicalName);

                repo.Network.Push(currentBranch, pushOptions);
            }

            DirectoryManagerExtensions.DeleteDirectory(providerRepositoryPath);

            return releaseDefinition;
        }

        public async Task DeleteBranch(DeleteBranchOptions @options)
        {
            /*PUSH REPOSITORY*******************************************************************************************/

            //Git Provider - Clone 
            var providerCloneOptions = new CloneOptions();
            if (@options.GitProviderType == ConfigurationManagementService.GitHub)
                providerCloneOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.GitProviderAccessToken, Password = string.Empty };
            else
                providerCloneOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.GitProviderAccessId, Password = @options.GitProviderAccessSecret };

            //Create local folder for the new repository
            string providerRepositoryPath = $"{Path.GetTempPath()}\\{Guid.NewGuid().ToString("n")}\\{@options.ProjectName}\\{@options.ServiceName}";
            Directory.CreateDirectory(providerRepositoryPath);
            var providerFolder = new DirectoryInfo(providerRepositoryPath);

            Repository.Clone(@options.GitProviderRepositoryUrl, providerRepositoryPath, providerCloneOptions);

            //Git - Push
            using (var repo = new Repository(providerRepositoryPath))
            {
                string featureName = @options.FeatureName.ToLower();

                Branch branch = repo.Branches.FirstOrDefault(x => x.FriendlyName.Equals($"origin/{featureName}", StringComparison.InvariantCultureIgnoreCase));
                
                var remote = repo.Network.Remotes.FirstOrDefault(r => r.Name == @options.GitProviderAccessId);
                if (remote == null)
                {
                    repo.Network.Remotes.Add(@options.GitProviderAccessId, @options.GitProviderRepositoryUrl);
                    remote = repo.Network.Remotes.FirstOrDefault(r => r.Name == @options.GitProviderAccessId);
                }

                var pushOptions = new PushOptions();
                if (@options.GitProviderType == ConfigurationManagementService.GitHub)
                    pushOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.GitProviderAccessToken, Password = string.Empty };
                else
                    pushOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.GitProviderAccessId, Password = @options.GitProviderAccessSecret };

                repo.Branches.Remove(branch);
                repo.Network.Push(remote, $":refs/heads/{featureName}", pushOptions);
            }

            DirectoryManagerExtensions.DeleteDirectory(providerRepositoryPath);
        }

        public async Task<GetQueueResult> GetQueue(GetQueueOptions @options)
        {
            string accountUrl = $"https://{@options.VSTSAccountName}.visualstudio.com";

            HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
            authCredentials.Schema = "Basic";
            authCredentials.Value = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @options.VSTSAccessSecret)));

            string queueUrl = $"{accountUrl}/{@options.VSTSAccountProjectId}/_apis/distributedtask/queues";
            var queueResponse = await _httpClientWrapperService.GetAsync(queueUrl, authCredentials);
            queueResponse.EnsureSuccessStatusCode();

            var queues = await queueResponse.MapTo<CMSVSTSQueueListModel>();

            var defaultQueue = queues.Items.FirstOrDefault(x => x.Pool.Id == int.Parse(@options.AgentPoolId));
            if (defaultQueue == null)
                throw new Exception($"Agent Pool with id {@options.AgentPoolId} was not found");

            return new GetQueueResult()
            {
                QueueId = defaultQueue.Id,
                QueueName = defaultQueue.Name,
                PoolId = defaultQueue.Pool.Id,
                PoolName = defaultQueue.Pool.Name
            };
        }

        public async Task<int> CreateBuildDefinition(CreateBuildDefinitionOptions @options)
        {
            string accountUrl = $"https://{@options.VSTSAccountName}.visualstudio.com";

            HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
            authCredentials.Schema = "Basic";
            authCredentials.Value = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @options.VSTSAccessSecret)));

            CMSVSTSBuildDefinitionParamsModel @params = new CMSVSTSBuildDefinitionParamsModel();
            @params.Name = @options.CommitStageName;
            @params.Branch = @options.GitProviderRepositoryBranch;
            @params.DefaultBranch = @options.GitProviderRepositoryBranch;
            @params.ServiceExternalId = @options.GitProviderRepositoryId;
            @params.ServiceUrl = @options.GitProviderRepositoryUrl;
            @params.QueueId = @options.QueueId;
            @params.QueueName = @options.QueueName;
            @params.PoolId = @options.PoolId;
            @params.PoolName = @options.PoolName;

            string gitProviderRepositoryType = "TfsGit";

            if (@options.GitProviderType == ConfigurationManagementService.GitHub)
            {
                gitProviderRepositoryType = "github";

                @params.ServiceEndpointId = @options.ProjectExternalGitEndpoint;

                string endpointRepositoriesUrl = $"{accountUrl}/{@options.VSTSAccountProjectId}/_apis/sourceProviders/GitHub/repositories?api-version={@options.VSTSAPIVersion}-preview&serviceEndpointId={@params.ServiceEndpointId}&resultSet=0&pageResults=false";
                var endpointRepositoriesResponse = await _httpClientWrapperService.GetAsync(endpointRepositoriesUrl, authCredentials);
                endpointRepositoriesResponse.EnsureSuccessStatusCode();

                var endpointRepository = await endpointRepositoriesResponse.MapTo<CMSVSTSEndpointRepositoryModel>();
                var endpointRepositoryItem = endpointRepository.Repositories.FirstOrDefault(x => x.Id.Equals($"{@options.GitProviderAccessId}/{@options.CommitStageName}", StringComparison.InvariantCultureIgnoreCase));

                if (endpointRepositoryItem != null)
                {
                    @params.ApiUrl = endpointRepositoryItem.Properties.ApiUrl;
                    @params.BranchesUrl = endpointRepositoryItem.Properties.BranchesUrl;
                    @params.CloneUrl = endpointRepositoryItem.Properties.CloneUrl;
                    @params.ManageUrl = endpointRepositoryItem.Properties.ManageUrl;
                    @params.RefsUrl = endpointRepositoryItem.Properties.RefsUrl;
                    @params.IsFork = endpointRepositoryItem.Properties.IsFork;
                    @params.IsPrivate = endpointRepositoryItem.Properties.IsPrivate;
                    @params.LastUpdated = endpointRepositoryItem.Properties.LastUpdated;
                    @params.ManageUrl = endpointRepositoryItem.Properties.ManageUrl;
                    @params.OwnerAvatarUrl = endpointRepositoryItem.Properties.OwnerAvatarUrl;
                    @params.OwnerIsAUser = endpointRepositoryItem.Properties.OwnerIsAUser;
                    @params.RefsUrl = endpointRepositoryItem.Properties.RefsUrl;
                    @params.SafeOwnerId = endpointRepositoryItem.Properties.SafeOwnerId;
                    @params.SafeRepository = endpointRepositoryItem.Properties.SafeRepository;

                    @params.ServiceName = $"{@options.GitProviderAccessId}/{@options.ServiceName}";
                    @params.ServiceExternalId = $"{@options.GitProviderAccessId}/{@options.ServiceName}";
                }
            }

            if (@options.GitProviderType == ConfigurationManagementService.GitLab)
            {
                gitProviderRepositoryType = "Git";

                @params.ServiceEndpointId = @options.ProjectExternalGitEndpoint;

                string endpointRepositoriesUrl = $"{accountUrl}/{@options.VSTSAccountProjectId}/_apis/sourceProviders/git/repositories?api-version={@options.VSTSAPIVersion}-preview&serviceEndpointId={@params.ServiceEndpointId}&resultSet=0&pageResults=false";
                var endpointRepositoriesResponse = await _httpClientWrapperService.GetAsync(endpointRepositoriesUrl, authCredentials);
                endpointRepositoriesResponse.EnsureSuccessStatusCode();

                var endpointRepository = await endpointRepositoriesResponse.MapTo<CMSVSTSEndpointRepositoryModel>();
                var endpointRepositoryItem = endpointRepository.Repositories.FirstOrDefault(x => x.Id.Equals(options.ProjectExternalGitEndpoint, StringComparison.InvariantCultureIgnoreCase));

                if (endpointRepositoryItem != null)
                {
                    @params.ApiUrl = endpointRepositoryItem.Properties.ApiUrl;
                    @params.BranchesUrl = endpointRepositoryItem.Properties.BranchesUrl;
                    @params.CloneUrl = endpointRepositoryItem.Properties.CloneUrl;
                    @params.ManageUrl = endpointRepositoryItem.Properties.ManageUrl;
                    @params.RefsUrl = endpointRepositoryItem.Properties.RefsUrl;
                    @params.IsFork = endpointRepositoryItem.Properties.IsFork;
                    @params.IsPrivate = endpointRepositoryItem.Properties.IsPrivate;
                    @params.LastUpdated = endpointRepositoryItem.Properties.LastUpdated;
                    @params.ManageUrl = endpointRepositoryItem.Properties.ManageUrl;
                    @params.OwnerAvatarUrl = endpointRepositoryItem.Properties.OwnerAvatarUrl;
                    @params.OwnerIsAUser = endpointRepositoryItem.Properties.OwnerIsAUser;
                    @params.RefsUrl = endpointRepositoryItem.Properties.RefsUrl;
                    @params.SafeOwnerId = endpointRepositoryItem.Properties.SafeOwnerId;
                    @params.SafeRepository = endpointRepositoryItem.Properties.SafeRepository;

                    @params.ServiceName = endpointRepositoryItem.FullName;
                    @params.ServiceExternalId = $"{@options.ProjectExternalGitEndpoint}";
                }
            }

            if (@options.GitProviderType == ConfigurationManagementService.Bitbucket)
            {
                gitProviderRepositoryType = "Bitbucket";

                @params.ServiceEndpointId = @options.ProjectExternalGitEndpoint;

                string endpointRepositoriesUrl = $"{accountUrl}/{@options.VSTSAccountProjectId}/_apis/sourceProviders/Bitbucket/repositories?api-version={@options.VSTSAPIVersion}-preview&serviceEndpointId={@params.ServiceEndpointId}&resultSet=0&pageResults=false";
                var endpointRepositoriesResponse = await _httpClientWrapperService.GetAsync(endpointRepositoriesUrl, authCredentials);
                endpointRepositoriesResponse.EnsureSuccessStatusCode();

                var endpointRepository = await endpointRepositoriesResponse.MapTo<CMSVSTSEndpointRepositoryModel>();
                var endpointRepositoryItem = endpointRepository.Repositories.FirstOrDefault(x => x.Name.Equals($"{@options.CommitStageName}", StringComparison.InvariantCultureIgnoreCase));

                if (endpointRepositoryItem != null)
                {
                    @params.ApiUrl = endpointRepositoryItem.Properties.ApiUrl;
                    @params.BranchesUrl = endpointRepositoryItem.Properties.BranchesUrl;
                    @params.CloneUrl = endpointRepositoryItem.Properties.CloneUrl;
                    @params.ManageUrl = endpointRepositoryItem.Properties.ManageUrl;
                    @params.RefsUrl = endpointRepositoryItem.Properties.RefsUrl;
                    @params.IsFork = endpointRepositoryItem.Properties.IsFork;
                    @params.IsPrivate = endpointRepositoryItem.Properties.IsPrivate;
                    @params.LastUpdated = endpointRepositoryItem.Properties.LastUpdated;
                    @params.ManageUrl = endpointRepositoryItem.Properties.ManageUrl;
                    @params.OwnerAvatarUrl = endpointRepositoryItem.Properties.OwnerAvatarUrl;
                    @params.OwnerIsAUser = endpointRepositoryItem.Properties.OwnerIsAUser;
                    @params.RefsUrl = endpointRepositoryItem.Properties.RefsUrl;
                    @params.SafeOwnerId = endpointRepositoryItem.Properties.SafeOwnerId;
                    @params.SafeRepository = endpointRepositoryItem.Properties.SafeRepository;

                    @params.ServiceName = endpointRepositoryItem.FullName;
                    @params.ServiceExternalId = endpointRepositoryItem.FullName;
                }

            }

            @params.RepositoryType = gitProviderRepositoryType;
            @params.YamlFilename = options.YamlFilename;

            var model = CMSVSTSServiceBuildDefinitionCreateModel.Factory.Create(@params);

            string buildDefinitionUrl = $"{accountUrl}/{@options.VSTSAccountProjectId}/_apis/build/definitions?api-version={@options.VSTSAPIVersion}";
            var buildDefinitionResponse = await _httpClientWrapperService.PostAsync(buildDefinitionUrl, model, authCredentials);
            buildDefinitionResponse.EnsureSuccessStatusCode();

            var buildDefinition = await buildDefinitionResponse.MapTo<CMSVSTSServiceBuildDefinitionModel>();
            return buildDefinition.Id;
        }

        public async Task DeleteBuildDefinition(DeleteBuildDefinitionOptions @options)
        {
            string accountUrl = $"https://{@options.VSTSAccountName}.visualstudio.com";

            /*CREATE DEFINITION*******************************************************************************************/
            HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
            authCredentials.Schema = "Basic";
            authCredentials.Value = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @options.VSTSAccessSecret)));

            string buildDefinitionUrl = $"{accountUrl}/{@options.VSTSAccountProjectId}/_apis/build/definitions/{@options.CommitStageId}?api-version={@options.VSTSAPIVersion}";
            var buildDefinitionResponse = await _httpClientWrapperService.DeleteAsync(buildDefinitionUrl, authCredentials);
            buildDefinitionResponse.EnsureSuccessStatusCode();
        }

        public async Task<Guid> CreateServiceHook(CreateServiceHookOptions @options)
        {
            string accountUrl = "";

            HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
            authCredentials.Schema = "Basic";
            authCredentials.Value = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @options.VSTSAccessSecret)));

            CMSVSTSServiceHookParamsModel @params = new CMSVSTSServiceHookParamsModel();
            @params.ProjectId = @options.ProjectExternalId;
            @params.Definition = @options.Definition;
            @params.Url = @options.Url;

            @params.Repository = @options.Repository;
            @params.Branch = @options.Branch;

            CMSVSTSServiceHookCreateBaseModel model = null;
            if (@options.EventType == "code")
            {
                accountUrl = $"https://{@options.VSTSAccountName}.visualstudio.com";
                model = new CMSVSTSServiceHookCodeCreateModel().Build(@params);
            }
            if (@options.EventType == "build")
            {
                accountUrl = $"https://{@options.VSTSAccountName}.visualstudio.com";
                model = new CMSVSTSServiceHookBuildCreateModel().Build(@params);
            }
            if (@options.EventType == "releaseStarted")
            {
                accountUrl = $"https://{@options.VSTSAccountName}.vsrm.visualstudio.com";
                model = new CMSVSTSServiceHookReleaseStartedCreateModel().Build(@params);
            }
            if (@options.EventType == "release")
            {
                accountUrl = $"https://{@options.VSTSAccountName}.vsrm.visualstudio.com";
                model = new CMSVSTSServiceHookReleaseCreateModel().Build(@params);
            }
            if (@options.EventType == "releasePendingApproval")
            {
                accountUrl = $"https://{@options.VSTSAccountName}.vsrm.visualstudio.com";
                model = new CMSVSTSServiceHookReleasePendingApprovalCreateModel().Build(@params);
            }
            if (@options.EventType == "releaseCompletedApproval")
            {
                accountUrl = $"https://{@options.VSTSAccountName}.vsrm.visualstudio.com";
                model = new CMSVSTSServiceHookReleaseCompletedApprovalCreateModel().Build(@params);
            }

            string serviceHookUrl = $"{accountUrl}/_apis/hooks/subscriptions?api-version={@options.VSTSAPIVersion}";
            var serviceHookResponse = await _httpClientWrapperService.PostAsync(serviceHookUrl, model, authCredentials);

#if !DEBUG
            serviceHookResponse.EnsureSuccessStatusCode();
#endif
            var serviceHook = await serviceHookResponse.MapTo<CMSVSTSServiceHookModel>();
            return serviceHook.Id;
        }

        public async Task DeleteServiceHook(DeleteServiceHookOptions @options)
        {
            string accountUrl = "";

            HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
            authCredentials.Schema = "Basic";
            authCredentials.Value = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @options.VSTSAccessSecret)));

            CMSVSTSServiceHookCreateBaseModel model = null;
            if (@options.EventType == "build")
            {
                accountUrl = $"https://{@options.VSTSAccountName}.visualstudio.com";
            }
            else
            {
                accountUrl = $"https://{@options.VSTSAccountName}.vsrm.visualstudio.com";
            }

            string serviceHookUrl = $"{accountUrl}/_apis/hooks/subscriptions/{@options.ServiceHookId}?api-version={@options.VSTSAPIVersion}";
            var serviceHookResponse = await _httpClientWrapperService.DeleteAsync(serviceHookUrl, model, authCredentials);

#if !DEBUG
            serviceHookResponse.EnsureSuccessStatusCode();
#endif

        }

        public async Task<CMSVSTSReleaseDefinitionReadModel> GetReleaseDefinition(ReadReleaseDefinitionOptions @options)
        {
            string accountUrl = $"https://{@options.VSTSAccountName}.visualstudio.com";
            string releseUrl = $"https://{@options.VSTSAccountName}.vsrm.visualstudio.com";

            HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
            authCredentials.Schema = "Basic";
            authCredentials.Value = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @options.VSTSAccessSecret)));

            string releseDefinitionUrl = $"{releseUrl}/{@options.VSTSAccountProjectId}/_apis/release/definitions/{@options.ReleaseStageId}?api-version={@options.VSTSAPIVersion}-preview";
            var releseDefinitionResponse = await _httpClientWrapperService.GetAsync(releseDefinitionUrl, authCredentials);
            releseDefinitionResponse.EnsureSuccessStatusCode();

            var releaseDefinition = await releseDefinitionResponse.MapTo<CMSVSTSReleaseDefinitionReadModel>();

            return releaseDefinition;
        }

        public async Task<int?> CreateReleaseDefinition(CreateReleaseDefinitionOptions @options)
        {
            if (!string.IsNullOrEmpty(@options.ReleaseDefinition) && !string.IsNullOrEmpty(@options.CloudProviderEndpointId))
            {
                string accountUrl = $"https://{@options.VSTSAccountName}.visualstudio.com";
                string releseUrl = $"https://{@options.VSTSAccountName}.vsrm.visualstudio.com";
                
                HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
                authCredentials.Schema = "Basic";
                authCredentials.Value = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @options.VSTSAccessSecret)));

                //Replace Values
                @options.ReleaseDefinition = @options.ReleaseDefinition.Replace("PS_COMMIT_STAGE_NAME", @options.BuildDefinitionName);
                @options.ReleaseDefinition = @options.ReleaseDefinition.Replace("PS_RELEASE_STAGE_NAME", @options.ReleaseStageName);
                @options.ReleaseDefinition = @options.ReleaseDefinition.Replace("PS_ORGANIZATION_NAME", @options.OrganizationName.ToLower());
                @options.ReleaseDefinition = @options.ReleaseDefinition.Replace("PS_PROJECT_NAME", @options.ProjectName.ToLower());
                @options.ReleaseDefinition = @options.ReleaseDefinition.Replace("PS_SERVICE_NAME", @options.ServiceName.ToLower());
                @options.ReleaseDefinition = @options.ReleaseDefinition.Replace("PS_CLOUD_PROVIDER_ENDPOINT_ID", @options.CloudProviderEndpointId);
                @options.ReleaseDefinition = @options.ReleaseDefinition.Replace("PS_ACCESS_KEY", @options.CloudProviderAccessId);
                @options.ReleaseDefinition = @options.ReleaseDefinition.Replace("PS_ACCESS_SECRET", @options.CloudProviderAccessSecret);
                @options.ReleaseDefinition = @options.ReleaseDefinition.Replace("PS_ACCESS_REGION", @options.CloudProviderAccessRegion);
                //@options.ReleaseDefinition = @options.ReleaseDefinition.Replace("PS_NEW_PASSWORD", _passwordGeneratorService.GenerateRandomPassword());

                CMSVSTSReleaseDefinitionInputModel releaseDefinitionInput = JsonConvert.DeserializeObject<CMSVSTSReleaseDefinitionInputModel>(@options.ReleaseDefinition);

                //For Development
                dynamic environmentDevelopmentVariables = new ExpandoObject();
                var dictionaryDevelopment = (IDictionary<string, object>)environmentDevelopmentVariables;
                dictionaryDevelopment.Add("ASPNETCORE_ENVIRONMENT", CMSVSTSReleaseDefinitionEnviromentVariableModel.Factory.Create(DomainConstants.Environments.Development, DomainConstants.Environments.Development.ToLower()));
                dictionaryDevelopment.Add("ASPNETCORE_FEATURE", CMSVSTSReleaseDefinitionEnviromentVariableModel.Factory.Create(DomainConstants.Environments.Development, @options.WorkFeature.ToLower()));
                dictionaryDevelopment.Add("PS_ENVIRONMENT_ENABLE", CMSVSTSReleaseDefinitionEnviromentVariableModel.Factory.Create(DomainConstants.Environments.Development, "True"));
                //dictionaryDevelopment.Add("PS_ENVIRONMENT_SKIPEXPRESSION", CMSVSTSReleaseDefinitionEnviromentVariableModel.Factory.Create(DomainConstants.Environments.Development, "PS_SKIP_ENVIRONMENT_$(Release.EnvironmentName)"));

                foreach (var item in @options.TemplateParameters)
                {
                    dictionaryDevelopment.Add(item.VariableName, CMSVSTSReleaseDefinitionEnviromentVariableModel.Factory.Create(DomainConstants.Environments.Development, item.Value));
                }

                //For Production
                dynamic environmentProductionVariables = new ExpandoObject();
                var dictionaryProduction = (IDictionary<string, object>)environmentProductionVariables;
                dictionaryProduction.Add("ASPNETCORE_ENVIRONMENT", CMSVSTSReleaseDefinitionEnviromentVariableModel.Factory.Create(DomainConstants.Environments.Production, DomainConstants.Environments.Production.ToLower()));
                dictionaryProduction.Add("ASPNETCORE_FEATURE", CMSVSTSReleaseDefinitionEnviromentVariableModel.Factory.Create(DomainConstants.Environments.Production, @options.WorkFeature.ToLower()));
                dictionaryProduction.Add("PS_ENVIRONMENT_ENABLE", CMSVSTSReleaseDefinitionEnviromentVariableModel.Factory.Create(DomainConstants.Environments.Production, "True"));
                //dictionaryProduction.Add("PS_ENVIRONMENT_SKIPEXPRESSION", CMSVSTSReleaseDefinitionEnviromentVariableModel.Factory.Create(DomainConstants.Environments.Production, "PS_SKIP_ENVIRONMENT_$(Release.EnvironmentName)"));

                foreach (var item in @options.TemplateParameters)
                {
                    dictionaryProduction.Add(item.VariableName, CMSVSTSReleaseDefinitionEnviromentVariableModel.Factory.Create(DomainConstants.Environments.Production, item.Value));
                }

                string buildDefinitionUrl = $"{accountUrl}/{@options.VSTSAccountProjectId}/_apis/build/definitions/{@options.CommitStageId.ToString()}?api-version={@options.VSTSAPIVersion}-preview";
                var buildDefinitionResponse = await _httpClientWrapperService.GetAsync(buildDefinitionUrl, authCredentials);

                var buildDefinition = await buildDefinitionResponse.MapTo<CMSVSTSBuildDefinitionReadModel>();

                CMSVSTSeReleaseDefinitionParamsModel @params = new CMSVSTSeReleaseDefinitionParamsModel
                {
                    CommitStageId = @options.CommitStageId.ToString(),
                    CommitStageName = @options.CommitStageName,
                    ReleaseStageName = @options.ReleaseStageName,
                    ProjectExternalId = @options.ProjectExternalId,
                    ProjectName = @options.ProjectName,
                    ServiceName = @options.ServiceName,
                    EnvironmentDevelopmentName = DomainConstants.Environments.Development,
                    EnvironmentProductionName = DomainConstants.Environments.Production,
                    QueueId = @options.QueueId,
                    ReleaseDefinitionInput = releaseDefinitionInput,
                    EnvironmentDevelopmentVariables = environmentDevelopmentVariables,
                    EnvironmentProductionVariables = environmentProductionVariables,
                    Owner = buildDefinition.AuthoredBy
                };

                var model = CMSVSTSReleaseDefinitionCreateModel.Factory.Create(@params);

                string releseDefinitionUrl = $"{releseUrl}/{@options.VSTSAccountProjectId}/_apis/release/definitions?api-version={@options.VSTSAPIVersion}-preview";
                var releseDefinitionResponse = await _httpRetryPolicy.ExecuteAsync(() => _httpClientWrapperService.PostAsync(releseDefinitionUrl, model, authCredentials)); 

                if (!releseDefinitionResponse.IsSuccessStatusCode)
                {
                    StringBuilder messageBuilder = new StringBuilder();
                    messageBuilder.AppendLine($"Code: {releseDefinitionResponse.StatusCode} ##");
                    messageBuilder.AppendLine($"Url: {releseDefinitionUrl} ##");
                    messageBuilder.AppendLine($"Auth: {authCredentials.Value} ##");
                    messageBuilder.AppendLine($"Body: {JsonConvert.SerializeObject(model)} ##");

                    TelemetryClientManager.Instance.TrackTrace(messageBuilder.ToString());
                }
                
                releseDefinitionResponse.EnsureSuccessStatusCode();

                var releaseDefinition = await releseDefinitionResponse.MapTo<CMSVSTSServiceReleaseDefinitionModel>();
                return releaseDefinition.Id;
            }

            return null;
        }

        public async Task<int?> CreateReleaseDefinitionFromBaseDefinition(CreateReleaseDefinitionOptions @options)
        {
            ReadReleaseDefinitionOptions readReleaseDefinitionOptions = new ReadReleaseDefinitionOptions();
            readReleaseDefinitionOptions.VSTSAPIVersion = @options.VSTSAPIVersion;
            readReleaseDefinitionOptions.VSTSAccountName = @options.VSTSAccountName;
            readReleaseDefinitionOptions.VSTSAccessSecret = @options.VSTSAccessSecret;
            readReleaseDefinitionOptions.VSTSAccountProjectId = @options.VSTSAccountProjectId;

            readReleaseDefinitionOptions.OrganizationName = @options.OrganizationName;
            readReleaseDefinitionOptions.ProjectName = @options.ProjectName;
            readReleaseDefinitionOptions.ReleaseStageId = @options.BaseReleaseStageId.Value;

            var releaseDefinitionBase = await GetReleaseDefinition(readReleaseDefinitionOptions);
            
            if (!string.IsNullOrEmpty(@options.ReleaseDefinition) && !string.IsNullOrEmpty(@options.CloudProviderEndpointId))
            {
                string accountUrl = $"https://{@options.VSTSAccountName}.visualstudio.com";
                string releseUrl = $"https://{@options.VSTSAccountName}.vsrm.visualstudio.com";

                HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
                authCredentials.Schema = "Basic";
                authCredentials.Value = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @options.VSTSAccessSecret)));

                //Replace Values
                @options.ReleaseDefinition = @options.ReleaseDefinition.Replace("PS_COMMIT_STAGE_NAME", @options.BuildDefinitionName);
                @options.ReleaseDefinition = @options.ReleaseDefinition.Replace("PS_RELEASE_STAGE_NAME", @options.ReleaseStageName);
                @options.ReleaseDefinition = @options.ReleaseDefinition.Replace("PS_ORGANIZATION_NAME", @options.OrganizationName.ToLower());
                @options.ReleaseDefinition = @options.ReleaseDefinition.Replace("PS_PROJECT_NAME", @options.ProjectName.ToLower());
                @options.ReleaseDefinition = @options.ReleaseDefinition.Replace("PS_SERVICE_NAME", @options.ServiceName.ToLower());
                @options.ReleaseDefinition = @options.ReleaseDefinition.Replace("PS_CLOUD_PROVIDER_ENDPOINT_ID", @options.CloudProviderEndpointId);
                @options.ReleaseDefinition = @options.ReleaseDefinition.Replace("PS_ACCESS_KEY", @options.CloudProviderAccessId);
                @options.ReleaseDefinition = @options.ReleaseDefinition.Replace("PS_ACCESS_SECRET", @options.CloudProviderAccessSecret);
                @options.ReleaseDefinition = @options.ReleaseDefinition.Replace("PS_ACCESS_REGION", @options.CloudProviderAccessRegion);
                //@options.ReleaseDefinition = @options.ReleaseDefinition.Replace("PS_NEW_PASSWORD", _passwordGeneratorService.GenerateRandomPassword());

                dynamic environmentVariables = new ExpandoObject();
                var dictionary = (IDictionary<string, object>)environmentVariables;
                dictionary.Add("ASPNETCORE_ENVIRONMENT", CMSVSTSReleaseDefinitionEnviromentVariableModel.Factory.Create(DomainConstants.Environments.Development, DomainConstants.Environments.Development.ToLower()));
                dictionary.Add("ASPNETCORE_FEATURE", CMSVSTSReleaseDefinitionEnviromentVariableModel.Factory.Create(DomainConstants.Environments.Development, @options.WorkFeature.ToLower()));

                CMSVSTSReleaseDefinitionInputModel releaseDefinitionInput = new CMSVSTSReleaseDefinitionInputModel();
                var baseEnvironment = releaseDefinitionBase.Environments.FirstOrDefault(x => x.Name.Equals(DomainConstants.Environments.Development, StringComparison.InvariantCultureIgnoreCase));
                if (baseEnvironment != null)
                {
                    releaseDefinitionInput.Tasks = baseEnvironment.DeployPhases[0].WorkflowTasks.Select(x => new CMSVSTSReleaseDefinitionTemplateTasksCreateModel()
                    {
                        Inputs = x.Inputs,
                        Name = x.Name,
                        TaskId = x.TaskId,
                        Version = x.Version
                    }).ToList();

                    /*Release Variables*/
                    var baseReleaseVariables = ((JObject)releaseDefinitionBase.Variables).DeepClone();
                    var propertyBuildDefinition = ((JObject)baseReleaseVariables).GetValue("BUILD_DEFINITION");
                    if (propertyBuildDefinition != null)
                    {
                        propertyBuildDefinition["value"] = @options.CommitStageName;
                    }
                    releaseDefinitionInput.Variables = baseReleaseVariables;
                    
                    /*Environment Variables*/
                    var baseEnvironmentVariables = ((JObject)baseEnvironment.Variables).DeepClone();
                    var propertyEnvironment = ((JObject)baseEnvironmentVariables).GetValue("ASPNETCORE_ENVIRONMENT");
                    if (propertyEnvironment != null)
                    {
                        propertyEnvironment["value"] = DomainConstants.Environments.Development.ToLower();
                    }
                    var propertyFeature = ((JObject)baseEnvironmentVariables).GetValue("ASPNETCORE_FEATURE");
                    if (propertyFeature != null)
                    {
                        propertyFeature["value"] = @options.WorkFeature.ToLower();
                    }
                    var propertyEnvironmentEnabled = ((JObject)baseEnvironmentVariables).GetValue("PS_ENVIRONMENT_ENABLE");
                    if (propertyEnvironmentEnabled != null)
                    {
                        propertyEnvironmentEnabled["value"] = "True";
                    }
                    environmentVariables = baseEnvironmentVariables;
                }
                else
                {
                    releaseDefinitionInput = JsonConvert.DeserializeObject<CMSVSTSReleaseDefinitionInputModel>(@options.ReleaseDefinition);
                }

                CMSVSTSeReleaseDefinitionParamsModel @params = new CMSVSTSeReleaseDefinitionParamsModel
                {
                    CommitStageId = @options.CommitStageId.ToString(),
                    CommitStageName = @options.CommitStageName,
                    ReleaseStageName = @options.ReleaseStageName,
                    ProjectExternalId = @options.ProjectExternalId,
                    ProjectName = @options.ProjectName,
                    ServiceName = @options.ServiceName,
                    EnvironmentDevelopmentName = DomainConstants.Environments.Development,
                    QueueId = @options.QueueId,
                    ReleaseDefinitionInput = releaseDefinitionInput,
                    EnvironmentDevelopmentVariables = environmentVariables
                };

                var model = CMSVSTSReleaseDefinitionCreateModel.Factory.CreateForFeature(@params);

                string releseDefinitionUrl = $"{releseUrl}/{@options.VSTSAccountProjectId}/_apis/release/definitions?api-version={@options.VSTSAPIVersion}-preview";
                var releseDefinitionResponse = await _httpClientWrapperService.PostAsync(releseDefinitionUrl, model, authCredentials);
                releseDefinitionResponse.EnsureSuccessStatusCode();

                var releaseDefinition = await releseDefinitionResponse.MapTo<CMSVSTSServiceReleaseDefinitionModel>();
                return releaseDefinition.Id;
            }

            return null;
        }

        public async Task DeleteReleaseDefinition(DeleteReleaseDefinitionOptions @options)
        {
            string releseUrl = $"https://{@options.VSTSAccountName}.vsrm.visualstudio.com";

            HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
            authCredentials.Schema = "Basic";
            authCredentials.Value = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @options.VSTSAccessSecret)));

            string releseDefinitionUrl = $"{releseUrl}/{@options.VSTSAccountProjectId}/_apis/release/definitions/{@options.ReleaseStageId}?forceDelete=true&api-version={@options.VSTSAPIVersion}-preview";
            var releseDefinitionResponse = await _httpClientWrapperService.DeleteAsync(releseDefinitionUrl, authCredentials);
            releseDefinitionResponse.EnsureSuccessStatusCode();
        }

        public async Task UpdateReleaseDefinition(UpdateReleaseDefinitionOptions @options)
        {
            string releseUrl = $"https://{@options.VSTSAccountName}.vsrm.visualstudio.com";

            HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
            authCredentials.Schema = "Basic";
            authCredentials.Value = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @options.VSTSAccessSecret)));

            string releseDefinitionUrl = $"{releseUrl}/{@options.VSTSAccountProjectId}/_apis/release/definitions?api-version={@options.VSTSAPIVersion}-preview";
            var releseDefinitionResponse = await _httpClientWrapperService.PutAsync(releseDefinitionUrl, @options.Model, authCredentials);
            releseDefinitionResponse.EnsureSuccessStatusCode();
        }

        public async Task QueueBuild(QueueBuildOptions @options)
        {
            string accountUrl = $"https://{@options.VSTSAccountName}.visualstudio.com";

            HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
            authCredentials.Schema = "Basic";
            authCredentials.Value = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @options.VSTSAccessSecret)));

            string queueBuildUrl = $"{accountUrl}/{@options.VSTSAccountProjectId}/_apis/build/builds?api-version={@options.VSTSAPIVersion}";

            var queueBuild = CMSVSTSBuildModel.Create(@options.QueueId, @options.BuildDefinitionId, @options.ProjectExternalId, @options.SourceBranch);
            var queueBuildResponse = await _httpClientWrapperService.PostAsync(queueBuildUrl, queueBuild, authCredentials);
            queueBuildResponse.EnsureSuccessStatusCode();
        }

        public async Task QueueRelease(QueueReleaseOptions @options)
        {
            string accountUrl = $"https://{@options.VSTSAccountName}.vsrm.visualstudio.com";

            HttpClientWrapperAuthorizationModel authCredentials = new HttpClientWrapperAuthorizationModel();
            authCredentials.Schema = "Basic";
            authCredentials.Value = Convert.ToBase64String(ASCIIEncoding.ASCII.GetBytes(string.Format("{0}:{1}", "", @options.VSTSAccessSecret)));

            string queueReleaseUrl = $"{accountUrl}/{@options.VSTSAccountProjectId}/_apis/release/releases?api-version={@options.VSTSAPIVersion}";

            var queueRelease = CMSVSTSReleaseModel.Create(@options.ReleaseDefinitionId, @options.Alias, @options.VersionId, @options.VersionName, @options.Description);
            var queueReleaseResponse = await _httpClientWrapperService.PostAsync(queueReleaseUrl, queueRelease, authCredentials);
            queueReleaseResponse.EnsureSuccessStatusCode();
        }

        public async Task<string> GetReleaseDefinition(CreateRepositoryOptions options, string buildDefinition)
        {
            /*PUSH REPOSITORY*******************************************************************************************/

            //Create local folder for the new repository
            string repositoryPath = $"{Path.GetTempPath()}\\{Guid.NewGuid().ToString("n")}\\{@options.ProjectName}\\{@options.ServiceName}";

            Directory.CreateDirectory(repositoryPath);
            var folder = new DirectoryInfo(repositoryPath);

            //Git - Clone 
            var cloneOptions = new CloneOptions() {
                CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.VSTSAccessId, Password = @options.VSTSAccessSecret }
            };

            Repository.Clone(@options.VSTSRepositoryTemplateUrl, repositoryPath, cloneOptions);
            
            //Retreiving release definition file
            string releaseDefinition = string.Empty;
            if (File.Exists($"{repositoryPath}\\{options.VSTSRepositoryTemplatePath}\\release\\definition.json"))
            {
                releaseDefinition = File.ReadAllText($"{repositoryPath}\\{options.VSTSRepositoryTemplatePath}\\release\\definition.json");
            }

            string repositoryImportPath = $"{Path.GetTempPath()}\\{Guid.NewGuid().ToString("n")}\\{@options.ProjectName}\\{@options.ServiceName}";

            Directory.CreateDirectory(repositoryImportPath);
            var folderImport = new DirectoryInfo(repositoryImportPath);

            var importCloneOptions = new CloneOptions();
            var repositoryUrl = @options.GitProviderRepositoryUrl;
            if (@options.GitProviderType == ConfigurationManagementService.VSTS)
            {
                importCloneOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.GitProviderAccessId, Password = @options.GitProviderAccessSecret };
            }

            if (@options.GitProviderType == ConfigurationManagementService.GitHub)
            {
                importCloneOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.GitProviderAccessToken };
            }

            //importCloneOptions.BranchName = options.Branch;

            Repository.Clone(repositoryUrl, repositoryImportPath, importCloneOptions);

            var directoryPipelineSpace = Path.Combine(repositoryImportPath, ".pipelinespace");

            if (!Directory.Exists(directoryPipelineSpace))
            {
                Directory.CreateDirectory(directoryPipelineSpace);
            }
            File.WriteAllText(Path.Combine(directoryPipelineSpace, "build.definition.yml"), buildDefinition);

            //Git - Stage 
            using (var repo = new Repository(folderImport.FullName))
            {
                Commands.Stage(repo, "*");
            }

            //Git - Commit
            using (var repo = new Repository(folderImport.FullName))
            {
                // Create the committer's signature and commit
                Signature author = new Signature(@options.GitProviderAccessId, @options.GitProviderAccessId, DateTime.Now);
                Signature committer = author;

                // Commit to the repository
                Commit commit = repo.Commit("Initial PipelineSpace", author, committer);
            }
            
            //Git - Push
            using (var repo = new Repository(folderImport.FullName))
            {
                string featureName = @options.Branch.ToLower();

                Branch branch = repo.CreateBranch(featureName);
                Branch currentBranch = Commands.Checkout(repo, branch);

                var remote = repo.Network.Remotes.FirstOrDefault(r => r.Name == @options.GitProviderAccessId);
                if (remote == null)
                {
                    repo.Network.Remotes.Add(@options.GitProviderAccessId, @options.GitProviderRepositoryUrl);
                    remote = repo.Network.Remotes.FirstOrDefault(r => r.Name == @options.GitProviderAccessId);
                }

                var pushOptions = new PushOptions();
                if (@options.GitProviderType == ConfigurationManagementService.GitHub)
                    pushOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.GitProviderAccessToken, Password = string.Empty };
                else
                    pushOptions.CredentialsProvider = (_url, _user, _cred) => new UsernamePasswordCredentials { Username = @options.GitProviderAccessId, Password = @options.GitProviderAccessSecret };

                currentBranch = repo.Branches.Update(currentBranch, b => b.Remote = remote.Name, b => b.UpstreamBranch = currentBranch.CanonicalName);

                repo.Network.Push(currentBranch, pushOptions);
            }

            //delete local folder for the repository
            DirectoryManagerExtensions.DeleteDirectory(repositoryImportPath);
            DirectoryManagerExtensions.DeleteDirectory(repositoryPath);

            return releaseDefinition;
        }
    }
}
