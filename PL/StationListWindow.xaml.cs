using BL;
using BlApi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Shapes;

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
            Stations_ListBox.DataContext = stationToListsBL;
            Stations_ListBox.ItemsSource = stationToListsBL;
            ChargeSlotsSelector.ItemsSource = Enumerable.Range(0, 100);
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
        /// select station by their number of charge slots
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FreeChargeSlots_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            if (ExistSlotsSelector.SelectedItem.ToString() == "No")
            {
                Stations_ListBox.ItemsSource = from item in stationToListsBL
                                               where item.FreeChargeSlots == 0
                                               orderby item.Id
                                               select item;
            }
            else
            {
                Stations_ListBox.ItemsSource = from item in stationToListsBL
                                               where item.FreeChargeSlots == (int)ChargeSlotsSelector.SelectedItem
                                               orderby item.Id
                                               select item;
            }
        }


        ///// <summary>
        ///// button add a station
        ///// </summary>
        ///// <param name="sender"></param>
        ///// <param name="e"></param>
        //private void AddingStationButton_Click(object sender, RoutedEventArgs e)
        //{
        //    System.Windows.Controls.Button addStationBn = sender as System.Windows.Controls.Button;
        //    if (addStationBn != null)
        //    {
        //        new StationWindow(bl, this).Show();
        //    }
        //}


        /// <summary>
        /// cancel adding action and close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


    }
}
