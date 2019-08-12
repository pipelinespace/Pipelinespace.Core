using System;

namespace PipelineSpace.Infra.Options
{
    public class NotificationOptions
    {
        public SendGridOptions SendGrid { get; set; }
    }

    public class SendGridOptions
    {
        public string ApiKey { get; set; }
        public string From { get; set; }
    }
}
