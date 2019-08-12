using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PipelineSpace.Presentation.Models
{
    public class BreadCrumbModel
    {
        public BreadCrumbModel()
        {
            Items = new List<BreadCrumbItem>();
        }
        public void Add(string url, string text)
        {
            Items.Add(new BreadCrumbItem() { Url = url, Text = text });
        }
        public List<BreadCrumbItem> Items { get; set; }
        public string Text { get; set; }
    }

    public class BreadCrumbItem
    {
        public string Text { get; set; }
        public string Url { get; set; }
    }
}
