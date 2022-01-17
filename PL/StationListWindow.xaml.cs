using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using BlApi;
using BO;
using DO;

namespace PL
{
    /// <summary>
    /// Interaction logic for StationListWindow.xaml
    /// </summary>
    public partial class StationListWindow : Window
    {

        IBL bl;
        public ObservableCollection<BO.BaseStationToList> stationToListsBL;

        /// <summary>
        /// constructor for adding station
        /// </summary>
        /// <param name="ibl"></param>
        public StationListWindow(IBL ibl)
        {
            InitializeComponent();
            bl = ibl;
            stationToListsBL =
            new ObservableCollection<BO.BaseStationToList>(from item in bl.GetStationList()
                                                           orderby item.Id
                                                           select item);
            //Stations_ListBox.DataContext = stationToListsBL;
            Stations_ListBox.ItemsSource = stationToListsBL;
            stationToListsBL.CollectionChanged += StationToListsBL_CollectionChanged;
            ChargeSlotsSelector.ItemsSource = Enumerable.Range(0, 100);
        }

        private void StationToListsBL_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            FreeChargeSlots_SelectionChange();
        }

        /// <summary>
        /// open station window and add a station to the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddingStation_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button addStationBn = sender as System.Windows.Controls.Button;
            if (addStationBn != null)
            {
                new StationWindow(bl, this).Show();
            }
        }

        /// <summary>
        /// double Click event that select a station from the list and open station window for "options" 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseStationFromTheList(object sender, MouseButtonEventArgs e)
        {
            StationWindow open =
                new StationWindow(bl, bl.GetStation(((BaseStationToList)Stations_ListBox.SelectedItem).Id), this);
            open.Show();
        }


        /// <summary>
        /// button add a station
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddingStationButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button addStationBn = sender as System.Windows.Controls.Button;
            if (addStationBn != null)
            {
                new StationWindow(bl, this).Show();
            }
        }


        /// <summary>
        /// cancel adding action and close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        public void FreeChargeSlots_SelectionChange()
        {
            Stations_ListBox.ItemsSource = from item in stationToListsBL
                                           where (int)ChargeSlotsSelector.SelectedItem == item.FreeChargeSlots
                                           select item;
        }

        public void FreeChargeSlots_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            FreeChargeSlots_SelectionChange();
        }
        internal void refresh()
        {
            Close();
            new StationListWindow(bl).Show();
        }
    }
}
