using System.Collections.ObjectModel;
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
using System.Windows.Threading;

namespace SystemPractice
{
    
    public partial class MainWindow : Window
    {
        object lockey = new();

        public ObservableCollection<Thread> CreatedThreads { get; set; }
        public ObservableCollection<Thread> WaitingThreads { get; set; }
        public ObservableCollection<Thread> WorkingThreads { get; set; }


        Semaphore semaphore;

        public MainWindow()
        {
            InitializeComponent();

            CreatedThreads = new();
            WaitingThreads = new();
            WorkingThreads = new();

            CreatedThreadsListView.ItemsSource = CreatedThreads;
            WaitingThreadsListView.ItemsSource = WaitingThreads;
            WorkingThreadsListView.ItemsSource = WorkingThreads;

            semaphore = new Semaphore(3, 3, "SEMAPHORE");
        }

        private void ListViewDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ((Thread)CreatedThreadsListView.SelectedItem).Start();
            
        }

        private void CreateButtonClick(object sender, RoutedEventArgs e)
        {
            Thread thread = new(() =>
            {
                int a = Thread.CurrentThread.ManagedThreadId;

                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var item in CreatedThreads)
                    {
                        if (item.ManagedThreadId == a)
                        {
                            WaitingThreads.Add(item);
                            CreatedThreads.Remove(item);
                            break;
                        }
                    }
                });

                

                semaphore.WaitOne();


                

                lock (lockey)
                {
                    Application.Current.Dispatcher.Invoke(() =>
                    {
                        foreach (var item in WaitingThreads)
                        {
                            if (item.ManagedThreadId == a)
                            {
                                WorkingThreads.Add(item);
                                WaitingThreads.Remove(item);
                                break;
                            }
                        }
                    });
                }

                Thread.Sleep(5000);

                semaphore.Release();

                
            });

            thread.Name = "Thread " + thread.ManagedThreadId;
            CreatedThreads.Add(thread);
        }
    }
}