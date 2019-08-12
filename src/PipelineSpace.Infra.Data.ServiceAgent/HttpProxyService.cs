using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Net.Http.Headers;
using System.Text;
using PipelineSpace.Application.Interfaces.Models;

namespace PipelineSpace.Infra.Data.ServiceAgent
{
    public class HttpProxyService : IHttpProxyService
    {
        readonly IHttpClientFactory _httpClientFactory;
        public HttpProxyService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HttpResponseMessage> DeleteAsync(string resource, CMSAuthCredentialModel authCredentials, Dictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, resource);

            return await this.SendAsync(request, authCredentials, headers);
        }

        public async Task<HttpResponseMessage> DeleteAsync<T>(string resource, T model, CMSAuthCredentialModel authCredentials, Dictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, resource);
            request.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            return await this.SendAsync(request, authCredentials, headers);
        }

        public async Task<HttpResponseMessage> GetAsync(string resource, CMSAuthCredentialModel authCredentials, Dictionary<string, string> headers = null, bool cache = false, double cacheExpiration = 60)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, resource);
            return await this.SendAsync(request, authCredentials, headers);
        }

        public async Task<HttpResponseMessage> PatchAsync<T>(string resource, T model, CMSAuthCredentialModel authCredentials, Dictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), resource);
            request.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            return await this.SendAsync(request, authCredentials, headers);
        }

        public async Task<HttpResponseMessage> UploadFileAsync<T>(string resource, T model, CMSAuthCredentialModel authCredentials, Dictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, resource);
            request.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            return await this.SendAsync(request, authCredentials, headers);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string resource, T model, CMSAuthCredentialModel authCredentials, Dictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, resource);
            request.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            return await this.SendAsync(request, authCredentials, headers);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string resource, T model, CMSAuthCredentialModel authCredentials, Dictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, resource);
            request.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            return await this.SendAsync(request, authCredentials, headers);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CMSAuthCredentialModel authCredentials)
        {
            return await SendAsync(request, authCredentials, null);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CMSAuthCredentialModel authCredentials, Dictionary<string, string> headers)
        {
            try
            {
                request.Headers.Authorization = new AuthenticationHeaderValue(authCredentials.Type, authCredentials.AccessToken);

                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        request.Headers.Add(item.Key, (string)item.Value);
                    }
                }

                var client = _httpClientFactory.CreateClient("RemoteServerFromService");
                client.BaseAddress = new Uri(authCredentials.Url);
                var response = await client.SendAsync(request);
                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        
    }
}
