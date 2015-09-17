using System.Collections.Generic;

namespace FDD.Akka.Infrastructure
{
    public class Message
    {
        public IEnumerable<Attachment> Attachments { get; set; }
    }
}