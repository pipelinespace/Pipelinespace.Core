using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Worker.Handlers.Extensions
{
    public static class HttpClientExtensions
    {
        public static async Task<T> MapTo<T>(this HttpResponseMessage response)
        {
            var model = await response.Content.ReadAsStringAsync();

            return JsonConvert.DeserializeObject<T>(model);
        }
    }
}
