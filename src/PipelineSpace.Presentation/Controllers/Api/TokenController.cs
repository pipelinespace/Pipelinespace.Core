using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PipelineSpace.Application.Models;
using PipelineSpace.Domain.Core.Manager;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PipelineSpace.Presentation.Controllers.Api
{
    [Route("{api}/tokens")]
    public class TokenController : BaseController
    {
        private readonly IConfiguration _configuration;
        public TokenController(IDomainManagerService domainManagerService,
                               IConfiguration configuration) : base(domainManagerService)
        {
            _configuration = configuration;
        }

        [HttpPost]
        [AllowAnonymous]
        [Route("refresh")]
        public async Task<IActionResult> Refresh([FromBody]TokenRefreshPostRp resource)
        {
            if (!ModelState.IsValid)
                return this.BadRequest(ModelState);

            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
            keyValues.Add(new KeyValuePair<string, string>("client_id", _configuration["AdminClient:ClientId"]));
            keyValues.Add(new KeyValuePair<string, string>("client_secret", _configuration["AdminClient:ClientSecret"]));
            keyValues.Add(new KeyValuePair<string, string>("refresh_token", resource.RefreshToken));
            keyValues.Add(new KeyValuePair<string, string>("scope", _configuration["AdminClient:Scope"]));

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync($"{_configuration["AdminClient:Authority"]}/connect/token", new FormUrlEncodedContent(keyValues));

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    return this.Ok(Newtonsoft.Json.JsonConvert.DeserializeObject<dynamic>(jsonData));
                }

                return this.Conflict(new { Reason = "InvalidRefreshAttempt" });
            }
        }
    }
}
