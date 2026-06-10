using System;
using System.IO;

namespace PRG281_Project
{
    // this class monitors a directory for changes to a specific file
    public sealed class DirectoryMonitor : IDisposable
    {
        private readonly FileSystemWatcher _watcher;
        private readonly string _targetFile;
        private readonly Action _onChange;

        public DirectoryMonitor(string directory, string targetFileName, Action onChange)
        {
            // Validate inputs
            _targetFile = targetFileName ?? throw new ArgumentNullException(nameof(targetFileName));
            _onChange = onChange ?? throw new ArgumentNullException(nameof(onChange));

            
            _watcher = new FileSystemWatcher(directory)
            {
                EnableRaisingEvents = true,
                IncludeSubdirectories = false,
                NotifyFilter = NotifyFilters.FileName | NotifyFilters.LastWrite
            };// Set up the watcher to monitor the specified file

            //subscribe to events
            _watcher.Created += Handle;
            _watcher.Changed += Handle;
            _watcher.Renamed += Handle;
        }

        private void Handle(object sender, FileSystemEventArgs e)
        {
            // Check if the event is for the target file
            if (!e.Name.Equals(_targetFile, StringComparison.OrdinalIgnoreCase)) return;

            
            System.Threading.Thread.Sleep(150);
            _onChange();
        }

        public void Dispose()
        {
            // Unsubscribe from events and dispose of the watcher
            _watcher.EnableRaisingEvents = false;
            _watcher.Created -= Handle;
            _watcher.Changed -= Handle;
            _watcher.Renamed -= Handle;
            _watcher.Dispose();
        }
    }
}
