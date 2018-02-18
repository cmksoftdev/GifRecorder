using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Drawing.Imaging;

namespace GifRecorder.Services
{
    public static class ScreenShotCreator
    {
        [StructLayout(LayoutKind.Sequential)]
        struct CURSORINFO
        {
            public Int32 cbSize;
            public Int32 flags;
            public IntPtr hCursor;
            public POINTAPI ptScreenPos;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct POINTAPI
        {
            public int x;
            public int y;
        }

        [DllImport("user32.dll")]
        static extern bool GetCursorInfo(out CURSORINFO pci);

        [DllImport("user32.dll")]
        static extern bool DrawIcon(IntPtr hDC, int X, int Y, IntPtr hIcon);

        const Int32 CURSOR_SHOWING = 0x00000001;

        public static Bitmap CaptureScreen(bool CaptureMouse)
        {
            return CaptureScreen(CaptureMouse, 0, 0, 0, 0);
        }

        public static Bitmap CaptureScreen(bool CaptureMouse, int aX, int aY, int bX, int bY)
        {
            Bitmap result = new Bitmap(bX, bY, PixelFormat.Format24bppRgb);

            try
            {
                using (Graphics g = Graphics.FromImage(result))
                {
                    
                    g.CopyFromScreen(aX, aY, 0, 0, result.Size, CopyPixelOperation.SourceCopy);

                    if (CaptureMouse)
                    {
                        CURSORINFO pci;
                        pci.cbSize = System.Runtime.InteropServices.Marshal.SizeOf(typeof(CURSORINFO));

                        if (GetCursorInfo(out pci))
                        {
                            if (pci.flags == CURSOR_SHOWING)
                            {
                                DrawIcon(g.GetHdc(), pci.ptScreenPos.x - aX, pci.ptScreenPos.y - aY, pci.hCursor);
                                g.ReleaseHdc();
                            }
                        }
                    }
                }
            }
            catch
            {
                result = null;
            }

            return result;
        }
    }
}
