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
        private int timeInterval = 150;
        private readonly Action<int> stepAction;

        public GifRecorder(Stream stream, Action<int> action)
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
                var time = 0L;
                for (int i = 0; i < imageCount; i++)
                {
                    time = this.timeInterval - time < 0 ? 0 : this.timeInterval - time;
                    await Task.Delay((int)(time)).ContinueWith( (t) => 
                    {
                        stepAction.Invoke(time==0?0:1);
                        var time2 = DateTime.Now.Ticks/10000;
                        var image = ScreenShotCreator.CaptureScreen(true, ax, ay, bx, by);
                        gifWriter.WriteFrame(image);                        
                        time = DateTime.Now.Ticks / 10000 - time2;
                    });
                }
            }
        }
    }
}
