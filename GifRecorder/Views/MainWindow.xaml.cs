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
                Task.Delay(10);
                ((MainViewModel)(this.DataContext)).StartRecorder(seconds, this.FileName.Text).Wait();
                this.isRunning = false;
                this.Status.Content = "Startbereit";
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
