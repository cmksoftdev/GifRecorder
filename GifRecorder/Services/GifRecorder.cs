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
        private int timeInterval = 50;
        private readonly Action stepAction;

        public GifRecorder(Stream stream, Action action)
        {
            this.stream = stream;
            this.stepAction = action;
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
                var time = DateTime.Now.Ticks;
                for (int i = 0; i < imageCount; i++)
                {
                    var lastTime = time;
                    time = DateTime.Now.Ticks;
                    await Task.Delay(((int)(this.timeInterval - time - lastTime))).ContinueWith( (t) => 
                    {
                        var image = ScreenShotCreator.CaptureScreen(true, ax, ay, bx, by);
                        gifWriter.WriteFrame(image);
                        stepAction.Invoke();
                    });
                }
            }
        }
    }
}
