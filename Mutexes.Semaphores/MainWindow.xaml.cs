using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Mutexes.Semaphores
{
    public partial class MainWindow : Window
    {

        private Mutex _mutex = new Mutex();
        private SemaphoreSlim _semaphore = new SemaphoreSlim(3); 
        private Random _random = new Random();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartThreads_Click(object sender, RoutedEventArgs e)
        {
            NumberListBox.Items.Clear(); 
            StatusLabel.Content = "Threads running..."; 

            await Task.WhenAll(
                Task.Run(() => DisplayAscendingNumbers()),
                Task.Run(() => DisplayDescendingNumbers())
            );

            StatusLabel.Content = "Threads completed."; 
        }

        private async Task DisplayAscendingNumbers()
        {
            _mutex.WaitOne(); 
            try
            {
                for (int i = 0; i <= 20; i++)
                {
                    AppendToNumberListBox($"Thread 1: {i}");
                    await Task.Delay(_random.Next(50, 200)); 
                }
            }
            finally
            {
                _mutex.ReleaseMutex(); 
            }
        }

        private async Task DisplayDescendingNumbers()
        {
            await _semaphore.WaitAsync(); 
            try
            {
                await Task.Delay(100); 
                for (int i = 10; i >= 0; i--)
                {
                    AppendToNumberListBox($"Thread 2: {i}");
                    await Task.Delay(_random.Next(50, 200)); 
                }
            }
            finally
            {
                _semaphore.Release(); 
            }
        }

        private void AppendToNumberListBox(string text)
        {
            Dispatcher.Invoke(() =>
            {
                NumberListBox.Items.Add(text);
                NumberListBox.ScrollIntoView(text); 
            });
        }
    }
}