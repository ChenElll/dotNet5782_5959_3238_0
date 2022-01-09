using System;
using System.Windows;
using System.Windows.Controls;
using BlApi;
//using IBL;


namespace PL
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        internal readonly IBL bl = BlFactory.GetBl();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void ShowDrones_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                new DroneListWindow(bl).Show();
            }
        }

        private void ShowStations_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                new StationListWindow(bl).Show();
            }
        }

        private void ShowCustomers_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                new CustomerListWindow(bl).Show();
            }
        }

        private void ShowParcels_Click(object sender, RoutedEventArgs e)
        {
            Button b = sender as Button;
            if (b != null)
            {
                new ParcelListWindow(bl).Show();
            }
        }

        private void clipVideo_Loaded(object sender, RoutedEventArgs e)
        {
            clip.Play();
        }

        private void clipVideo_MediaEnded(object sender, RoutedEventArgs e)
        {
            clip.Position = TimeSpan.FromSeconds(0);
        }

    }
}
