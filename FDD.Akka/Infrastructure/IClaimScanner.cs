namespace FDD.Akka.Infrastructure
{
    public interface IClaimScanner
    {
        ScanResult Scan(string attachment);
    }

    class FakeClaimScanner : IClaimScanner
    {
        public ScanResult Scan(string attachment)
        {
            return new ScanResult()
            {
                Claim = new Claim() {ClaimId = RandomString.Next(8)},
                Success = true
            };
        }
    }
}