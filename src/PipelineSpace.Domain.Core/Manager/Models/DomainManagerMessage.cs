using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Domain.Core.Manager.Models
{
    public class DomainManagerMessage 
    {
        public Guid DomainManagerMessageId { get; private set; }
        public DomainManagerMessageType DomainManagerMessageType { get; set; }
        public string Key { get; private set; }
        public string Value { get; private set; }
        public int Version { get; private set; }

        public DomainManagerMessage(string key, string value, DomainManagerMessageType type)
        {
            DomainManagerMessageId = Guid.NewGuid();
            DomainManagerMessageType = type;
            Version = 1;
            Key = key;
            Value = value;
        }
    }
}
