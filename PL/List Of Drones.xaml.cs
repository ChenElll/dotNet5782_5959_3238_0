using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
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



            droneToListsBL = new ObservableCollection<BO.DroneToList>();
            var dronesTemp = (from item in bl.GetDroneList()
                              orderby item.Id
                              select item).AsEnumerable();


            foreach (var item in dronesTemp)
            {
                droneToListsBL.Add(item);
            }



            Drones_ListBox.ItemsSource = droneToListsBL;
            StatusSelector.ItemsSource = Enum.GetValues(enumType: typeof(BO.DroneStatuses));
            WeightSelector.ItemsSource = Enum.GetValues(enumType: typeof(DO.WeightCategories));
            droneToListsBL.CollectionChanged += DroneToListsBL_CollectionChanged;
            // DroneWindow DrnWnd = new(bl,this);
            //DrnWnd.UpdateButton.Click += DrnWnd.UpdateButton_Click;
        }

        private void DroneToListsBL_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            StatuesAndWeight_SelectionChange();

            
            //ObservableCollection < DroneToList > d = new ObservableCollection<DroneToList>();
            //ObservableCollection<DroneToList> obsSender = sender as ObservableCollection<DroneToList>;
            //NotifyCollectionChangedAction action = e.Action;
        }

        public void StatuesAndWeight_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            StatuesAndWeight_SelectionChange();

        }
        //choice to select a drone according to it status and weight
        public void StatuesAndWeight_SelectionChange()
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
                new DroneWindow(bl, bl.GetDrone(((DroneToList)Drones_ListBox.SelectedItem).Id), this);
            
           // Drones_ListBox.ItemsSource = bl.GetDroneList();
            open.Show();

        }

        internal void refresh()
        {
            Close();
            new List_Of_Drones(bl).Show();
        }

        /// <summary>
        /// cancel adding action and close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelAdd_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
