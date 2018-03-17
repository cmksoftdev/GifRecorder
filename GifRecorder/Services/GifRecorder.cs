using System;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Imaging;

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
            await this.captureScreenSequenceApng(seconds, ax, ay, bx, by, timeInterval);
        }

        private async Task captureScreenSequence(int seconds, int ax, int ay, int bx, int by, int timeInterval)
        {
            using (var gifWriter = new GifWriter(this.stream, timeInterval, -1))
            {
                Cancel = false;
                IsRunning = true;
                var imageCount = seconds * 1000 / timeInterval;
                var time = 0L;
                this.stepAction.Invoke(3);
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

        private async Task captureScreenSequenceApng(int seconds, int ax, int ay, int bx, int by, int timeInterval)
        {
            using (var pngWriter = new PngWriter(this.stream, bx, by, timeInterval, 0))
            {
                var imageChangeAnalyser = new ImageChangeAnalyser();
                Cancel = false;
                IsRunning = true;
                var imageCount = seconds * 1000 / timeInterval;
                var time = 0L;
                this.stepAction.Invoke(3);
                for (int i = 0; i < imageCount; i++)
                {
                    if (Cancel)
                        break;
                    time = timeInterval - time < 0 ? 0 : timeInterval - time;
                    await Task.Delay((int)(time)).ContinueWith((t) =>
                    {
                        stepAction.Invoke(time == 0 ? 0 : 1);
                        var time2 = DateTime.Now.Ticks / 10000;
                        var image = ScreenShotCreator.CaptureScreen(true, ax, ay, bx, by);
                        if (false)//for later use
                        {
                            var changes = imageChangeAnalyser.GetChanges(image);
                            if (changes.SizeX == 0 || changes.SizeY == 0)
                            {
                                changes.SizeX = 2;
                                changes.SizeY = 2;
                                changes.OffsetX = changes.OffsetX > 4 ? changes.OffsetX - 2 : 2;
                                changes.OffsetY = changes.OffsetY > 4 ? changes.OffsetY - 2 : 2;
                            }
                            var newImage = imageChangeAnalyser.GetPartialImage(image, changes);
                            pngWriter.WriteFrame(newImage, changes.OffsetX, changes.OffsetY);
                        }
                        else
                        {
                            pngWriter.WriteFrame(image);
                        }

                        time = DateTime.Now.Ticks / 10000 - time2;
                    });
                }
                this.stepAction.Invoke(-1);
                IsRunning = false;
            }
            this.stream.Close();
        }
    }
}
