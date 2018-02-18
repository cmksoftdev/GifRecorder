using GifRecorder.ViewModels;
using GifRecorder.Views;
using System.Windows;
using System.Windows.Media;

namespace GifRecorder
{
    /// <summary>
    /// Interaktionslogik für MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        bool isRunning = false;
        private int step = 0;
        public MainWindow()
        {
            this.DataContext = new MainViewModel();
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            if (isRunning)
                return;
            int seconds;
            if (int.TryParse(this.Seconds.Text, out seconds))
            {
                this.isRunning = true;
                this.Status.Content = "Aufnahme läuft";
                ((MainViewModel)(this.DataContext)).StartRecorder(seconds, this.FileName.Text, SetText);
                this.isRunning = false;
            }
        }

        private void SetText(int status)
        {
            if (step < 3)
                step++;
            else
                step = 0;
            switch (status)
            {
                case 0:
                    this.Dispatcher.Invoke(() => { this.Status.Foreground = Brushes.Red; });
                    break;
                case 1:
                    this.Dispatcher.Invoke(() => { this.Status.Foreground = Brushes.Green; });
                    break;
                case 2:
                    this.Dispatcher.Invoke(() => { this.Status.Foreground = Brushes.Black; });
                    break;
                default:
                    this.Dispatcher.Invoke(() => { this.Status.Foreground = Brushes.Red; });
                    break;
            }

            switch (step)
            {
                case 0:
                    this.Dispatcher.Invoke(()=> { this.Status.Content = "-"; });
                    break;
                case 1:
                    this.Dispatcher.Invoke(() => { this.Status.Content = "\\"; });
                    break;
                case 2:
                    this.Dispatcher.Invoke(() => { this.Status.Content = "|"; });
                    break;
                case 3:
                    this.Dispatcher.Invoke(() => { this.Status.Content = "/"; });
                    break;
                default:
                    this.Dispatcher.Invoke(() => { this.Status.Content = "*"; });
                    break;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var section = new SectionView();
            section.DataContext = this.DataContext;
            section.ShowDialog();
        }
    }
}
