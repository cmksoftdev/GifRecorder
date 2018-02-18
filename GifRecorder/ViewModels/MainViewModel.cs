using GifRec = GifRecorder.Services.GifRecorder;
using System.Threading.Tasks;
using System.IO;
using System;

namespace GifRecorder.ViewModels
{
    public class MainViewModel
    {
        public int AX;
        public int AY;
        public int BX;
        public int BY;

        private GifRec gifRecorder;

        public async Task<bool> StartRecorder(int seconds, string fileName, Action<int> action, int timeInterval)
        {
            var filePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + fileName + ".gif";
            var stream = new FileStream(filePath, FileMode.CreateNew);
            gifRecorder = new GifRec(stream, action);
            await gifRecorder.Start(seconds, AX, AY, BX, BY, timeInterval);
            return true;
        }

        public bool ToggleRecorder(int seconds, string fileName, Action<int> action, int timeInterval)
        {
            if (gifRecorder != null && gifRecorder.IsRunning)
            {
                Cancel();
                return false;
            }
            else
            {
                StartRecorder(seconds, fileName, action, timeInterval);
                return true;
            }
        }

        public void Cancel()
        {
            if (gifRecorder!=null)
                gifRecorder.Cancel = true;
        }
    }
}
