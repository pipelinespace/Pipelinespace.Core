using Newtonsoft.Json;
using PipelineSpace.Domain.Core.Manager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PipelineSpace.Domain.Core.Manager
{
    public class DomainManagerService : IDomainManagerService
    {
        private List<DomainManagerMessage> _messages;
        private Dictionary<string, string> _results;

        public DomainManagerService()
        {
            _messages = new List<DomainManagerMessage>();
            _results = new Dictionary<string, string>();
        }

        public async Task AddConflict(string message)
        {
            DomainManagerMessage domainMessage = new DomainManagerMessage("", message, DomainManagerMessageType.Conflict);
            await Task.Run(() => { _messages.Add(domainMessage); });
        }

        public async Task AddNotFound(string message)
        {
            DomainManagerMessage domainMessage = new DomainManagerMessage("", message, DomainManagerMessageType.NotFound);
            await Task.Run(() => { _messages.Add(domainMessage); });
        }

        public async Task AddBadRequest(string message)
        {
            DomainManagerMessage domainMessage = new DomainManagerMessage("", message, DomainManagerMessageType.BadRequest);
            await Task.Run(() => { _messages.Add(domainMessage); });
        }

        public async Task AddForbidden(string message)
        {
            DomainManagerMessage domainMessage = new DomainManagerMessage("", message, DomainManagerMessageType.Unauthorized);
            await Task.Run(() => { _messages.Add(domainMessage); });
        }

        public async Task AddResult(string key, object value)
        {
            await Task.Run(() => { _results.Add(key, JsonConvert.SerializeObject(value)); });
        }

        public async Task<T> GetResult<T>(string key)
        {
            if (!_results.Keys.Any(x => x.Equals(key, StringComparison.InvariantCultureIgnoreCase)))
                throw new KeyNotFoundException($"The key ${key} was not found");

            return await Task.Run(() => { return JsonConvert.DeserializeObject<T>(_results[key]); });
        }

        public string GetConflicts()
        {
            StringBuilder stringBuilder = new StringBuilder();
            var conflicts = _messages.Where(x => x.DomainManagerMessageType == DomainManagerMessageType.Conflict);
            foreach (var item in conflicts)
            {
                stringBuilder.AppendLine(item.Value);
            }
            return stringBuilder.ToString();
        }

        public string GetNotFounds()
        {
            StringBuilder stringBuilder = new StringBuilder();
            var conflicts = _messages.Where(x => x.DomainManagerMessageType == DomainManagerMessageType.NotFound);
            foreach (var item in conflicts)
            {
                stringBuilder.AppendLine(item.Value);
            }
            return stringBuilder.ToString();
        }

        public string GetBadRequests()
        {
            StringBuilder stringBuilder = new StringBuilder();
            var conflicts = _messages.Where(x => x.DomainManagerMessageType == DomainManagerMessageType.BadRequest);
            foreach (var item in conflicts)
            {
                stringBuilder.AppendLine(item.Value);
            }
            return stringBuilder.ToString();
        }

        public string GetForbidden()
        {
            StringBuilder stringBuilder = new StringBuilder();
            var conflicts = _messages.Where(x => x.DomainManagerMessageType == DomainManagerMessageType.Unauthorized);
            foreach (var item in conflicts)
            {
                stringBuilder.AppendLine(item.Value);
            }
            return stringBuilder.ToString();
        }

        public bool HasConflicts()
        {
            return _messages.Any(x => x.DomainManagerMessageType == DomainManagerMessageType.Conflict);
        }

        public bool HasNotFounds()
        {
            return _messages.Any(x => x.DomainManagerMessageType == DomainManagerMessageType.NotFound);
        }

        public bool HasBadRequests()
        {
            return _messages.Any(x => x.DomainManagerMessageType == DomainManagerMessageType.BadRequest);
        }

        public bool HasForbidden()
        {
            return _messages.Any(x => x.DomainManagerMessageType == DomainManagerMessageType.Unauthorized);
        }

        public void Dispose()
        {
            _messages = new List<DomainManagerMessage>();
        }
    }
}
