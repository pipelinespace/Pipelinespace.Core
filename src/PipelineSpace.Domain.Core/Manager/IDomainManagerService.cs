using System.Threading.Tasks;

namespace PipelineSpace.Domain.Core.Manager
{
    public interface IDomainManagerService
    {
        Task AddConflict(string message);
        Task AddNotFound(string message);
        Task AddBadRequest(string message);
        Task AddResult(string key, object value);
        Task AddForbidden(string message);
        string GetConflicts();
        string GetNotFounds();
        string GetBadRequests();
        string GetForbidden();
        Task<T> GetResult<T>(string key);
        bool HasConflicts();
        bool HasNotFounds();
        bool HasBadRequests();
        bool HasForbidden();
    }
}
