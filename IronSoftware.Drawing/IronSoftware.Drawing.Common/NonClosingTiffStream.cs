using BitMiracle.LibTiff.Classic;
using System;
using System.IO;

namespace IronSoftware.Drawing
{
    internal class NonClosingTiffStream : TiffStream, IDisposable
    {
        private readonly Stream _stream;
        private bool _disposed = false;

        public NonClosingTiffStream(Stream stream)
        {
            _stream = stream ?? throw new ArgumentNullException(nameof(stream));
        }

        public override int Read(object clientData, byte[] buffer, int offset, int count)
        {
            return _stream.Read(buffer, offset, count);
        }

        public override void Write(object clientData, byte[] buffer, int offset, int count)
        {
            _stream.Write(buffer, offset, count);
        }

        public override long Seek(object clientData, long offset, SeekOrigin origin)
        {
            return _stream.Seek(offset, origin);
        }

        public override void Close(object clientData)
        {
            // Suppress automatic closing — manual control only
        }

        public override long Size(object clientData)
        {
            return _stream.Length;
        }

        /// <summary>
        /// Manually closes the underlying stream when you are ready.
        /// </summary>
        public void CloseStream()
        {
            if (!_disposed)
            {
                _stream.Dispose();
                _disposed = true;
            }
        }

        public void Dispose()
        {
            CloseStream();
        }
    }
}