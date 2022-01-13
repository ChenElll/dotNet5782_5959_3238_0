using BlApi;
using BO;
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
    /// Interaction logic for CustomerWindow.xaml
    /// </summary>
    public partial class CustomerWindow : Window
    {
        IBL bl;
        private BO.Customer myCustomer = null;
        private CustomerListWindow customerListWindow;

        /// <summary>
        /// constructor for adding a customer
        /// </summary>
        /// <param name="blD"></param>
        /// <param name="DroneListWindow"></param>
        public CustomerWindow(IBL blD, CustomerListWindow customerToLists)
        {
            InitializeComponent();
            customerListWindow = customerToLists;
            this.bl = blD;
            this.DataContext = myCustomer;
            AddCustomerGrid.Visibility = Visibility.Visible;
            UpdateCustomerGrid.Visibility = Visibility.Hidden;
            CustomerSelected_Box.ItemsSource = (System.Collections.IEnumerable)myCustomer;
        }


        /// <summary>
        /// constructor for updates details of the customer
        /// </summary>
        /// <param name="blD"></param>
        /// <param name="drone"></param>
        public CustomerWindow(IBL blD, Customer selectedItem, CustomerListWindow customerToLists)
        {
            InitializeComponent();
            bl = blD;
            new ObservableCollection<BO.CustomerToList>(from item in bl.GetCustomerList()
                                                        where item.Id == selectedItem.Id
                                                        select item);
            customerListWindow = customerToLists;
            myCustomer = selectedItem;
            this.DataContext = selectedItem;
            AddCustomerGrid.Visibility = Visibility.Hidden;
            UpdateCustomerGrid.Visibility = Visibility.Visible;
            CustomerIdText_View.Text = selectedItem.Id.ToString();
            CustomerIdText_View.IsEnabled = false;
            CustomerNameText_View.Text = selectedItem.CustomerName.ToString();
            CustomerNameText_View.IsEnabled = true;
            CustomerPhoneText_View.Text = selectedItem.PhoneNumber.ToString();
            CustomerPhoneText_View.IsEnabled = true;
            ParcelsFromCustomer_View.Content = (from item in selectedItem.FromCustomer
                                                select item.Id).ToString();
            ParcelsFromCustomer_View.IsEnabled = false;
            ParcelsToCustomer_View.Content = (from item in selectedItem.ToCustomer
                                              select item.Id).ToString();
            ParcelsToCustomer_View.IsEnabled = false;
            CustomerLattitudeText_View.DataContext = selectedItem.Location.Lattitude;
            CustomerLongitudeText_View.DataContext = selectedItem.Location.Longtitude;
            CustomerLattitudeText_View.IsEnabled = false;
            CustomerLongitudeText_View.IsEnabled = false;

        }


        /// <summary>
        /// update the changes for customer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            BO.CustomerToList customer = new BO.CustomerToList
            {
                Id = myCustomer.Id,
                CustomerName = myCustomer.CustomerName,
                PhoneNumber = myCustomer.PhoneNumber,
                //NumberParcelsOnWay = ,
                //NumberParcelsReceived = ,
                //NumberSentAndProvidedParcels = ,
                //NumberSentAnd_Not_ProvidedParcels=,
            };


            if (customerListWindow.customerToListsBL.Remove(bl.GetCustomerList().First(x => x.Id == myCustomer.Id)))
            {
                customer.CustomerName = CustomerNameText_View.Text;
                customer.PhoneNumber = CustomerPhoneText_View.Text;
                customerListWindow.customerToListsBL.Add(customer);
                MessageBoxResult result = MessageBox.Show("Station succefully updated");
                Close();
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("you can't update the station");
            }
        }

        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            bool closeWindow = true;
            Customer customer = new Customer
            {
                Id = int.Parse(CustomerIdText_View.Text),
                CustomerName = NameCustomerText.Text,
                PhoneNumber = CustomerPhoneText.Text,
                Location= new Location {Longtitude=double.Parse(LongitudeCustomerText.Text), Lattitude= double.Parse(LattitudeCustomerText.Text) },
                //FromCustomer= from ,
                //ToCustomer=,
            };
            MessageBoxResult result = MessageBox.Show("customer succefully added");
            try
            {
                bl.AddCustomer(customer);
                customer = bl.GetCustomer(customer.Id);
            }
            catch (Exception)
            {
                MessageBox.Show("can't add the customer");
                //exception
            }
            if (closeWindow)
            {
                customerListWindow.customerToListsBL.Add(bl.GetCustomerList().First(d => d.Id == customer.Id));
                Close();
            }
        }

        private void CancelCustomer_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
