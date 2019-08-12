using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Infra.Data.ServiceAgent.Models.BitBucket
{
    public class CMSBitBucketLinkModel
    {
        public CMSBitBucketLinkHrefModel Repositories { get; set; }
        public CMSBitBucketLinkHrefModel Hooks { get; set; }
    }

    public class CMSBitBucketLinkHrefModel
    {
        public string Href { get; set; }
        public string Name { get; set; }
    }
}
