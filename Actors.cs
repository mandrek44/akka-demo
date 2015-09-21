using Akka.Actor;

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
}