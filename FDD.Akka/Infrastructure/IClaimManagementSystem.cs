using NLog;

namespace FDD.Akka.Infrastructure
{
    public interface IClaimManagementSystem
    {
        void Upload(Claim claim);
    }

    class StubClaimManagementSystem : IClaimManagementSystem
    {
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        public void Upload(Claim claim)
        {
            _log.Info($"Uploaded claim {claim.ClaimId}");
        }
    }
}