using PipelineSpace.Application.Interfaces.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace PipelineSpace.Infra.Data.ServiceAgent
{
    public interface IHttpProxyService
    {
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CMSAuthCredentialModel authCredentials, Dictionary<string, string> headers);
        Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CMSAuthCredentialModel authCredentials);
        Task<HttpResponseMessage> GetAsync(string resource, CMSAuthCredentialModel authCredentials, Dictionary<string, string> headers = null, bool cache = false, double cacheExpiration = 60);
        Task<HttpResponseMessage> PostAsync<T>(string resource, T model, CMSAuthCredentialModel authCredentials, Dictionary<string, string> headers = null);
        Task<HttpResponseMessage> PatchAsync<T>(string resource, T model, CMSAuthCredentialModel authCredentials, Dictionary<string, string> headers = null);
        Task<HttpResponseMessage> PutAsync<T>(string resource, T model, CMSAuthCredentialModel authCredentials, Dictionary<string, string> headers = null);
        Task<HttpResponseMessage> DeleteAsync(string resource, CMSAuthCredentialModel authCredentials, Dictionary<string, string> headers = null);
        Task<HttpResponseMessage> DeleteAsync<T>(string resource, T model, CMSAuthCredentialModel authCredentials, Dictionary<string, string> headers = null);
    }
}
