using GifRecorder.Models;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifRecorder.Services
{
    public class ImageChangeAnalyser
    {
        private Bitmap oldImage = null;

        public SizeOffset GetChanges(Image newImage)
        {
            var newImageBmp = new Bitmap(newImage);
            var x = newImage.Width;
            var y = newImage.Height;
            var result = new SizeOffset()
            {
                OffsetX = x,
                OffsetY = y,
                SizeX = 0,
                SizeY = 0
            };
            var isChanged = false;
            if (oldImage != null)
            {
                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        Color a = oldImage.GetPixel(i, j);
                        Color b = newImageBmp.GetPixel(i, j);
                        if (!isColorEqual(a, b))
                        {
                            isChanged = true;
                            result.OffsetX = i > result.OffsetX ? i : result.OffsetX;
                            result.OffsetY = j > result.OffsetY ? j : result.OffsetY;
                            result.SizeX = i < result.SizeX ? i : result.SizeX;
                            result.SizeY = j < result.SizeY ? j : result.SizeY;
                        }
                    }
                }
            }

            oldImage = newImageBmp;
            return isChanged ? result : new SizeOffset()
            {
                OffsetX = 0,
                OffsetY = 0,
                SizeX = x,
                SizeY = y
            };
        }
        private bool isB = false;
        public Image BlackoutImage(Image newImage, int step)
        {
            int s = 0;// isB ? 1 : 0;
            isB = !isB;
            var newImageBmp = new Bitmap(newImage);
            var x = newImage.Width;
            var y = newImage.Height;
            var newImage2 = new Bitmap(newImageBmp);
            if (oldImage != null)
            {
                for (int i = 0; i < x - 4; i += 4)
                {
                    for (int j = 0; j < y - 4; j += 4)
                    {
                        Color a = oldImage.GetPixel(i + s, j + s);
                        Color b = newImageBmp.GetPixel(i + s, j + s);
                        if (isColorEqual(a, b))
                        {
                            set_transparent(newImageBmp, i, j);
                        }
                    }
                }
            }
            oldImage = newImage2;
            return newImageBmp;
        }

        private void set_transparent(Bitmap bmp, int x, int y)
        {
            for (int i = 0; i < 4; i++)
            {
                for (int j = 0; j < 4; j++)
                {
                    bmp.SetPixel(i + x, j + y, Color.Transparent);
                }
            }
        }

        public Image BlackoutImage(Image newImage, out bool equal)
        {
            equal = false;
            if (newImage == null)
                return null;
            var newImageBmp = new Bitmap(newImage);
            var x = newImage.Width;
            var y = newImage.Height;
            var pixelCount = x * y;
            var changeCount = 0;
            var newImage2 = new Bitmap(newImageBmp);
            if (oldImage != null)
            {
                for (int i = 0; i < x; i++)
                {
                    for (int j = 0; j < y; j++)
                    {
                        Color a = oldImage.GetPixel(i, j);
                        Color b = newImageBmp.GetPixel(i, j);
                        if (isColorEqual(a, b))
                        {
                            changeCount++;
                            newImageBmp.SetPixel(i, j, Color.Transparent);
                        }
                    }
                }
            }
            oldImage = newImage2;
            equal = changeCount == pixelCount;
            return newImageBmp;
        }

        public Image GetPartialImage(Image image, SizeOffset sizeOffset)
        {
            Bitmap original = new Bitmap(image);
            Rectangle srcRect = new Rectangle(
                sizeOffset.OffsetX,
                sizeOffset.OffsetY,
                sizeOffset.SizeX,
                sizeOffset.SizeY);
            return sizeOffset.SizeX == 0 || sizeOffset.SizeY == 0 ?
                null : original.Clone(srcRect, original.PixelFormat);
        }

        private bool isColorEqual(Color a, Color b)
        {
            return
                a.R == b.R &&
                a.G == b.G &&
                a.B == b.B;
        }
    }
}
