using System.Collections.Generic;

namespace FDD.Akka.Infrastructure
{
    public interface IMailClient
    {
        IEnumerable<Message> ReadMessages();
    }
}