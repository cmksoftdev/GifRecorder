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

        public bool O1 { get; set; }
        public bool O2 { get; set; }
        public bool O3 { get; set; }

        public bool O4 { get; set; }
        public bool O5 { get; set; }
        public bool O6 { get; set; }

        public async Task<bool> StartRecorder(int seconds, string fileName, Action<int> action, int timeInterval, PresentationSource source, int option, int format)
        {
            fileName += format == 1 ? ".png" : ".gif";
            FilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + fileName;
            if (File.Exists(FilePath))
            {
                if (System.Windows.Forms.MessageBox.Show($"Eine Datei mit dem Namen {fileName} existiert bereits. \nSoll diese ersetzt werden?", "Datei existiert bereits", MessageBoxButtons.YesNo) == DialogResult.No)
                    return false;
            }
            var dpi = DpiGetter.GetDpi(source);
            AX = (int)(AX * 96.0 / dpi.DpiX);
            AY = (int)(AY * 96.0 / dpi.DpiY);
            BX = (int)(BX * 96.0 / dpi.DpiX);
            BY = (int)(BY * 96.0 / dpi.DpiY);
            var stream = new FileStream(FilePath, FileMode.Create);
            gifRecorder = new GifRec(stream, action);
            int option2 = !O4 ? (!O5 ? 3 : 2) : 1;
            await gifRecorder.Start(seconds, AX, AY, BX, BY, timeInterval, option, option2, format);
            return true;
        }

        public bool ToggleRecorder(int seconds, string fileName, Action<int> action, int timeInterval, PresentationSource source, int format)
        {
            if (gifRecorder != null && gifRecorder.IsRunning)
            {
                Cancel();
                return false;
            }
            else
            {
                int option = !O1 ? (!O2 ? 3 : 2) : 1;
                StartRecorder(seconds, fileName, action, timeInterval, source, option, format);
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
