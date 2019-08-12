using PipelineSpace.Worker.Handlers.Models;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Services.Interfaces
{
    public interface IHttpClientWrapperService
    {
        Task<HttpResponseMessage> DeleteAsync(string resource, HttpClientWrapperAuthorizationModel authCredentials, Dictionary<string, string> headers = null);
        Task<HttpResponseMessage> DeleteAsync<T>(string resource, T model, HttpClientWrapperAuthorizationModel authCredentials, Dictionary<string, string> headers = null);
        Task<HttpResponseMessage> GetAsync(string resource, HttpClientWrapperAuthorizationModel authCredentials, Dictionary<string, string> headers = null, bool cache = false, double cacheExpiration = 60);
        Task<HttpResponseMessage> PatchAsync<T>(string resource, T model, HttpClientWrapperAuthorizationModel authCredentials, Dictionary<string, string> headers = null);
        Task<HttpResponseMessage> UploadFileAsync<T>(string resource, T model, HttpClientWrapperAuthorizationModel authCredentials, Dictionary<string, string> headers = null);
        Task<HttpResponseMessage> PostAsync<T>(string resource, T model, HttpClientWrapperAuthorizationModel authCredentials, Dictionary<string, string> headers = null);
        Task<HttpResponseMessage> PutAsync<T>(string resource, T model, HttpClientWrapperAuthorizationModel authCredentials, Dictionary<string, string> headers = null);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpClientWrapperAuthorizationModel authCredentials);
        Task<OAuthToken> GetTokenFromClientCredentials(string authorityUrl, string clientId, string clientSecret, string scope);
    }
}
