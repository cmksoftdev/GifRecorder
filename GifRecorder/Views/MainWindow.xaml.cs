using GifRecorder.ViewModels;
using GifRecorder.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

        private void SetText()
        {
            if (step < 3)
                step++;
            else
                step = 0;
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
