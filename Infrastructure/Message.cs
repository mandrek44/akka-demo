using System.Collections.Generic;

namespace FDD.Akka.Infrastructure
{
    public class Message
    {
        public IEnumerable<Attachment> Attachments { get; set; }
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

    class MailReceived
    {
        public IEnumerable<Attachment> Attachments { get; }

        public MailReceived(IEnumerable<Attachment> attachments) { Attachments = attachments; }
    }

    class ClaimDetected
    {
        public Claim Claim { get; }

        public ClaimDetected(Claim claim) { Claim = claim; }
    }

    class ScanClaim
    {
        public string AttachmentContent { get; }

        public ScanClaim(string attachmentContent) { AttachmentContent = attachmentContent; }
    }
}