namespace FDD.Akka.Infrastructure
{
    public interface IClaimScanner
    {
        ScanResult Scan(string attachment);
    }
}