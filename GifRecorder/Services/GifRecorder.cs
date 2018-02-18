using System;
using System.Threading.Tasks;
using System.IO;

namespace GifRecorder.Services
{
    public class GifRecorder
    {
        private Stream stream;
        private readonly Action<int> stepAction;

        public bool Cancel { get; set; }
        public bool IsRunning { get; private set; }

        public GifRecorder(Stream stream, Action<int> action)
        {
            this.stream = stream;
            this.stepAction = action;
        }

        public async Task Start(int seconds, int ax, int ay, int bx, int by, int timeInterval)
        {
            await this.captureScreenSequence(seconds, ax, ay, bx, by, timeInterval);
        }

        private async Task captureScreenSequence(int seconds, int ax, int ay, int bx, int by, int timeInterval)
        {
            using (var gifWriter = new GifWriter(this.stream, timeInterval, -1))
            {
                Cancel = false;
                IsRunning = true;
                var imageCount = seconds * 1000 / timeInterval;
                var time = 0L;
                for (int i = 0; i < imageCount; i++)
                {
                    if (Cancel)
                        break;
                    time = timeInterval - time < 0 ? 0 : timeInterval - time;
                    await Task.Delay((int)(time)).ContinueWith( (t) => 
                    {
                        stepAction.Invoke(time==0?0:1);
                        var time2 = DateTime.Now.Ticks/10000;
                        var image = ScreenShotCreator.CaptureScreen(true, ax, ay, bx, by);
                        gifWriter.WriteFrame(image);                        
                        time = DateTime.Now.Ticks / 10000 - time2;
                    });
                }
                this.stepAction.Invoke(-1);
                IsRunning = false;
            }
        }
    }
}
