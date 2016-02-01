using System;
using System.Diagnostics;
using System.IO;
using Akka.Actor;
// for logging
using Akka.Event;
//using Akka.Routing;
using DocumentInspector.Messages;
namespace DocumentInspector.Actors
{
    public class DirectoryCrawler : BaseMonitoringActor
    {
        // Testing concurrent processing
        //private IActorRef router;
        // Testing logging
        private ILoggingAdapter _logger = Context.GetLogger();
        private bool CrawlingDone = false;
        private int fileno = 0;
        private int fileProcessed = 0;
        private int filesCrawled;
        private string _textToSearch = String.Empty;
        private readonly Stopwatch m_sw = new Stopwatch();

        /// <summary>
        /// Initializes a new instance of the <see cref="DirectoryCrawler"/> class.
        /// </summary>
        public DirectoryCrawler()
        {
            Ready();
        }

        private void Ready()
        {
            Receive<DirectoryToSearchMessage>(msg => Handle(msg));
            Receive<FileInfo>(msg => Handle(msg));
            Receive<CompletedFile>(msg => Handle(msg));
            Receive<DoneEnumeratingFiles>(msg => DoneEnumerating(msg));
            Receive<FailureMessage>(msg => Handle(msg));
        }

        public void Handle(DirectoryToSearchMessage message)
        {
            IncrementMessagesReceived();
            fileno = 0;
            fileProcessed = 0;
            filesCrawled = 0;
            CrawlingDone = false;
            _textToSearch = message.TextToSearch;
            m_sw.Start();
            //router = Context.ActorOf(new RoundRobinPool(8).Props(DocumentActor.GetProps()),
            //   String.Format("File{0}", fileno));
            var EnumeratorActor = Context.ActorOf(FileEnumeratorActor.GetProps());
            EnumeratorActor.Tell(message);
        }

        private void Handle(FileInfo msg)
        {
            IncrementMessagesReceived();
            fileno++;



            //router.Tell(new FileToProcess(msg.FullName, fileno, _textToSearch));

            var documentActor = Context.ActorOf(DocumentActor.GetProps());
            documentActor.Tell(new FileToProcess(msg.FullName, fileno, _textToSearch));
            Context.Parent.Tell(new StatusMessage("Processing file " + msg.FullName));
        }

        public void Handle(CompletedFile message)
        {
            IncrementMessagesReceived();
            fileProcessed++;
            ReportedFile reportedFileMsg = new ReportedFile(message.FileName, message.ElapsedMilliseconds, message.Author, fileProcessed);
            Context.Parent.Tell(reportedFileMsg);
            CrawlingFinished();
        }
        private void DoneEnumerating(DoneEnumeratingFiles msg)
        {
            IncrementMessagesReceived();
            filesCrawled = msg.Count;

            CrawlingDone = true;
            CrawlingFinished();
        }
        public void Handle(FailureMessage fail)
        {
            IncrementMessagesReceived();
            fileProcessed++;
            var exception = fail.Cause;
            if (exception is AggregateException)
            {
                var agg = (AggregateException)exception;
                exception = agg.InnerException;
                agg.Handle(exception1 => true);
            }
            //Context.Parent.Tell(new StatusMessage("Error " + fail.Child.Path + " " + exception != null ? exception.Message : "no exception object"));
            _logger.Error(fail.Cause.Message);
        }

        private void CrawlingFinished()
        {
            if (CrawlingDone && (fileProcessed == fileno))
            {
                m_sw.Stop();
                Context.Parent.Tell(new Done(fileProcessed, m_sw.Elapsed));
                m_sw.Reset();
                Sender.Tell(PoisonPill.Instance);
            }
        }

        protected override void PreRestart(Exception reason, object message)
        {
            // preserve all children in the event of a restart
        }
    }
}
