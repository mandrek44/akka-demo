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
                var recognizedAttachmentsContents = emailMessage.Attachments
                    .Select(attachment => ocr.Scan(attachment.Path))
                    .ToArray();               
            }
        }
    }
}