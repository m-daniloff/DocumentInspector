using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Akka.Actor;

namespace DocumentInspector.Messages
{
    public class StartSearch
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StartSearch"/> class.
        /// </summary>
        public StartSearch(string folders, string extension)
        {
            Extension = extension;
            Folders = folders;
        }
        public string Folders { get; private set; }
        public string Extension { get; private set; }

        
    }

    public class ValidateArgs
    {
        public ValidateArgs(string folders, string extension)
        {
            Extension = extension;
            Folders = folders;
        }
        public string Folders { get; private set; }
        public string Extension { get; private set; }
    }
    public class InvalidArgs
    {
        public InvalidArgs(string errorMessage)
        {
            ErrorMessage = errorMessage;
        }
        public string ErrorMessage { get; private set; }
    }
    public class ValidArgs
    {
        public ValidArgs(string fullpath)
        {
            Fullpath = fullpath;
        }
        public string Fullpath { get; private set; }
    }

    public class DirectoryToSearchMessage
    {
        public DirectoryToSearchMessage(string directory, string searchPattern, string textToSearch)
        {
            Directory = directory;
            SearchPattern = searchPattern;
            TextToSearch = textToSearch;
        }
        public string SearchPattern { get; private set; }
        public string Directory { get; private set; }

        public string TextToSearch { get; private set; }
    }
    public class FileToProcess
    {
        public FileToProcess(string fileName, int fileno, string textToSearch)
        {
            Fileno = fileno;
            FileName = fileName;
            TextToSearch = textToSearch;
        }

        public string FileName { get; private set; }
        public int Fileno { get; private set; }

        public string TextToSearch { get; private set; }
    }

    public class FailureMessage
    {
        public FailureMessage(Exception ex, IActorRef actor)
        {
            Cause = ex;
            Child = actor;
        }

        public Exception Cause { get; private set; }
        public IActorRef Child { get; private set; }
    }
    public class CompletedFile
    {
        public CompletedFile(string fileName, long elapsedMilliseconds, string author)
        {
            ElapsedMilliseconds = elapsedMilliseconds;
            FileName = fileName;
            Author = author;
        }
        public string FileName { get; private set; }
        public long ElapsedMilliseconds { get; private set; }

        public string Author { get; private set; }
    }

    public class ReportedFile : CompletedFile
    {
        public ReportedFile(string fileName, long elapsedMilliseconds, string author, int fileNumber) : 
            base(fileName, elapsedMilliseconds, author)
        {
            FileNumber = fileNumber;
        }

        public int FileNumber { get; private set; }
    }

    public class StatusMessage
    {
        public StatusMessage(string message)
        {
            Message = message;
        }
        public string Message { get; private set; }
    }
    public class Done
    {
        /// <summary>
        /// Initializes a new instance of the Done class.
        /// </summary>
        public Done(int count, TimeSpan elapsedMs)
        {
            ElapsedTime = elapsedMs;
            Count = count;
        }
        public int Count { get; private set; }
        public TimeSpan ElapsedTime { get; private set; }
    }

    public class DoneEnumeratingFiles
    {
        /// <summary>
        /// Initializes a new instance of the DoneEnumeratingFiles class.
        /// </summary>
        public DoneEnumeratingFiles(int count, TimeSpan elapsedMs)
        {
            ElapsedTime = elapsedMs;
            Count = count;
        }
        public int Count
        { get; private set; }
        public TimeSpan ElapsedTime
        { get; private set; }
    }

    public class ProcessDocumentMessage
    {
        public ProcessDocumentMessage(string filePath, string textToSearch)
        {
            FilePath = filePath;
            TextToSearch = textToSearch;
        }

        public string FilePath { get; private set; }
        public string TextToSearch { get; private set; }
        
    }

    public class DocumentResultMessage
    {
        public DocumentResultMessage(bool textFound, string authorName)
        {
            TextFound = textFound;
            Author = authorName;
        }
        public bool TextFound { get; private set; }
        public string Author { get; private set; }
    }
}
