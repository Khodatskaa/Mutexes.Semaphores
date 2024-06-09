using System;
using System.Threading;
using System.Windows;

namespace Mutexes.Semaphores
{
    public partial class MainWindow : Window
    {
        private Semaphore semaphore;

        public MainWindow()
        {
            InitializeComponent();

            semaphore = new Semaphore(3, 3);

            if (!semaphore.WaitOne(0))
            {
                MessageBox.Show("Only three instances of this module can run simultaneously.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                Close();
            }
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            semaphore.Release();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show("Button clicked!", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }
}
