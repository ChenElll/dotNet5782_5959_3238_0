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
    /// Interaction logic for List_Of_Drones.xaml
    /// </summary>
    public partial class List_Of_Drones : Window
    {
        IBL bl;
        public ObservableCollection<BO.DroneToList> droneToListsBL;

        public List_Of_Drones(IBL ibl)
        {
            InitializeComponent();

            bl = ibl;

            droneToListsBL =
            new ObservableCollection<BO.DroneToList>(from item in bl.GetDroneList()
                                                         orderby item.Id
                                                         select item);
            Drones_ListBox.DataContext = droneToListsBL;
            Drones_ListBox.ItemsSource = droneToListsBL;
            StatusSelector.ItemsSource = Enum.GetValues(enumType: typeof(BO.DroneStatuses));
            WeightSelector.ItemsSource = Enum.GetValues(enumType: typeof(DO.WeightCategories));

        }


        //choice to select a drone according to it status and weight
        private void StatuesAndWeight_SelectionChange(object sender, SelectionChangedEventArgs e)
        {

            Drones_ListBox.ItemsSource = from item in droneToListsBL
                                         where
                                            (StatusSelector.SelectedItem == null
                                         || (DroneStatuses)StatusSelector.SelectedItem == DroneStatuses.All
                                         || item.Status == (DroneStatuses)StatusSelector.SelectedItem)
                                         && (WeightSelector.SelectedItem == null
                                         || (WeightCategories)WeightSelector.SelectedItem == WeightCategories.All
                                         || item.MaxWeight == (WeightCategories)WeightSelector.SelectedItem)
                                         orderby item.Id
                                         select item;
        }

        /// <summary>
        /// button to open add drone window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddingDrone_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button addDroneBn = sender as System.Windows.Controls.Button;
            if (addDroneBn != null)
            {
                new DroneWindow(bl, this).Show();
            }
        }

        /// <summary>
        /// double Click event that select a drone from the list and open drone window for "options" 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseDroneFromTheList(object sender, MouseButtonEventArgs e)
        {
            DroneWindow open =
                new DroneWindow(bl, bl.GetDrone(((DroneToList)Drones_ListBox.SelectedItem).Id), Drones_ListBox);
            open.Show();

        }

    }
}
