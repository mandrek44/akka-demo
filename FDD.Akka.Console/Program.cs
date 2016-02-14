using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.DI.AutoFac;
using Akka.DI.Core;
using Autofac;
using FDD.Akka.Infrastructure;
using FDD.Akka;

namespace FDD.Akka.Console
{
    class Program
    {
        static void Main(string[] args)
        {
            var system = ActorSystem.Create("MySystem");
            
            SetupDependencyInjection(system);

            RunAttachmentScanner(system);

            RunSupervisor(system);

            System.Console.ReadLine();
        }

        private static void RunSupervisor(ActorSystem system)
        {
            var actorRef = system.ActorOf<Supervisor>("supervisor");
        }

        private static void RunAttachmentScanner(ActorSystem system)
        {
            //var attachmetScannerActor = system.ActorOf(system
            //    .DI().Props<AttachmentScannerActor>());

            //var scannedAttachment = attachmetScannerActor.Ask<AttachmentScanned>(
            //    new ScanAttachment(new Attachment())).Result;

            //System.Console.WriteLine($"Scanned attachment {scannedAttachment.AttachmentContent}");
        }

        private static void SetupDependencyInjection(ActorSystem system)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterAssemblyTypes(typeof (ClaimsProcessorManager).Assembly).AsSelf().AsImplementedInterfaces();

            new AutoFacDependencyResolver(containerBuilder.Build(), system);
        }
    }
}
