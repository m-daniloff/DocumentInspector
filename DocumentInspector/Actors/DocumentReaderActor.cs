using System;
using System.Linq;
using Akka.Actor;
//logging
using Akka.Event;
using DocumentInspector.Messages;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
namespace DocumentInspector.Actors
{
    public class DocumentReaderActor : BaseMonitoringActor
    {
        private ILoggingAdapter _logger = Context.GetLogger();
        private string Author { get; set; }
        public static Props GetProps()
        {
            return Props.Create<DocumentReaderActor>();
        }

        public DocumentReaderActor()
        {
            Ready();
        }

        private void Ready()
        {
            Receive<ProcessDocumentMessage>(msg => Handle(msg));
        }

        private void Handle(ProcessDocumentMessage message)
        {
            IncrementMessagesReceived();
            bool bResult = ProcessDocument(message.FilePath, message.TextToSearch);
            Sender.Tell(new DocumentResultMessage(bResult, Author));
        }

        //TODO: maybe instead of bool, we should send different messages?
        private bool ProcessDocument(string filePath, string textToSearch)
        {
            _logger.Info("Processing File {0}", filePath);
            bool found = false;
            try
            {
                using (WordprocessingDocument document = WordprocessingDocument.Open(filePath, false))
                {
                    found = document.MainDocumentPart.Document.Descendants<Text>().Any(element => element.Text.Contains(textToSearch));

                    if (String.IsNullOrEmpty(textToSearch))
                        found = true;

                    if (found)
                    {
                        Author = document.PackageProperties.Creator;
                    }
                }

                return found;
            }
            catch (Exception exception)
            {
                Sender.Tell(new FailureMessage(exception, Self));
                return false;
            }
        }

        protected override void PreRestart(Exception reason, object message)
        {
            Context.Parent.Tell(new FailureMessage(reason, Self));
        }
    }

    
}
