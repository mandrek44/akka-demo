using System;
using System.Collections.Generic;
using System.Linq;

namespace FDD.Akka.Infrastructure
{
    public interface IMailClient
    {
        IEnumerable<Message> ReadMessages();
    }

    class FakeMailClient : IMailClient
    {
        private readonly Random _random = new Random(128);
        private DateTime _nextRead = DateTime.MinValue;

        public IEnumerable<Message> ReadMessages()
        {
            if (DateTime.Now < _nextRead)
                return new Message[] {};

            _nextRead = DateTime.Now.AddSeconds(_random.Next(1, 5));

            var messageCount = _random.Next(3);
            return Enumerable.Range(1, messageCount).Select(_ => GenerateMessage()).ToArray();
        }

        private Message GenerateMessage()
        {
            return new Message()
            {
                Attachments =
                    Enumerable.Range(1, _random.Next(1, 2)).Select(_ => new Attachment()).ToArray()
            };
        }
    }
}