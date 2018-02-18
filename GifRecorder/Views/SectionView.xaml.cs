using GifRecorder.ViewModels;
using System.Runtime.InteropServices;
using System.Windows;

namespace GifRecorder.Views
{
    /// <summary>
    /// Interaktionslogik für SectionView.xaml
    /// </summary>
    public partial class SectionView : Window
    {
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool GetCursorPos(out PointAPI lpPoint);

        private struct PointAPI
        {
            public int X;
            public int Y;
        }
        public SectionView()
        {
            InitializeComponent();
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            this.DragMove();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            ((MainViewModel)this.DataContext).AX = (int)this.Left;
            ((MainViewModel)this.DataContext).AY = (int)this.Top;
            ((MainViewModel)this.DataContext).BX = (int)this.Width;
            ((MainViewModel)this.DataContext).BY = (int)this.Height;
            this.Close();
        }
    }
}
