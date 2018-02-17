using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace GifRecorder.Services
{
    public class GifRecorder
    {
        private Stream stream;
        private int timeInterval = 200;

        public GifRecorder(Stream stream)
        {
            this.stream = stream;
        }

        public async Task Start(int seconds, int ax, int ay, int bx, int by)
        {
            await this.captureScreenSequence(seconds, ax, ay, bx, by);
        }

        private async Task captureScreenSequence(int seconds, int ax, int ay, int bx, int by)
        {
            using (var gifWriter = new GifWriter(this.stream, this.timeInterval, -1))
            {
                var imageCount = seconds * 1000 / this.timeInterval;
                for (int i = 0; i < imageCount; i++)
                {
                    var image = ScreenShotCreator.CaptureScreen(true, ax, ay, bx, by);
                    gifWriter.WriteFrame(image);
                    Task.Delay(this.timeInterval).Wait();
                }
            }
        }
    }
}
