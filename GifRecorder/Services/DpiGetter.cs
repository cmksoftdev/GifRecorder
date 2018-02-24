using GifRecorder.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace GifRecorder.Services
{
    public class DpiGetter
    {
        public static DpiContainer GetDpi(PresentationSource source)
        {
            return new DpiContainer
            {
                DpiX = 96.0 * source.CompositionTarget.TransformToDevice.M11,
                DpiY = 96.0 * source.CompositionTarget.TransformToDevice.M22
            };
        }
    }
}
