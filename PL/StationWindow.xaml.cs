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

            myStation = bl.GetStation(selectedItem.Id);
            DataContext = myStation;
            stationListWindow = stationToLists;
            AddStationGrid.Visibility = Visibility.Hidden;
            UpdateStationGrid.Visibility = Visibility.Visible;
            FreeChargeSlotsText_View.Text = selectedItem.FreeChargeSlots.ToString();
            StationIdText_View.Text = selectedItem.Id.ToString();
            StationNameText_View.Text = selectedItem.Name.ToString();
            ListOfDroneInCharge.Text = (from item in selectedItem.DronesInCharge
                                        select item.Id).ToList().ToString();


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
                DronesInCharge = (System.Collections.Generic.List<DroneInCharge>)ListOfDroneInCharge.Text.AsEnumerable(),
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
                stationListWindow.stationToListsBL.Add(bl.GetStationList().FirstOrDefault(x => x.Id == station.Id));
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
            myStation = bl.GetStation(myStation.Id);

            if (StationNameText_View.Text!=myStation.Name || FreeChargeSlotsText_View.Text!= myStation.FreeChargeSlots.ToString())
            {
                myStation.Name = StationNameText_View.Text.ToString();
                myStation.FreeChargeSlots = int.Parse(FreeChargeSlotsText_View.Text);
                bl.SetStationDetails(myStation.Id);
                stationListWindow.refresh();
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
