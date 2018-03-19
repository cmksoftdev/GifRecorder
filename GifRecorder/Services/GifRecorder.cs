using System;
using System.Threading.Tasks;
using System.IO;
using System.Drawing.Imaging;
using System.Drawing;
using System.Collections.Generic;

namespace GifRecorder.Services
{
    public class GifRecorder
    {
        private Stream stream;
        private readonly Action<int> stepAction;
        private readonly Action<float> progressAction;

        public bool Cancel { get; set; }
        public bool IsRunning { get; private set; }

        public GifRecorder(Stream stream, Action<int> action, Action<float> action2)
        {
            this.stream = stream;
            this.stepAction = action;
            this.progressAction = action2;
        }

        public async Task Start(int seconds, int ax, int ay, int bx, int by, int timeInterval,int option, int option2, int format = 1)
        {
            if (format==1)
                await this.captureScreenSequenceApng(seconds, ax, ay, bx, by, timeInterval, option, option2);
            else
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

        private async Task captureScreenSequenceApng(int seconds, int ax, int ay, int bx, int by, int timeInterval, int option, int option2)
        {
            using (var pngWriter = new PngWriter(this.stream, bx, by, timeInterval, 0))
            {
                var imageChangeAnalyser = new ImageChangeAnalyser();
                Cancel = false;
                IsRunning = true;
                var imageCount = seconds * 1000 / timeInterval;
                var time = 0L;
                this.stepAction.Invoke(3);
                List<Image> imageList = new List<Image>();
                using (var imageStore = new ImageStore())
                {
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
                            if (option2==2)
                            {
                                imageList.Add(image);
                            }
                            else if (option2 == 3)
                            {
                                imageStore.SetImage(image);
                            }
                            else
                            {
                                if (option == 0)//for later use
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
                                    pngWriter.WriteFrame(newImage, (short)timeInterval, changes.OffsetX, changes.OffsetY);
                                }
                                else if (option == 1)
                                {
                                    pngWriter.WriteFrame(image, (short)timeInterval);
                                }
                                else if (option == 2)
                                {
                                    bool e;
                                    var newImage = imageChangeAnalyser.BlackoutImage(image, out e);
                                    pngWriter.WriteFrame(newImage, (short)timeInterval);
                                }
                                else
                                {
                                    var newImage = imageChangeAnalyser.BlackoutImage(image, 1);
                                    pngWriter.WriteFrame(newImage, (short)timeInterval);
                                }
                            }
                            time = DateTime.Now.Ticks / 10000 - time2;
                        });
                    }
                    if (option2 == 3)
                    {
                        Action a = new Action(()=> 
                        {

                            this.stepAction.Invoke(4);
                            var count = imageStore.ImageCount;
                            var delay = (short)timeInterval;
                            var duplicat = false;
                            for (int i = 0; i < count; i++)
                            {
                                var percent = (100f/count) * i;
                                progressAction.Invoke(percent);
                                var image = imageStore.GetImage(i);
                                bool equal;
                                var image2 = imageChangeAnalyser.BlackoutImage(image, out equal);
                                if(equal&&i!=count-1)
                                {
                                    duplicat = true;
                                    delay += (short)timeInterval;
                                }
                                else
                                {
                                    if (duplicat)
                                    {
                                        duplicat = false;
                                        pngWriter.WriteFrame(image2, delay);
                                    }
                                    else
                                        pngWriter.WriteFrame(image2, delay);
                                    delay = (short)timeInterval;
                                }
                            }
                            progressAction.Invoke(-1f);
                        });
                        await Task.Run(a);
                    }
                    else if (option2==2)
                    {
                        foreach (var image in imageList)
                        {
                            bool equal;
                            var image2 = imageChangeAnalyser.BlackoutImage(image, out equal);
                            pngWriter.WriteFrame(image2, (short)timeInterval);
                        }
                    }
                } 
                this.stepAction.Invoke(-1);
                IsRunning = false;
            }
            this.stream.Close();
        }
    }
}
