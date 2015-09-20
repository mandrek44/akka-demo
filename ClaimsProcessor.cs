using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using FDD.Akka.Infrastructure;

namespace FDD.Akka
{
    public class ClaimsProcessorManager
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
                    try
                    {
                        var scanResult = claimScanner.Scan(recognizedAttachmentsContent);
                        if (scanResult.Success)
                            claims.Add(scanResult.Claim);
                        else
                            throw new Exception("Unable to find claim in attachment");
                    }
                    catch (Exception e)
                    {
                        ProcessFailedClaim(recognizedAttachmentsContent, e);
                    }
                }

                foreach (var claim in claims)
                {
                    int retryCount = 3;
                    while (true)
                    {
                        try
                        {
                            claimManagement.Upload(claim);
                        }
                        catch (Exception)
                        {
                            retryCount--;
                            if (retryCount < 0)
                                throw;
                            else
                                Thread.Sleep(5000);
                        }
                    }
                }
            }
        }

        private void ProcessFailedClaim(string recognizedAttachmentsContent, Exception exception)
        {
            // TODO: failed claim error handling
        }

        private void ProcessFailedAttachment(Attachment attachment, Exception exception)
        {
            // TODO: failed attachment error handling
        }
    }
}