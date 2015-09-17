namespace FDD.Akka.Infrastructure
{
    public interface IOpticalCharacterRecognizer
    {
        string Scan(string filePath);
    }
}