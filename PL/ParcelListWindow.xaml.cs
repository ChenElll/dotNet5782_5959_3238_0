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
        }

        private void AddingParcel_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button addParcelBn = sender as System.Windows.Controls.Button;
            if (addParcelBn != null)
            {
                new ParcelWindow(bl, this).Show();
            }
        }

        private void CancelAdd_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ChooseParcelFromTheList(object sender, MouseButtonEventArgs e)
        {
            ParcelWindow open =
                new ParcelWindow(bl, bl.GetParcel(((ParcelToList)Parcels_ListBox.SelectedItem).Id), this);
            open.Show();
        }

        private void PriorityAndWeight_SelectionChange(object sender, SelectionChangedEventArgs e)
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
    }
}
