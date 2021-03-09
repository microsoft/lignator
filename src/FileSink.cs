// Copyright (c) Microsoft Corporation.
// Licensed under the MIT License.

using System.Collections.Concurrent;
using System.IO;
using System.Threading.Tasks;
using Lignator.Interfaces;

namespace Lignator
{
    public class FileSink : IFileSink
    {
        private ConcurrentQueue<string> queue;
        private Task sinker;

        private bool disposing = false;
        private bool disposed = false;
        private readonly string path;
        private readonly bool clean;

        public FileSink() {}
        private FileSink(string path, bool multiline = false, bool clean = false)
        {
            this.path = path;
            this.clean = clean;
            string directory = Path.GetDirectoryName(path);
            if (!Directory.Exists(directory))
            {
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
                    await writer.FlushAsync();
                }
            });
        }

        public IFileSink Start(string path, bool multiline = false, bool clean = false)
        {
            return new FileSink(path, multiline, clean);
        }

        public void Sink(string content)
        {
            if (this.disposing) return;
            this.queue.Enqueue(content);
        }

        public void Dispose()
        {
            this.disposing = true;
            while (this.queue.Count > 0) { } // wait until all logs are written;
            this.disposed = true;
            Task.WaitAll(new Task[] { this.sinker });
            this.sinker.Dispose();
            if (this.clean) File.Delete(this.path);
        }
    }
}