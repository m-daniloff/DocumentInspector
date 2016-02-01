using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka.Actor;
using DocumentInspector.Messages;
using DocumentInspector.ViewModels;

namespace DocumentInspector.Actors
{
    public class DocumentInspectorSupervisor : BaseMonitoringActor
    {
        private readonly MainWindowViewModel _vm;

        public static Props GetProps(MainWindowViewModel vm)
        {
            return Props.Create(() => new DocumentInspectorSupervisor(vm));
        }

        public DocumentInspectorSupervisor(MainWindowViewModel vm)
        {
            _vm = vm;
            Ready();
        }

        private void Ready()
        {
            // receive from parent
            Receive<StartSearch>(msg => Handle(msg));

            // receive from validator
            Receive<ValidateArgs>(msg => Handle(msg));

            // receive when arguments are invalid
            Receive<InvalidArgs>(msg => Handle(msg));

            // receive from crawler
            Receive<ReportedFile>(msg => Handle(msg));

            // receive from children
            Receive<StatusMessage>(msg => Handle(msg));
            Receive<Done>(msg => Handle(msg));
        }

        private void Handle(StartSearch msg)
        {
            IncrementMessagesReceived();
            var validator = Context.ActorOf(FileValidatorActor.GetProps(), ActorPaths.FileValidator.Name);
            validator.Tell(new ValidateArgs(msg.Folders, msg.Extension));
        }

        private void Handle(ValidateArgs msg)
        {
            IncrementMessagesReceived();
            var child = Context.Child("directoryCrawler");
            if (child.Equals(ActorRefs.Nobody))
            {
                child = Context.ActorOf<DirectoryCrawler>("directoryCrawler");
            }

            child.Tell(new DirectoryToSearchMessage(msg.Folders, msg.Extension, _vm.TextSearch));
        }

        private void Handle(InvalidArgs msg)
        {
            IncrementMessagesReceived();
            _vm.Status = msg.ErrorMessage;
            _vm.Crawling = false;
        }

        private void Handle(ReportedFile msg)
        {
            IncrementMessagesReceived();
            _vm.AddItem.OnNext(new ResultItem()
            {
                FileNumber = msg.FileNumber,
                FilePath = msg.FileName,
                DirectoryPath = System.IO.Path.GetDirectoryName(msg.FileName),
                FileName = System.IO.Path.GetFileName(msg.FileName),
                DocumentAuthor = msg.Author,
                ElapsedMs = msg.ElapsedMilliseconds
            });
        }

        private void Handle(StatusMessage msg)
        {
            IncrementMessagesReceived();
            _vm.Status = msg.Message;
        }

        private void Handle(Done msg)
        {
            IncrementMessagesReceived();
            _vm.Crawling = false;
            _vm.Status = string.Format("Processed {0:N0} file(s) in total time of {1}", msg.Count, Convert(msg.ElapsedTime));
        }

        private String Convert(TimeSpan ts)
        {
            var result = String.Empty;

            if (ts.Hours > 0)
            {
                result = string.Format("{0:D} hours", ts.Hours);
            }

            if (ts.Minutes > 0)
            {
                result += string.Format(" {0:D} min", ts.Minutes);
            }
            if (ts.Seconds > 0)
            {
                result += string.Format(" {0:D} secs", ts.Seconds);
            }

            if (ts.Milliseconds > 0)
            {
                result += string.Format(" {0:D3} millsecs", ts.Milliseconds);
            }
            return result;
        }
    }
}
