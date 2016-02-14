using Akka.Actor;
using Akka.Event;
using FDD.Akka.Infrastructure;

namespace FDD.Akka
{
    class AttachmentScannerActor : LoggingReceiveActor
    {
        public AttachmentScannerActor(IOpticalCharacterRecognizer ocr)
        {
            Receive<ScanAttachment>(message =>
            {
                var attachmentContent = ocr.Scan(message.Attachment.Path);
                Sender.Tell(new AttachmentScanned(attachmentContent));
            });
        }
    }

    class LoggingReceiveActor : ReceiveActor
    {
        protected readonly ILoggingAdapter Log = Context.GetLogger();

        protected override bool AroundReceive(Receive receive, object message)
        {
            Log.Info($"{Self.Path.Name} received {message.GetType().Name}" );
            return base.AroundReceive(receive, message);
        }
    }

    class ClaimsProcessingDirector : LoggingReceiveActor
    {
        private readonly IActorRef _mailMonitor;

        public ClaimsProcessingDirector(IActorRef attachmentScanner,
            IActorRef claimScanner,
            IActorRef claimManagementSystem,
            IActorRef mailMonitor)
        {
            _mailMonitor = mailMonitor;

            Receive<MailReceived>(message =>
            {
                foreach (var attachment in message.Attachments)
                    attachmentScanner.Tell(new ScanAttachment(attachment));
            });

            Receive<AttachmentScanned>(message =>
            {
                claimScanner.Tell(new ScanClaim(message.AttachmentContent));
            });

            Receive<ClaimDetected>(message =>
            {
                claimManagementSystem.Tell(new UploadClaim(message.Claim));
            });
        }

        protected override void PreStart()
        {
            _mailMonitor.Tell(MailMonitorPrompter.StartObserving.Message);
        }

        protected override void PostStop()
        {
            _mailMonitor.Tell(MailMonitorPrompter.StopObserving.Message);
        }
    }

    class ClaimScannerActor : LoggingReceiveActor
    {
        public ClaimScannerActor(IClaimScanner claimScanner)
        {
            Receive<ScanClaim>(message =>
            {
                var scanResult = claimScanner.Scan(message.AttachmentContent);
                if (scanResult.Success)
                    Sender.Tell(new ClaimDetected(scanResult.Claim));
            });
        }
    }

    internal class ClaimManagementSystemActor : LoggingReceiveActor
    {
        public ClaimManagementSystemActor(IClaimManagementSystem claimManagementSystem)
        {
            Receive<UploadClaim>(message =>
            {
                claimManagementSystem.Upload(message.Claim);
            });
        }
    }
}