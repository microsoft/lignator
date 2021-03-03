using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Lignator.Interfaces;
using Microsoft.Extensions.Logging;

namespace Lignator
{
    public class FileSink : IFileSink
    {
        private ConcurrentQueue<string> queue;
        private Task sinker;

        private bool disposing = false;
        private bool disposed = false;
        private readonly ILogger<FileSink> logger;

        public FileSink(ILogger<FileSink> logger)
        {
            this.logger = logger;
        }

        public IFileSink Start(string path, bool multiline = false)
        {
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
                this.logger.LogDebug($"Directory did not exist, creating directory {directory}");
                Directory.CreateDirectory(directory);
            }

            this.queue = new ConcurrentQueue<string>();
            this.sinker = Task.Run(async () =>
            {
                using (StreamWriter writer = new StreamWriter(path, true))
                {
                    while (!this.disposed)
                    {
                        string log;
                        if (this.queue.TryDequeue(out log))
                        {
                            Task task = multiline ? writer.WriteAsync(log) : writer.WriteLineAsync(log);
                            await task;
                        }
                    }
                }
            });
            return this;
        }

        public void Sink(string content)
        {
            if (this.disposing) return;
            this.queue.Enqueue(content);
        }

        public void DeleteFile(string path)
        {
            File.Delete(path);
        }

        public void Dispose()
        {
            this.disposing = true;
            while (this.queue.Count > 0) { } // wait until all logs are written;
            this.disposed = true;
            Task.WaitAll(new Task[] { this.sinker });
            this.sinker.Dispose();
        }
    }
}