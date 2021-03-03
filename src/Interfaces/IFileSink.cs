using System;

namespace Lignator.Interfaces
{
    public interface IFileSink: IDisposable
    {
        IFileSink Start(string path, bool multiline = false);

        void Sink(string content);

        void DeleteFile(string path);
    }
}