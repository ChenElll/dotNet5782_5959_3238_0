using BlApi;
using BO;
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
    /// Interaction logic for ParcelWindow.xaml
    /// </summary>
    public partial class ParcelWindow : Window
    {
        IBL bl;
        private BO.Parcel myParcel = null;
        private ParcelListWindow parcelListWindow;

        /// <summary>
        /// constructor for adding a parcel
        /// </summary>
        /// <param name="blD"></param>
        /// <param name="parcelToLists"></param>
        public ParcelWindow(IBL blD, ParcelListWindow parcelToLists)
        {
            InitializeComponent();
            parcelListWindow = parcelToLists;
            this.bl = blD;
            this.DataContext = myParcel;
            AddParcelGrid.Visibility = Visibility.Visible;
            UpdateParcelGrid.Visibility = Visibility.Hidden;

        }

        /// <summary>
        /// constructor for updating  parcel
        /// </summary>
        /// <param name="blD"></param>
        /// <param name="selectedItem"></param>
        /// <param name="parcelToLists"></param>
        public ParcelWindow(IBL blD, BO.Parcel selectedItem, ParcelListWindow parcelToLists)
        {
            InitializeComponent();
            bl = blD;
            new ObservableCollection<BO.ParcelToList>(from item in bl.GetParcelList()
                                                      where item.Id == selectedItem.Id
                                                      select item);
            parcelListWindow = parcelToLists;
            myParcel = bl.GetParcel(selectedItem.Id);
            DataContext = myParcel;
            SenderParcelText_View.Text = bl.GetParcelList().First(x => x.Id == myParcel.Id).NameSender.ToString();
            TargetParcelText_View.Text= bl.GetParcelList().First(x => x.Id == myParcel.Id).NameTarget.ToString();
        }

        /// <summary>
        /// cancel  the adding and closing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        /// <summary>
        /// function to add a parcel to the list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddToList_Click(object sender, RoutedEventArgs e)
        {
            CustomerInParcel customerS = new();
            CustomerInParcel customerT = new();
            DroneInParcel droneInParcel = new();
            customerS.CustomerName = SenderParcelText.Text;
            customerT.CustomerName = TargetParcelText.Text;
            droneInParcel.Id = int.Parse(DroneParcelText.Text);
            bool closeWindow = true;
            BO.Parcel parcel = new BO.Parcel
            {
                //Id = int.Parse(IdParcelText.Text),
                //Sender = customerS,
                //Target = customerT,
                //Weight = WeightCategories WeightParcelText.Text,
                //Priority = (Priorities).PriorityParcelText.Text,
                //Drone = droneInParcel,
                //RequestedTime = (DateTime?)RequestedParcel_View.Text,
                //ScheduleTime = DateTime.Parse(ScheduledParcelText.Text),
                //PickUpTime = DateTime.Parse(PickUpParcelText.Text),
                //DeliveredTime = DateTime.Parse(DeliveredParcelText.Text),
            };
            MessageBoxResult result = MessageBox.Show("Parcel succefully added");
            try
            {
                bl.AddParcel(parcel);
                myParcel = bl.GetParcel(parcel.Id);
            }
            catch (Exception)
            {
                MessageBox.Show("can't add the parcel"); //to string override
                //exception
            }
            if (closeWindow)
            {
                parcelListWindow.parcelToListsBL.Add(bl.GetParcelList().First(d => d.Id == parcel.Id));
                Close();
            }
        }

        private void UpdateButton_Click(object sender, RoutedEventArgs e)
        {

            if (myParcel.RequestedTime.ToString() != RequestedParcel_View.Text || myParcel.ScheduleTime.ToString() != ScheduledParcel_View.Text
                || myParcel.DeliveredTime.ToString() != deliveredParcel_View.Text || myParcel.PickUpTime.ToString() != PickUpParcel_View.Text)
            {
                myParcel.ScheduleTime = DateTime.Parse(ScheduledParcel_View.Text);
                myParcel.PickUpTime = DateTime.Parse(PickUpParcel_View.Text);
                myParcel.DeliveredTime = DateTime.Parse(deliveredParcel_View.Text);
                bl.UpdateDeliveryToCustomer(myParcel.Id);
                bl.UpdatePickUpParcel(myParcel.Id);
                bl.UpdateScheduleParcel(myParcel.Id);
                parcelListWindow.refresh();
                MessageBoxResult result = MessageBox.Show("Parcel succefully updated");
                Close();
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("you can't update the parcel");
            }
        }
    }
}
