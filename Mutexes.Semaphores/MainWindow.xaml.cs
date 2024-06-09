using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Windows;

namespace Mutexes.Semaphores
{
    public partial class MainWindow : Window
    {
        private Mutex mutex = new Mutex();
        private Random random = new Random();
        private int totalPlayersVisited = 0;
        private int totalPlayers = 0;
        private string reportFileName = "casino_report.txt";
        private int betAmount;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void StartSimulation_Click(object sender, RoutedEventArgs e)
        {
            startButton.IsEnabled = false;
            progressText.Text = "Simulation started...\n";
            totalPlayers = random.Next(20, 101);
            List<Thread> threads = new List<Thread>();

            for (int i = 0; i < totalPlayers; i++)
            {
                Thread thread = new Thread(PlayerThread);
                threads.Add(thread);
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            progressText.Text += "All players have finished playing. Generating report...\n";
            GenerateReport();
            progressText.Text += "Report generated successfully.\n";
            startButton.IsEnabled = true;
        }

        private void PlayerThread()
        {
            int playerId = Interlocked.Increment(ref totalPlayersVisited);
            int money = random.Next(100, 1001);
            int betsPlaced = 0;
            int betsWon = 0;

            while (true)
            {
                int betAmount = random.Next(10, 101);
                int betNumber = random.Next(0, 37);

                mutex.WaitOne();
                try
                {
                    if (money <= 0)
                    {
                        break;
                    }

                    if (betsPlaced >= 100)
                    {
                        break;
                    }

                    int rouletteNumber = random.Next(0, 37);
                    if (rouletteNumber == betNumber)
                    {
                        money += betAmount;
                        betsWon++;
                    }
                    else
                    {
                        money -= betAmount;
                    }

                    betsPlaced++;

                    progressText.Dispatcher.Invoke(() =>
                    {
                        progressText.Text += $"Player {playerId}: Placed bet ${betAmount} on {betNumber}. ";
                        if (rouletteNumber == betNumber)
                        {
                            progressText.Text += $"Won ${betAmount * 2}!\n";
                        }
                        else
                        {
                            progressText.Text += $"Lost.\n";
                        }
                    });
                }
                finally
                {
                    mutex.ReleaseMutex();
                }

                Thread.Sleep(1000);
            }

            mutex.WaitOne();
            try
            {
                using (StreamWriter writer = new StreamWriter(reportFileName, true))
                {
                    writer.WriteLine($"Player ID: {playerId}");
                    writer.WriteLine($"Initial Money: {money + betAmount}");
                    writer.WriteLine($"Bets Placed: {betsPlaced}");
                    writer.WriteLine($"Bets Won: {betsWon}");
                    writer.WriteLine($"Final Money: {money}");
                    writer.WriteLine();
                }
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }

        private void GenerateReport()
        {
            mutex.WaitOne();
            try
            {
                using (StreamWriter writer = new StreamWriter(reportFileName, true))
                {
                    writer.WriteLine($"Total Players Visited: {totalPlayersVisited}");
                    writer.WriteLine($"Total Players: {totalPlayers}");
                }
            }
            finally
            {
                mutex.ReleaseMutex();
            }
        }
    }
}
