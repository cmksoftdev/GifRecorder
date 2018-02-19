using GifRec = GifRecorder.Services.GifRecorder;
using System.Threading.Tasks;
using System.IO;
using System;
using System.Windows.Forms;

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

        public async Task<bool> StartRecorder(int seconds, string fileName, Action<int> action, int timeInterval)
        {
            FilePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\" + fileName + ".gif";
            if (File.Exists(FilePath))
            {
                if (MessageBox.Show($"Eine Datei mit dem Namen {fileName}.gif existiert bereits. \nSoll diese ersetzt werden?", "Datei existiert bereits", MessageBoxButtons.YesNo) == DialogResult.No)
                    return false;
            }
            var stream = new FileStream(FilePath, FileMode.Create);
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
