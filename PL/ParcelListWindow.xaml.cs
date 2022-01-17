using BlApi;
using BO;
using DalApi;
using DO;
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
    /// Interaction logic for ParcelListWindow.xaml
    /// </summary>
    public partial class ParcelListWindow : Window
    {

        IBL bl;
        public ObservableCollection<BO.ParcelToList> parcelToListsBL;
        public ParcelListWindow(IBL ibl)
        {
            InitializeComponent();
            bl = ibl;
            parcelToListsBL =
            new ObservableCollection<BO.ParcelToList>(from item in bl.GetParcelList()
                                                      orderby item.Id
                                                      select item);
            Parcels_ListBox.DataContext = parcelToListsBL;
            Parcels_ListBox.ItemsSource = parcelToListsBL;
            WeightSelector.ItemsSource = Enum.GetValues(enumType: typeof(DO.WeightCategories));
            PrioritySelector.ItemsSource = Enum.GetValues(enumType: typeof(DO.Priorities));
            parcelToListsBL.CollectionChanged += ParcelToListsBL_CollectionChanged;
        }

        /// <summary>
        /// collection change function
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ParcelToListsBL_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            PriorityAndWeight_SelectionChange();
        }


        
        public void PriorityAndWeight_SelectionChange(object sender, SelectionChangedEventArgs e)
        {
            PriorityAndWeight_SelectionChange();
        }


        /// <summary>
        /// button to open parcel window and add a parcel
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddingParcel_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button addParcelBn = sender as System.Windows.Controls.Button;
            if (addParcelBn != null)
            {
                new ParcelWindow(bl, this).Show();
            }
        }


        /// <summary>
        /// cancel event - to close the window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelAdd_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }


        /// <summary>
        /// to open parcel action window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseParcelFromTheList(object sender, MouseButtonEventArgs e)
        {
            ParcelWindow open =
                new ParcelWindow(bl, bl.GetParcel(((ParcelToList)Parcels_ListBox.SelectedItem).Id), this);
            open.Show();
        }


        /// <summary>
        /// get a list according to the selected priority and weight
        /// </summary>
        private void PriorityAndWeight_SelectionChange()
        {
            Parcels_ListBox.ItemsSource = from item in parcelToListsBL
                                          where
                                          (PrioritySelector.SelectedItem == null && WeightSelector.SelectedItem == null
                                          || item.Weight == (WeightCategories)WeightSelector.SelectedItem)
                                          && (WeightSelector.SelectedItem == null
                                          || item.Priority == (Priorities)PrioritySelector.SelectedItem)
                                          && (WeightSelector.SelectedItem == null
                                          || (item.Priority == (Priorities)PrioritySelector.SelectedItem)
                                          && item.Weight == (WeightCategories)WeightSelector.SelectedItem)
                                          orderby item.Id
                                          select item;
        }


        internal void refresh()
        {
            Close();
            new ParcelListWindow(bl).Show();
        }

    }
}
