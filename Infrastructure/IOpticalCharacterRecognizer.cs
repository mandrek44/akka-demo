using System;
using System.Linq;
using System.Threading;
using NLog;

namespace FDD.Akka.Infrastructure
{
    public interface IOpticalCharacterRecognizer
    {
        string Scan(string filePath);
    }

    class FakeOpticalCharacterRecognizer : IOpticalCharacterRecognizer
    {
        private readonly Random _random = new Random();
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public string Scan(string filePath)
        {
            // Long runnig operation
            _log.Debug("Beginning OCR scan");
            Thread.Sleep(TimeSpan.FromSeconds(_random.Next(3, 8)));
            _log.Debug("Finished OCR scan");

            return RandomString.Next(8);
        }
    }

    static class RandomString
    {
        private static readonly Random _random = new Random();
        private const string Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";

        public static string Next(int length)
        {
            return new string(Enumerable
                .Repeat(Chars, length)
                .Select(s => s[_random.Next(s.Length)])
                .ToArray());
        }
    }
}