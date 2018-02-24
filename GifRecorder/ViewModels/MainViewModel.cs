using GifRec = GifRecorder.Services.GifRecorder;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Windows.Forms;
using System.Windows;
using GifRecorder.Services;

namespace GifRecorder.ViewModels
{
    public class MainViewModel
    {
        public int AX;
        public int AY;
        public int BX;
        public int BY;

        private GifRec gifRecorder;

        public string FilePath { get; private set; }

        public async Task<bool> StartRecorder(int seconds, string fileName, Action<int> action, int timeInterval, PresentationSource source)
        {
            FilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + fileName + ".gif";
            if (File.Exists(FilePath))
            {
                if (System.Windows.Forms.MessageBox.Show($"Eine Datei mit dem Namen {fileName}.gif existiert bereits. \nSoll diese ersetzt werden?", "Datei existiert bereits", MessageBoxButtons.YesNo) == DialogResult.No)
                    return false;
            }
            var dpi = DpiGetter.GetDpi(source);
            AX = (int)(AX * 96.0 / dpi.DpiX);
            AY = (int)(AY * 96.0 / dpi.DpiY);
            BX = (int)(BX * 96.0 / dpi.DpiX);
            BY = (int)(BY * 96.0 / dpi.DpiY);
            var stream = new FileStream(FilePath, FileMode.Create);
            gifRecorder = new GifRec(stream, action);
            await gifRecorder.Start(seconds, AX, AY, BX, BY, timeInterval);
            return true;
        }

        public bool ToggleRecorder(int seconds, string fileName, Action<int> action, int timeInterval, PresentationSource source)
        {
            if (gifRecorder != null && gifRecorder.IsRunning)
            {
                Cancel();
                return false;
            }
            else
            {
                StartRecorder(seconds, fileName, action, timeInterval, source);
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
