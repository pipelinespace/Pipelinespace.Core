using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PipelineSpace.Domain.Core.Manager;

namespace PipelineSpace.Presentation.Controllers
{
    public abstract class BaseController : Controller
    {
        readonly IDomainManagerService _domainManagerService;
        public BaseController(IDomainManagerService domainManagerService)
        {
            _domainManagerService = domainManagerService;
        }

        public IDomainManagerService DomainManager
        {
            get
            {
                return _domainManagerService;
            }
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Conflict(object value)
        {
            return this.StatusCode(StatusCodes.Status409Conflict, value);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Forbidden(object value)
        {
            return this.StatusCode(StatusCodes.Status403Forbidden, value);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult Error(object value)
        {
            return this.StatusCode(StatusCodes.Status500InternalServerError, value);
        }

        [ApiExplorerSettings(IgnoreApi = true)]
        public IActionResult ServiceUnavailable(object value)
        {
            return this.StatusCode(StatusCodes.Status503ServiceUnavailable, value);
        }
    }
}
