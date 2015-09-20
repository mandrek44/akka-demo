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

                var claims = new List<Claim>();
                foreach (var recognizedAttachmentsContent in recognizedAttachmentsContents)
                {
                    var scanResult = claimScanner.Scan(recognizedAttachmentsContent);
                    if (scanResult.Success)
                        claims.Add(scanResult.Claim);
                }

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