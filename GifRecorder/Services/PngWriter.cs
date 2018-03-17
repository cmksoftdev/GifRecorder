using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace GifRecorder.Services
{
    public class PngWriter : IDisposable
    {
        #region Fields
        readonly BinaryWriter _writer;
        Stream OutStream;
        #endregion

        #region Props
        public int DefaultFrameDelay { get; set; }
        public int Repeat { get; }

        private long FrameCountPosition { get; set; }
        private long FrameCount { get; set; }
        private int ChunkSequenceNumber { get; set; }
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
            ChunkSequenceNumber = 0;
            this.x = x;
            this.y = y;
            this.OutStream = OutStream;
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
            Byte[] ihdr = find_IHDR(png);
            if (ihdr != null)
            {
                write(ihdr);
            }
        }

        private void write_acTL_placeholder() // Animation Control Chunk
        {
            FrameCountPosition = _writer.BaseStream.Position;
            _writer.Write((byte)0);
            _writer.Write((byte)0);
            _writer.Write((byte)0);
            _writer.Write((byte)8);
            _writer.Write("acTL".ToCharArray());
            _writer.Write(0); // Number of frames
            _writer.Write((byte)0);
            _writer.Write((byte)0);
            _writer.Write((byte)0);
            _writer.Write((byte)Repeat); // Number of times to loop this APNG.  0 indicates infinite looping.
            _writer.Write(0);
        }

        private void write_fcTL() // Frame Control Chunk
        {
            List<Byte> chunk = new List<byte>();
            chunk.AddRange(new Byte[]{ 0, 0, 0, 26 });
            chunk.AddRange("fcTL".ToCharArray().Select(c => (byte)c).ToArray());
            Byte[] _ChunkSequenceNumber = BitConverter.GetBytes(ChunkSequenceNumber);
            Array.Reverse(_ChunkSequenceNumber);
            chunk.AddRange(_ChunkSequenceNumber);
            Byte[] _x = BitConverter.GetBytes(x);
            Array.Reverse(_x);
            chunk.AddRange(_x);
            Byte[] _y = BitConverter.GetBytes(y);
            Array.Reverse(_y);
            chunk.AddRange(_y);
            chunk.AddRange(new Byte[] { 0, 0, 0, 0 });
            chunk.AddRange(new Byte[] { 0, 0, 0, 0 });
            Byte[] _DefaultFrameDelay = BitConverter.GetBytes((short)DefaultFrameDelay);
            Array.Reverse(_DefaultFrameDelay);
            Byte[] _FrameCount2 = BitConverter.GetBytes((short)FrameCount);
            Array.Reverse(_FrameCount2);
            chunk.AddRange(_DefaultFrameDelay);
            chunk.AddRange(new Byte[] { 3, 232 });
            chunk.AddRange(new Byte[] { 2, 0});
            CrcCalculator crc32 = new CrcCalculator();
            var crc = crc32.GetCRC32(chunk.Skip(4).ToArray());
            var crcArray = BitConverter.GetBytes(crc);
            Array.Reverse(crcArray);
            _writer.Write(chunk.ToArray());
            _writer.Write(crcArray);
            FrameCount++;
            ChunkSequenceNumber++;
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
            Byte[] idat = find_IDAT(png);
            if (idat != null)
            {
                Byte[] _ChunkSequenceNumber = BitConverter.GetBytes(ChunkSequenceNumber);
                Array.Reverse(_ChunkSequenceNumber);
                var length = idat.Count() - 8;
                var lengthArray = BitConverter.GetBytes(length);
                Array.Reverse(lengthArray);
                Byte[] fdAT = new byte[idat.Count()];
                lengthArray.CopyTo(fdAT, 0);
                var sign = "fdAT".ToCharArray().Select(c => (byte)c).ToArray();
                sign.CopyTo(fdAT, 4);
                _ChunkSequenceNumber.CopyTo(fdAT, 8);
                idat = idat.Take(idat.Count() - 4).ToArray();
                idat = idat.Skip(8).ToArray();
                idat.CopyTo(fdAT, 12);
                var crc32 = new CrcCalculator();
                var data = fdAT.Skip(4).ToArray();
                var crc = crc32.GetCRC32(data);
                var crcArray = BitConverter.GetBytes(crc);
                _writer.Write(fdAT);
                Array.Reverse(crcArray);
                _writer.Write(crcArray);
                ChunkSequenceNumber++;
            }
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
            Byte[] result = null;
            var searchBytes = search.Select(c => (byte)c).ToArray();
            Byte[] bytes = new Byte[search.Length];
            int i = 0;
            while (i < png.Length - 4)
            {
                png.Flush();
                png.Position = i;
                var debug = png.Read(bytes, 0, search.Length);
                i++;
                if (bytes.SequenceEqual(searchBytes))
                {
                    Byte[] rawLength = new Byte[4];
                    png.Position -= 8;
                    png.Read(rawLength, 0, 4);
                    Array.Reverse(rawLength);
                    UInt32 length = BitConverter.ToUInt32(rawLength, 0);
                    result = new Byte[length + 12];
                    png.Position -= 4;
                    png.Read(result, 0, (int)(length + 12));
                    break;
                }
            }
            return result;
        }

        private void write(Byte[] data)
        {
            _writer.Write(data);
        }

        private void write_acTL()
        {
            List<byte> chunk = new List<byte>()
            {
                0,0,0,8
            };
            chunk.AddRange("acTL".ToCharArray().Select(c => (byte)c).ToArray());
            Byte[] _FrameCount = BitConverter.GetBytes((int)FrameCount);
            Array.Reverse(_FrameCount);
            chunk.AddRange(_FrameCount);
            Byte[] _Repeat = BitConverter.GetBytes(Repeat);
            Array.Reverse(_Repeat);
            chunk.AddRange(_Repeat);
            _writer.Seek((int)FrameCountPosition, SeekOrigin.Begin);
            _writer.Write(chunk.ToArray());
            var crc32 = new CrcCalculator();
            var crc = crc32.GetCRC32(chunk.Skip(4).ToArray());
            var crcArray = BitConverter.GetBytes(crc);
            Array.Reverse(crcArray);
            _writer.Write(crcArray);
            FrameCount++;
        }

        public void WriteFrame(Image image)
        {
            using (Stream png = new MemoryStream())
            {
                image.Save(png, ImageFormat.Png);
                if (FrameCount == 0)
                {
                    write_Signature();
                    write_IHDR(png);
                    write_acTL_placeholder();
                }
                write_fcTL();
                if (FrameCount == 1)
                    write_IDAT(png);
                else
                    write_fdAT(png);
            }
        }

        public void Dispose()
        {
            write_IEND();
            write_acTL();
        }
    }
}
