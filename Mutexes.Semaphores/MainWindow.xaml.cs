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
        private SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void StartTask_Click(object sender, RoutedEventArgs e)
        {
            OutputTextBox.Text = ""; 

            await Task.Run(() => ModifyArrayAndFindMax());
        }

        private void ModifyArrayAndFindMax()
        {
            int[] array = new int[10];

            _mutex.WaitOne(); 
            try
            {
                Random random = new Random();
                for (int i = 0; i < array.Length; i++)
                {
                    array[i] = random.Next(100); 
                    AppendToOutput($"Modified array element {i}: {array[i]}");
                    Thread.Sleep(100); 
                }

                int maxValue = array.Max();
                AppendToOutput($"Maximum value in the array: {maxValue}");
            }
            finally
            {
                _mutex.ReleaseMutex(); 
            }
        }

        private void AppendToOutput(string text)
        {
            Dispatcher.Invoke(() =>
            {
                OutputTextBox.AppendText(text + Environment.NewLine);
                OutputTextBox.ScrollToEnd(); 
            });
        }
    }
}