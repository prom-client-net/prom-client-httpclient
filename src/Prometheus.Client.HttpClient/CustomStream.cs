using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Prometheus.Client.HttpClient
{
    internal class CustomStream : Stream
    {
        private readonly Stream _innerStream;
        private readonly Action _readCallback;
        public CustomStream(Stream innerStream, Action readCallback)
        {
            _innerStream = innerStream ?? throw new ArgumentNullException(nameof(innerStream));
            _readCallback = readCallback ?? throw new ArgumentNullException(nameof(readCallback));
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            int bytesRead = _innerStream.Read(buffer, offset, count);

            if (bytesRead == 0)
            {
                _readCallback();
            }

            return bytesRead;
        }

        public override void Close()
        {
            _innerStream.Close();
            base.Close();
        }

        public override bool CanRead => _innerStream.CanRead;

        public override bool CanSeek => _innerStream.CanSeek;

        public override bool CanWrite => _innerStream.CanWrite;

        public override long Length => _innerStream.Length;

        public override long Position { get => _innerStream.Position; set => _innerStream.Position = value; }
        public override void Flush() => _innerStream.Flush();

        public override long Seek(long offset, SeekOrigin origin) => _innerStream.Seek(offset, origin);

        public override void SetLength(long value) => _innerStream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count) => _innerStream.Write(buffer, offset, count);
    }
}
