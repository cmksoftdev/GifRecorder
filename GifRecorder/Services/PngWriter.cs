using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifRecorder.Services
{
    public class PngWriter : IDisposable
    {
        #region Fields
        readonly BinaryWriter _writer;
        #endregion

        #region Props
        public int DefaultFrameDelay { get; set; }
        public int Repeat { get; }

        private long FrameCountPosition { get; set; }
        private long FrameCount { get; set; }
        #endregion

        public PngWriter(Stream OutStream, int DefaultFrameDelay = 500, int Repeat = -1)
        {
            if (OutStream == null)
                throw new ArgumentNullException(nameof(OutStream));

            if (DefaultFrameDelay <= 0)
                throw new ArgumentOutOfRangeException(nameof(DefaultFrameDelay));

            if (Repeat < -1)
                throw new ArgumentOutOfRangeException(nameof(Repeat));

            _writer = new BinaryWriter(OutStream);
            this.DefaultFrameDelay = DefaultFrameDelay;
            this.Repeat = Repeat;
            FrameCount = 0;
        }

        private void write_Signature()
        {
            Byte[] signature =
            {
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A
            };
            write(signature);
        }

        private void write_IHDR() // Image Header
        {
            _writer.Write("dfa".ToCharArray());
        }

        private void write_acTL() // Animation Control Chunk
        {
            FrameCountPosition = _writer.BaseStream.Position;
            _writer.Write(0);
            _writer.Write(Repeat);
        }

        private void write_fcTL() // Frame Control Chunk
        {

        }

        private void write_IEND()
        {
            Byte[] iend =
            {
                0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
            };
            write(iend);
        }

        private void write(Byte[] data)
        {
            _writer.Write(data);
        }

        public void Dispose()
        {
            write_IEND();
        }
    }
}
