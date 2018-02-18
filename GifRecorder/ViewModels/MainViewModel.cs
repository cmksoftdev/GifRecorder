using GifRec = GifRecorder.Services.GifRecorder;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public async Task<bool> StartRecorder(int seconds, string fileName, Action action)
        {
            var filePath = Directory.GetCurrentDirectory() + "\\" + fileName + ".gif";
            var stream = new FileStream(filePath, FileMode.CreateNew);
            var gifRecorder = new GifRec(stream, action);
            await gifRecorder.Start(seconds, AX, AY, BX, BY);
            return true;
        }
    }
}
