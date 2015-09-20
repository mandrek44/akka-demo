using System;
using System.Collections.Generic;
using System.Linq;
using FDD.Akka.Infrastructure;

namespace FDD.Akka
{
    public class ClaimsProcessor
    {
        public void Process(IMailClient mailClient, IOpticalCharacterRecognizer ocr, IClaimScanner claimScanner, IClaimManagementSystem claimManagement)
        {
            var emails = mailClient.ReadMessages();

            foreach (var emailMessage in emails)
            {
                var recognizedAttachmentsContents = new List<string>();
                foreach (var attachment in emailMessage.Attachments)
                {
                    try
                    {
                        recognizedAttachmentsContents.Add(ocr.Scan(attachment.Path));
                    }
                    catch (Exception e)
                    {
                        ProcessFailedAttachment(attachment, e);
                    }
                }

                var claims = recognizedAttachmentsContents
                    .Select(claimScanner.Scan)
                    .Where(result => result.Success)
                    .Select(result => result.Claim)
                    .ToList();

                foreach (var claim in claims)
                {
                    claimManagement.Upload(claim);
                }
            }
        }

        private void ProcessFailedAttachment(Attachment attachment, Exception exception)
        {
            // TODO: failed attachment error handling
        }
    }
}