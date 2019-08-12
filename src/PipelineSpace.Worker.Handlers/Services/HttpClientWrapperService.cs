using Microsoft.ApplicationInsights.DataContracts;
using Newtonsoft.Json;
using PipelineSpace.Worker.Handlers.Models;
using PipelineSpace.Worker.Handlers.Services.Interfaces;
using PipelineSpace.Worker.Monitor;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Services
{
    public class HttpClientWrapperService : IHttpClientWrapperService
    {
        readonly IHttpClientFactory _httpClientFactory;
        public HttpClientWrapperService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory;
        }

        public async Task<HttpResponseMessage> DeleteAsync(string resource, HttpClientWrapperAuthorizationModel authCredentials, Dictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, resource);

            return await this.SendAsync(request, authCredentials, headers);
        }

        public async Task<HttpResponseMessage> DeleteAsync<T>(string resource, T model, HttpClientWrapperAuthorizationModel authCredentials, Dictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Delete, resource);
            request.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            return await this.SendAsync(request, authCredentials, headers);
        }

        public async Task<HttpResponseMessage> GetAsync(string resource, HttpClientWrapperAuthorizationModel authCredentials, Dictionary<string, string> headers = null, bool cache = false, double cacheExpiration = 60)
        {
            var request = new HttpRequestMessage(HttpMethod.Get, resource);
            return await this.SendAsync(request, authCredentials, headers);
        }

        public async Task<HttpResponseMessage> PatchAsync<T>(string resource, T model, HttpClientWrapperAuthorizationModel authCredentials, Dictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage(new HttpMethod("PATCH"), resource);
            request.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            return await this.SendAsync(request, authCredentials, headers);
        }

        public async Task<HttpResponseMessage> UploadFileAsync<T>(string resource, T model, HttpClientWrapperAuthorizationModel authCredentials, Dictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, resource);
            request.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            return await this.SendAsync(request, authCredentials, headers);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string resource, T model, HttpClientWrapperAuthorizationModel authCredentials, Dictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Post, resource);
            request.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            return await this.SendAsync(request, authCredentials, headers);
        }

        public async Task<HttpResponseMessage> PutAsync<T>(string resource, T model, HttpClientWrapperAuthorizationModel authCredentials, Dictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage(HttpMethod.Put, resource);
            request.Content = new StringContent(JsonConvert.SerializeObject(model), Encoding.UTF8, "application/json");

            return await this.SendAsync(request, authCredentials, headers);
        }

        public async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpClientWrapperAuthorizationModel authCredentials)
        {
            return await SendAsync(request, authCredentials, null);
        }

        private async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, HttpClientWrapperAuthorizationModel authCredentials, Dictionary<string, string> headers)
        {
            try
            {
                if(authCredentials != null)
                {
                    request.Headers.Authorization = new AuthenticationHeaderValue(authCredentials.Schema, authCredentials.Value);
                }
                
                if (headers != null)
                {
                    foreach (var item in headers)
                    {
                        request.Headers.Add(item.Key, (string)item.Value);
                    }
                }

                var startTime = DateTime.UtcNow;
                var timer = System.Diagnostics.Stopwatch.StartNew();

                var response = await _httpClientFactory.CreateClient("RemoteServerFromWorker").SendAsync(request);

                timer.Stop();

                TelemetryClientManager.Instance.TrackDependency(new DependencyTelemetry()
                {
                    Type = "HTTP",
                    Data = $"Call {request.RequestUri.AbsolutePath}",
                    Timestamp = DateTime.UtcNow,
                    Duration = timer.Elapsed,
                    Success = response.IsSuccessStatusCode,
                    Name = $"Call {request.RequestUri.AbsolutePath}",
                    Target = request.RequestUri.Host
                });

                return response;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public async Task<OAuthToken> GetTokenFromClientCredentials(string authorityUrl, string clientId, string clientSecret, string scope)
        {
            var keyValues = new List<KeyValuePair<string, string>>();
            keyValues.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
            keyValues.Add(new KeyValuePair<string, string>("client_id", clientId));
            keyValues.Add(new KeyValuePair<string, string>("client_secret", clientSecret));
            keyValues.Add(new KeyValuePair<string, string>("scope", scope));

            using(var httpClient = new HttpClient())
            {
                var response = await httpClient.PostAsync(authorityUrl, new FormUrlEncodedContent(keyValues));

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
