using BlApi;
using BO;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace PL
{
    /// <summary>
    /// Interaction logic for StationWindow.xaml
    /// </summary>
    public partial class StationWindow : Window
    {
        IBL bl;
        private BO.BaseStation myStation = null;
        private StationListWindow stationListWindow;

        /// <summary>
        /// constructor for adding station
        /// </summary>
        /// <param name="blD"></param>
        /// <param name="stationToLists"></param>
        public StationWindow(IBL blD, StationListWindow stationToLists)
        {
            InitializeComponent();
            this.bl = blD;
            stationListWindow = stationToLists;
            this.DataContext = myStation;
            StationSelected_Box.ItemsSource = (System.Collections.IEnumerable)myStation;
            AddStationGrid.Visibility = Visibility.Visible;
            UpdateStationGrid.Visibility = Visibility.Hidden;
        }

        /// <summary>
        /// constructor for updates details of the station
        /// </summary>
        /// <param name="blD"></param>
        /// <param name="drone"></param>
        public StationWindow(IBL blD, BaseStation selectedItem, StationListWindow stationToLists)
        {
            InitializeComponent();
            bl = blD;
            new ObservableCollection<BO.BaseStationToList>(from item in bl.GetStationList()
                                                               where item.Id == selectedItem.Id
                                                               select item);
            AddStationGrid.Visibility = Visibility.Hidden;
            UpdateStationGrid.Visibility = Visibility.Visible;
            //ListOfDroneInCharge = DroneInCharge;

        }

        /// <summary>
        /// cancel adding action
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelStation_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        /// <summary>
        /// function to add a station to the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddToList_Click(object sender, RoutedEventArgs e)
        {
            bool closeWindow = true;
            BaseStation station = new BaseStation
            {
                Id = int.Parse(IdStationText.Text),
                Name = NameStationText.Text,
                Location = { Lattitude = double.Parse(LattitudeStationText.Text), Longtitude = double.Parse(LattitudeStationText.Text) },
                FreeChargeSlots = int.Parse(NumOfChargeSlotsText.Text),
            };
            MessageBoxResult result = MessageBox.Show("Station succefully added");
            try
            {
                bl.AddStation(station);
                myStation = bl.GetStation(station.Id);
            }
            catch (Exception)
            {
                MessageBox.Show("can't add the station"); //to string override
                //exception
            }
            if (closeWindow)
            {
                stationListWindow.stationToListsBL.Add(bl.GetStationList().First(d => d.Id == station.Id));
                Close();
            }
        }


        //-------------------------------------------------------Update Station-----------------------------------------------------------//

        /// <summary>
        /// function to update a the details of the station
        /// set station's name and free charge slots
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            // myStation = bl.GetStation(myStation.Id);?
            BaseStationToList station = new BaseStationToList
            {
                Id = myStation.Id,
                Name = myStation.Name,
                FreeChargeSlots = myStation.FreeChargeSlots,
                OccupiedChargeSlots = myStation.DronesInCharge.Count,
            };

            if (stationListWindow.stationToListsBL.Remove(station))
            {
                myStation.Name = StationNameText_View.Text.ToString();
                station.Name = myStation.Name;
                bl.SetStationDetails(myStation.Id);
                stationListWindow.stationToListsBL.Add(station);
                MessageBoxResult result = MessageBox.Show("Station succefully updated");
                Close();
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("you can't update the station");
            }
        }


    }
}
