using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ClassLibrary1
{
    public class Class1
    {
        // Proces
            // Retrieve all non processed emails by date (pull)
            // Scan each email for attachments
            // Scan each attachment for claim data (resource intensive)
            // Create claim in claim management system
            // Wait for claim to be created (pull)
            // Upload claim files 

        // Do napisania
            // Fake'owy pulse ktory odpowiada po jakims czasie
            // Fake'owy serwer mejlowy ktory co jakis czas daje nowe dane do przetwarzania


        public void Do()
        {            
        }
    }

    public interface IMailClient
    {
        IEnumerable<Message> ReadMessages();
    }

    public interface IOpticalCharacterRecognizer
    {
        string Scan(string filePath);
    }

    public interface IClaimScanner
    {
        ScanResult Scan(ScannedAttachment attachment);
    }

    public class ScanResult
    {
        public bool Success { get; set; }

        public Claim Claim { get; set; }
    }

    public class Claim
    {
        public string ClaimId { get; set; }
    }

    public interface IClaimManagementSystem
    {
        void Upload(Claim claim);
    }

    public class ClaimsProcessingManager
    {
        public void Process(IMailClient mailClient, IOpticalCharacterRecognizer ocr, IClaimScanner claimScanner, IClaimManagementSystem claimManagement)
        {
            var emails = mailClient.ReadMessages();

            foreach (var emailMessage in emails)
            {
                IEnumerable<ScannedAttachment> recognizedAttachments;
                try
                {
                    
                    recognizedAttachments = emailMessage.Attachments
                        .Select(attachment => new ScannedAttachment { Attachment = attachment, StringContent = ocr.Scan(attachment.Path) })
                        .ToArray();
                }
                catch (OCRException e)
                {
                    ProcessFailedEmail(emailMessage, "Error while scanning attachment", e);
                    continue;
                }

                IEnumerable<Claim> claims;
                try
                {
                    claims = recognizedAttachments
                        .Select(claimScanner.Scan)
                        .Where(result => result.Success)
                        .Select(result => result.Claim)
                        .ToList();
                }
                catch (ScannerException e)
                {
                    ProcessFailedEmail(emailMessage, "Error while scanning claims data", e);
                    continue;
                }

                foreach (var claim in claims)
                {
                    claimManagement.Upload(claim);
                }
            }
        }

        private void ProcessFailedEmail(Message emailMessage, string errorWhileProcessingAttachment, Exception ocrException)
        {
            // Send it to failed messages mailbox
        }
    }

    public class ScannerException : Exception
    {
    }

    public class OCRException : Exception
    {
    }

    public class Message
    {
        public IEnumerable<Attachment> Attachments { get; set; }
    }

    public class Attachment
    {
        public string Path { get; set; }
    }

    public class ScannedAttachment
    {
        public Attachment Attachment { get; set; }

        public string StringContent { get; set; }
    }
}
