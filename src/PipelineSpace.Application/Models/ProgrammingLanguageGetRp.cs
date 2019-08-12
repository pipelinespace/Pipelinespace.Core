using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using PipelineSpace.Domain.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace PipelineSpace.Application.Models
{
    public class ProgrammingLanguageListRp
    {
        public ProgrammingLanguageListRp()
        {
            this.Items = new List<ProgrammingLanguageListItemRp>();
        }

        public IReadOnlyList<ProgrammingLanguageListItemRp> Items { get; set; }
    }

    public class ProgrammingLanguageListItemRp
    {
        public Guid ProgrammingLanguageId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class ProgrammingLanguageGetRp
    {
        public Guid ProgrammingLanguageId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
