using System;

namespace PipelineSpace.Infra.Options
{
    public class VSTSServiceOptions
    {
        public string AccountId { get; set; }
        public string AccessId { get; set; }
        public string AccessSecret { get; set; }
        public string ApiVersion { get; set; }
        public string ProjectName { get; set; }
    }

    public class FakeAccountServiceOptions
    {
        public string AccountId { get; set; }
        public string AccessId { get; set; }
        public string AccessSecret { get; set; }
        public string ApiVersion { get; set; }
    }
}
