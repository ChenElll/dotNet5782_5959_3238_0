using BlApi;
using BO;
using DO;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PL
{
    /// <summary>
    /// Interaction logic for DroneWindow.xaml
    /// </summary>
    public partial class DroneWindow : Window
    {
        IBL bl;
        private BO.Drone myDrone = null;
        private List_Of_Drones droneListWindow;

        /// <summary>
        /// constructor for adding a drone
        /// </summary>
        /// <param name="blD"></param>
        /// <param name="DroneListWindow"></param>
        public DroneWindow(IBL blD, List_Of_Drones droneToLists)
        {
            InitializeComponent();
            droneListWindow = droneToLists;
            this.bl = blD;
            this.DataContext = myDrone;
            AddDroneGrid.Visibility = Visibility.Visible;
            UpdateDroneGrid.Visibility = Visibility.Hidden;
            WeightSelect.ItemsSource = Enum.GetValues(typeof(WeightCategories));
            StationIdSelection.ItemsSource = from station in bl.GetStationList()
                                             select station.Id;
            
        }

        /// <summary>
        /// constructor for updates details of the drone
        /// </summary>
        /// <param name="blD"></param>
        /// <param name="drone"></param>
        public DroneWindow(IBL blD, BO.Drone selectedItem, List_Of_Drones droneToLists)
        {
            InitializeComponent();
            bl = blD;
            new ObservableCollection<BO.DroneToList>(from item in bl.GetDroneList()
                                                     where item.Id == selectedItem.Id
                                                     select item);
            BO.DroneToList tempDrone = bl.GetDroneList(item =>item.Id == selectedItem.Id).FirstOrDefault();
            myDrone = bl.GetDrone(selectedItem.Id);
            DataContext = myDrone;
            droneListWindow = droneToLists;
            AddDroneGrid.Visibility = Visibility.Hidden;
            UpdateDroneGrid.Visibility = Visibility.Visible;
            IdDroneText_View.Text = tempDrone.Id.ToString();
            IdDroneText_View.IsEnabled = false;
            ModelDroneText_View.Text = tempDrone.Model.ToString();
            ModelDroneText_View.IsEnabled = true;
            WeightSelect_View.Text = tempDrone.MaxWeight.ToString();
            WeightSelect_View.IsEnabled = false;
            BatteryText_View.Text = ((float)tempDrone.Battery).ToString();
            BatteryText_View.IsEnabled = false;
            StatusText_View.Text = tempDrone.Status.ToString();
            StatusText_View.IsEnabled = false;
            LocationText_View.Text = tempDrone.Location.ToString(); //??
            LocationText_View.IsEnabled = false;
            ChargeButton.DataContext = tempDrone.Status;

            // send drone to charge and send to delivery buttons just for drones that are available
            // release drone from charge button just for drones that are in maintenance
            if (myDrone.Status == DroneStatuses.available)
            {
                ChargeButton.Content = "Send drone to charge";
                ChargeButton.IsEnabled = true;
                ScheduleDroneButton.IsEnabled = true;
            }
            else if (myDrone.Status == DroneStatuses.maintenance)
            {
                ChargeButton.Content = "Release drone from charge";
                ChargeButton.IsEnabled = true;
                ScheduleDroneButton.IsEnabled = false;
            }
            else
            {
                ChargeButton.Visibility = Visibility.Collapsed;
                ScheduleDroneButton.Visibility = Visibility.Collapsed;
            }


        }

        //---------------------------------------------------- ADDING DRONE -------------------------------------------------------//

        private void StationIdSelection_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            StationIdSelection.ItemsSource = from station in bl.GetStationList()
                                             where station.Id == (int)StationIdSelection.SelectedItem
                                             select station;

        }


        /// <summary>
        /// select a drone from the list and open drone window for "options"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddToList_Click(object sender, RoutedEventArgs e)
        {
            bool closeWindow = true;
            BO.Drone drone = new BO.Drone
            {
                Id = int.Parse(IdDroneText.Text),
                Model = ModelDroneText.Text,
                MaxWeight = (WeightCategories)WeightSelect.SelectedItem,
            };
            MessageBoxResult result = MessageBox.Show("drone succefully added");
            try
            {
                bl.AddDrone(drone, (int)StationIdSelection.SelectedItem);
                myDrone = bl.GetDrone(drone.Id);
            }
            catch (Exception)
            {
                MessageBox.Show("can't add the drone"); //to string override
                //exception
            }
            if (closeWindow)
            {
                droneListWindow.droneToListsBL.Add(bl.GetDroneList(d => d.Id == drone.Id).First());
                Close();
            }
        }


        #region allow to input only positive
        /// <summary>
        /// allow to input only positive number to drone id text box
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TextBox_OnlyNumbers_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            try
            {
                TextBox text = sender as TextBox;
                if (text == null) return;
                if (e == null) return;

                //allow get out of the text box
                if (e.Key is Key.Enter or Key.Return or Key.Tab)
                {
                    return;
                }

                //allow list of system keys (add other key here if you want to allow)
                if (e.Key == Key.Escape || e.Key == Key.Back || e.Key == Key.Delete ||
                    e.Key == Key.CapsLock || e.Key == Key.LeftShift || e.Key == Key.Home || e.Key == Key.End ||
                    e.Key == Key.Insert || e.Key == Key.Down || e.Key == Key.Right)
                    return;

                char c = (char)KeyInterop.VirtualKeyFromKey(e.Key);

                //allow control system keys
                if (char.IsControl(c)) return;

                //allow digits (without Shift or Alt)
                if (char.IsDigit(c))
                    if (!(Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightAlt)))
                        return; //let this key be written inside the textbox

                //forbid letters and signs (#,$, %, ...)
                e.Handled = true; //ignore this key. mark event as handled, will not be routed to other controls
                return;
            }
            catch(Exception)
            {
                MessageBox.Show("you can enter only numbers");
            }

        }
        #endregion


        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        //---------------------------------------------------- UPDATE DRONE -------------------------------------------------------//

        /// <summary>
        /// function to update drone's details
        /// details to update - model
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (myDrone.Model != ModelDroneText_View.Text) 
            {
                myDrone.Model = ModelDroneText_View.Text;
                bl.SetDroneName(myDrone);
                droneListWindow.refresh();
                MessageBoxResult result = MessageBox.Show("drone succefully updated");
                Close();
                
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("you can't update the drone");
            }


        }

        /// <summary>
        /// send to\release from charge according to the content of the button
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChargeButton_Click(object sender, RoutedEventArgs e)
        {
            DataContext = ChargeButton.Content;
            if (ChargeButton.Content.ToString() == "Send drone to charge")
            {
                bl.SetSendDroneToCharge(myDrone.Id);
                droneListWindow.refresh();
                Close();
            }
            else
            {
                //bl.SetReleaseDroneFromCharge(myDrone.Id,);
            }
        }

        private void ScheduleDroneButton_Click(object sender, RoutedEventArgs e) //
        {

            bl.UpdateScheduleParcel(myDrone.Id); 
            DroneToList drone = new DroneToList
            {
                Id = myDrone.Id,
                Model = myDrone.Model,
                MaxWeight = myDrone.MaxWeight,
                Status = myDrone.Status,
                Battery = myDrone.Battery,
                Location = myDrone.Location,
            };
            droneListWindow.droneToListsBL.Remove(drone);
            droneListWindow.droneToListsBL.Add(drone);
        }

        private void PickUpParcelButton_Click(object sender, RoutedEventArgs e) //to check
        {
            if (myDrone.Status != DroneStatuses.shipped && !myDrone.ParcelInTransfer.ParcelStatus) //איסוף חבילה(רק עבור רחפן במשלוח עם חבילה שעוד לא נאספה
            {
                PickUpParcelButton.IsEnabled = false;
            }
            else
            {
                PickUpParcelButton.IsEnabled = true;
            }
            bl.UpdatePickUpParcel(myDrone.Id); //??
            DroneToList drone = new DroneToList
            {
                Id = myDrone.Id,
                Model = myDrone.Model,
                MaxWeight = myDrone.MaxWeight,
                Status = myDrone.Status,
                Battery = myDrone.Battery,
                Location = myDrone.Location,
            };
            droneListWindow.droneToListsBL.Remove(drone);
            droneListWindow.droneToListsBL.Add(drone);
        }

        private void DeliverParcelButton_Click(object sender, RoutedEventArgs e)
        {
            if (myDrone.Status != DroneStatuses.shipped && myDrone.ParcelInTransfer.ParcelStatus) //אספקת החבילה(רק עבור הרחפן במשלוח שכבר אסף את החבילה)
            {
                DeliverParcelButton.IsEnabled = false;
            }
            else
            {
                DeliverParcelButton.IsEnabled = true;
            }

        }
    }
}
