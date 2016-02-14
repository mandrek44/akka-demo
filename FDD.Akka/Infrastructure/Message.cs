using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.Core;
using Akka.Routing;

namespace FDD.Akka.Infrastructure
{
    public class Message
    {
        public IEnumerable<Attachment> Attachments { get; set; }
    }

    public class AttachmentScanned
    {
        public string AttachmentContent { get; }

        public AttachmentScanned(string attachmentContent) { AttachmentContent = attachmentContent; }
    }

    public class ScanAttachment
    {
        public Attachment Attachment { get; }

        public ScanAttachment(Attachment attachment) { Attachment = attachment; }
    }

    public class MailReceived
    {
        public IEnumerable<Attachment> Attachments { get; }

        public MailReceived(IEnumerable<Attachment> attachments) { Attachments = attachments; }
    }

    public class ClaimDetected
    {
        public Claim Claim { get; }

        public ClaimDetected(Claim claim) { Claim = claim; }
    }

    public class ScanClaim
    {
        public string AttachmentContent { get; }

        public ScanClaim(string attachmentContent) { AttachmentContent = attachmentContent; }
    }

    public class UploadClaim
    {
        public Claim Claim { get; }

        public UploadClaim(Claim claim) { Claim = claim; }
    }

    public class Supervisor : ReceiveActor
    {
        protected override void PreStart()
        {
            var attachmentScanner = Context.ActorOf(
                Context.DI().Props<AttachmentScannerActor>()
                //.WithRouter(FromConfig.Instance)
                , "attachmentScanner");
            var claimsScanner = Context.ActorOf(
                Context.DI().Props<ClaimScannerActor>(), "claimsScanner");
            var claimManagementSystem = Context.ActorOf(
                Context.DI().Props<ClaimManagementSystemActor>(), "claimManagementSystem");
            var mailMonitor = Context.ActorOf(
                Context.DI().Props<MailMonitorPrompter>(), "mailMonitor");

            var claimProcessor = Context.ActorOf(
                Props.Create(() =>
                    new ClaimsProcessingDirector(attachmentScanner, claimsScanner, claimManagementSystem, mailMonitor)), 
                "claimProcessor");
        }
    }

    public class MailMonitorPrompter : ReceiveActor
    {
        private readonly IMailClient _mailClient;
        private readonly HashSet<IActorRef> _mailObservers = new HashSet<IActorRef>();
        private static readonly TimeSpan MailCheckInterval = TimeSpan.FromSeconds(5);

        public MailMonitorPrompter(IMailClient mailClient)
        {
            _mailClient = mailClient;

            Receive<CheckForMail>(_ =>
            {
                Task.Run(() => _mailClient.ReadMessages())
                    .ContinueWith(readTask => new MailArrived(readTask.Result))
                    .PipeTo(Self);
            });

            Receive<MailArrived>(message =>
            {
                ScheduleNextMailCheck();

                foreach (var mailObserver in _mailObservers)
                    foreach (var mail in message.Messages)
                        mailObserver.Tell(new MailReceived(mail.Attachments));
            });

            Receive<StartObserving>(_ => _mailObservers.Add(Sender));
            Receive<StopObserving>(_ => _mailObservers.Remove(Sender));
        }

        protected override void PreStart()
        {
            base.PreStart();

            ScheduleNextMailCheck();
        }

        private void ScheduleNextMailCheck()
        {
            Context.System.Scheduler.ScheduleTellOnce(
                MailCheckInterval,
                Self,
                new CheckForMail(),
                Self);
        }

        public class StartObserving
        {
            public static readonly StartObserving Message = new StartObserving();

            private StartObserving() { }
        }

        public class StopObserving
        {
            public static readonly StopObserving Message = new StopObserving();

            private StopObserving() { }
        }

        private class CheckForMail { }

        private class MailArrived
        {
            public IEnumerable<Message> Messages { get; }

            public MailArrived(IEnumerable<Message> messages) { Messages = messages; }
        }
    }
}