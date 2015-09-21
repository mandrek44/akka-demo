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
    class AttachmentScanned
    {
        public string AttachmentContent { get; }

        public AttachmentScanned(string attachmentContent) { AttachmentContent = attachmentContent; }
    }

    class ScanAttachment
    {
        public Attachment Attachment { get; }

        public ScanAttachment(Attachment attachment) { Attachment = attachment; }
    }
}