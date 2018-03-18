using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifRecorder.Services
{
    public class ImageStore : IDisposable
    {
        private string tempPath;
        private const string FILE_ENDING = ".tmp";
        public int ImageCount { get; private set; }

        public ImageStore()
        {
            tempPath = Path.GetTempPath();
            tempPath += "apng_" + DateTime.Now.ToFileTimeUtc() + "_";
            ImageCount = 0;
        }

        public void SetImage(Image image)
        {
            var filePathName = tempPath + ImageCount + FILE_ENDING;
            image.Save(filePathName);
            ImageCount++;
        }

        public Image GetImage(int id)
        {
            if (id < 0 || id > ImageCount)
                return null;
            var filePathName = tempPath + id + FILE_ENDING;
            Image imageClone = null;
            using (FileStream myStream = new FileStream(filePathName, FileMode.Open))
            {
                imageClone = Image.FromStream(myStream);
            }
            return imageClone;
        }

        private void deleteAllImages()
        {
            for (int i = 0; i<ImageCount;i++)
            {
                var filePathName = tempPath + i + FILE_ENDING;
                File.Delete(filePathName);
            }
        }

        public void Dispose()
        {
            deleteAllImages();
        }
    }
}
