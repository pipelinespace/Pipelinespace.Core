using Microsoft.Extensions.Configuration;
using PipelineSpace.Infra.CrossCutting.Identity.TokenProviderServices.Interfaces;
using PipelineSpace.Infra.CrossCutting.Identity.TokenProviderServices.Models;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace PipelineSpace.Infra.CrossCutting.Identity
{
    public class VSTSTokenProviderService : ITokenProviderService
    {
        readonly IConfiguration _configuration;
        public VSTSTokenProviderService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<OAuthToken> RefreshToken(string refreshToken)
        {
            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("client_assertion_type", "urn:ietf:params:oauth:client-assertion-type:jwt-bearer"));
            keyValues.Add(new KeyValuePair<string, string>("client_assertion", HttpUtility.UrlEncode(_configuration["Authentication:VisualStudio:ClientSecret"])));
            keyValues.Add(new KeyValuePair<string, string>("grant_type", "refresh_token"));
            keyValues.Add(new KeyValuePair<string, string>("assertion", HttpUtility.UrlEncode(refreshToken)));

            string applicationUrl = _configuration["Application:Url"];
            if (applicationUrl.Contains("localhost"))
            {
                applicationUrl = "https://pswebsitestaging.azurewebsites.net";
            }

            keyValues.Add(new KeyValuePair<string, string>("redirect_uri", applicationUrl + "/signin-vsts"));

            using (var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync("https://app.vssps.visualstudio.com/oauth2/token", new FormUrlEncodedContent(keyValues));

                if (response.IsSuccessStatusCode)
                {
                    var jsonData = await response.Content.ReadAsStringAsync();
                    return Newtonsoft.Json.JsonConvert.DeserializeObject<OAuthToken>(jsonData);
                }

                return await Task.FromResult<OAuthToken>(null);
            }
        }
    }
}
