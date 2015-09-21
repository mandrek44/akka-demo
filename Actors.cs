using Akka.Actor;
using FDD.Akka.Infrastructure;

namespace FDD.Akka
{
    class AttachmentScannerActor : ReceiveActor
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

    class ClaimsProcessingDirector : ReceiveActor
    {
        public ClaimsProcessingDirector(IActorRef attachmentScanner,
            IActorRef claimScanner)
        {
            Receive<MailReceived>(message =>
            {
                foreach (var attachment in message.Attachments)
                    attachmentScanner.Tell(new ScanAttachment(attachment));
            });

            Receive<AttachmentScanned>(message =>
            {
                claimScanner.Tell(new ScanClaim(message.AttachmentContent));
            });
        }
    }

    class ClaimScannerActor : ReceiveActor
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
}