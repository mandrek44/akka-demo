using Akka.Actor;
using FDD.Akka.Infrastructure;

namespace FDD.Akka
{
    class AttachmentScannerActor : ReceiveActor
    {
        public AttachmentScannerActor()
        {
            Receive<ScanAttachment>(message =>
            {
            });
        }
    }

    class ScanAttachment
    {
        public Attachment Attachment { get; }

        public ScanAttachment(Attachment attachment) { Attachment = attachment; }
    }
}