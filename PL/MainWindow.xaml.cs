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
                new List_Of_Drones(bl).Show();
            }
        }
    }
}
