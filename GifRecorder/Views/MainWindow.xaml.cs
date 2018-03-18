using GifRecorder.ViewModels;
using GifRecorder.Views;
using System.IO;
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
            int fps;
            if (int.TryParse(this.Seconds.Text, out seconds) && int.TryParse(this.FPS.Text, out fps))
            {
                var format = this.Png.IsChecked == true ? 1 : 0;
                this.isRunning = true;
                ((MainViewModel)(this.DataContext)).ToggleRecorderAsync(seconds, this.FileName.Text, SetText, 1000 / fps, PresentationSource.FromVisual(this), format);
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
                case 3:
                    this.Dispatcher.Invoke(() =>
                    {
                        this.Status.Foreground = Brushes.Black;
                        this.StartStopButton.Content = "Stop";
                    });
                    break;
                case 4:
                    this.Dispatcher.Invoke(() =>
                    {
                        this.Status.Foreground = Brushes.Black;
                        this.Status.Content = "Bilder werden verarbeitet";
                    });
                    return;
                default:
                    this.Dispatcher.Invoke(() => 
                    {
                        this.Status.Foreground = Brushes.Black;
                        this.AusgabedateiAnzeigen.IsEnabled = true;
                        this.Status.Content = "Datei unter MyDocuments gespeichert";
                        this.StartStopButton.Content = "Start";
                    });
                    return;
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
                    this.Dispatcher.Invoke(() => { this.Status.Content = "Aufnahme beendet"; });
                    break;
            }
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            var section = new SectionView();
            section.DataContext = this.DataContext;
            section.ShowDialog();
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            string filePath = ((MainViewModel)this.DataContext).FilePath;
            if (!File.Exists(filePath))
            {
                return;
            }
            string argument = "/select, \"" + filePath + "\"";
            System.Diagnostics.Process.Start("explorer.exe", argument);
        }

        private void Button_Click3(object sender, RoutedEventArgs e)
        {
            this.MenuGrid.ColumnDefinitions[1].Width = this.MenuGrid.ColumnDefinitions[1].Width == GridLength.Auto ? new GridLength(0) : GridLength.Auto;
            this.Width = this.Width == 430 ? 230 : 430;
        }
    }
}
