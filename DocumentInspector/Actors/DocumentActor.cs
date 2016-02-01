using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Akka.Actor;
using Akka.IO;
using DocumentInspector.Messages;

namespace DocumentInspector.Actors
{
    public class DocumentActor : BaseMonitoringActor
    {
        private string _fileName;
        private readonly Stopwatch m_sw;
        public static Props GetProps()
        {
            return Props.Create<DocumentActor>();
        }

        /// <summary>
        /// Initializes a new instance of the DocumentActor class
        /// </summary>
        public DocumentActor()
        {
            _fileName = String.Empty;
            m_sw = new Stopwatch();
            Ready();
        }

        private void Ready()
        {
            Receive<FileToProcess>(msg => Handle(msg));
            Receive<DocumentResultMessage>(msg => Handle(msg));
            Receive<FailureMessage>(msg => Handle(msg));
        }

        private void Handle(FileToProcess msg)
        {
            IncrementMessagesReceived();
            m_sw.Start();
            _fileName = msg.FileName;
            var docReaderActor = Context.ActorOf(DocumentReaderActor.GetProps(), String.Format("File{0}", msg.Fileno));
            try
            {
                docReaderActor.Tell(new ProcessDocumentMessage(_fileName, msg.TextToSearch));
            }
            catch (Exception ex)
            {
                Sender.Tell(new FailureMessage(ex, Self));
            }
        }

        private void Handle(DocumentResultMessage msg)
        {
            IncrementMessagesReceived();
            m_sw.Stop();
            if (msg.TextFound)
                Context.Parent.Tell(new CompletedFile(_fileName, m_sw.ElapsedMilliseconds, msg.Author));
            Self.Tell(PoisonPill.Instance);
        }

        private void Handle(FailureMessage failureMessage)
        {
            IncrementMessagesReceived();
            Context.Parent.Tell(failureMessage);
        }

        protected override void PreRestart(Exception reason, object message)
        {
            Context.Parent.Tell(new FailureMessage(reason, Self));
        }
    }
}
