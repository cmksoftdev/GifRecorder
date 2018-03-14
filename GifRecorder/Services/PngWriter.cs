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
        private int x { get; }
        private int y { get; }
        #endregion

        public PngWriter(Stream OutStream, int x, int y, int DefaultFrameDelay = 500, int Repeat = 0)
        {
            if (OutStream == null)
                throw new ArgumentNullException(nameof(OutStream));

            if (DefaultFrameDelay <= 0)
                throw new ArgumentOutOfRangeException(nameof(DefaultFrameDelay));

            if (Repeat < 0)
                throw new ArgumentOutOfRangeException(nameof(Repeat));

            _writer = new BinaryWriter(OutStream);
            this.DefaultFrameDelay = DefaultFrameDelay;
            this.Repeat = Repeat;
            FrameCount = 0;
            this.x = x;
            this.y = y;
        }

        private void write_Signature()
        {
            Byte[] signature =
            {
                0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A
            };
            write(signature);
        }

        private void write_IHDR(Stream png) // Image Header
        {
            Byte[] ihdr = find_IDAT(png);
            if (ihdr != null)
            {
                write(ihdr);
            }
        }

        private void write_acTL() // Animation Control Chunk
        {
            FrameCountPosition = _writer.BaseStream.Position;
            _writer.Write(0); // Number of frames
            _writer.Write(Repeat); // Number of times to loop this APNG.  0 indicates infinite looping.
        }

        private void write_fcTL() // Frame Control Chunk
        {
            _writer.Write(FrameCount); // Sequence number of the animation chunk, starting from 0
            _writer.Write(x); // Width of the following frame
            _writer.Write(y); // Height of the following frame
            _writer.Write(0); // X position at which to render the following frame
            _writer.Write(0); // Y position at which to render the following frame
            _writer.Write(DefaultFrameDelay); // Frame delay fraction numerator
            _writer.Write(0); // Frame delay fraction denominator
            _writer.Write(0); // Type of frame area disposal to be done after rendering this frame
            _writer.Write(0); // Type of frame area rendering for this frame
            FrameCount++;
        }

        private void write_IEND()
        {
            Byte[] iend =
            {
                0x00, 0x00, 0x00, 0x00, 0x49, 0x45, 0x4E, 0x44, 0xAE, 0x42, 0x60, 0x82
            };
            write(iend);
        }

        private void write_IDAT(Stream png)
        {
            Byte[] idat = find_IDAT(png);
            if (idat != null)
            {
                write(idat);
            }
        }

        private void write_fdAT(Stream png)
        {
        }

        private Byte[] find_IHDR(Stream png)
        {
            return find(png, "IHDR".ToCharArray());
        }

        private Byte[] find_IDAT(Stream png)
        {
            return find(png, "IDAT".ToCharArray());
        }

        private Byte[] find(Stream png, Char[] search)
        {
            Char[] result = null;
            using (var reader = new StreamReader(png))
            {
                Char[] bytes = null;
                int i = 0;
                while (bytes != search && i < png.Length - 4)
                {
                    reader.ReadBlock(bytes, i, search.Length);
                    i++;
                    if(bytes == search)
                    {
                        Char[] rawLength = null;
                        reader.ReadBlock(rawLength, i - 4, 4);
                        int length = BitConverter.ToInt32(rawLength.Select(c => (byte)c).ToArray(), 0);
                        reader.ReadBlock(result, i - 4, length + 16);
                    }
                }                
            }
            return result?.Select(c => (byte)c).ToArray();
        }

        private void write(Byte[] data)
        {
            _writer.Write(data);
        }

        private void writeFrameCount()
        {
            _writer.Seek((int)FrameCountPosition, SeekOrigin.Begin);
            _writer.Write(FrameCount);
        }

        public void Dispose()
        {
            write_IEND();
            writeFrameCount();
        }
    }
}
