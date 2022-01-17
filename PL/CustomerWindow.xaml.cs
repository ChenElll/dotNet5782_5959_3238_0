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
        /// CONSTRUCTOR FOR ADDING CUSTOMER
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
        }


        /// <summary>
        /// CONSTRUCTOR FOR CUSTOMER ACTIONS 
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
            myCustomer = bl.GetCustomer(selectedItem.Id);
            DataContext = myCustomer;
            AddCustomerGrid.Visibility = Visibility.Hidden;
            UpdateCustomerGrid.Visibility = Visibility.Visible;
            CustomerIdText_View.Text = selectedItem.Id.ToString();
            CustomerIdText_View.IsEnabled = false;
            CustomerNameText_View.Text = selectedItem.CustomerName.ToString();
            CustomerNameText_View.IsEnabled = true;
            CustomerPhoneText_View.Text = selectedItem.PhoneNumber.ToString();
            CustomerPhoneText_View.IsEnabled = true;
            //ListOfCustomerFrom_View.Text = (from item in bl.GetParcelList()
            //                                where item.NameSender == selectedItem.CustomerName
            //                                select item.Id).ToList().ToString();
            //ListOfCustomerTo_View.Text = (from item in selectedItem.ToCustomer
            //                              select item.Id).AsEnumerable().ToString();
            CustomerLattitudeText_View.Text = selectedItem.Location.Lattitude.ToString();
            CustomerLongitudeText_View.Text = selectedItem.Location.Longtitude.ToString();
            CustomerLattitudeText_View.IsEnabled = false;
            CustomerLongitudeText_View.IsEnabled = false;

        }



        #region UPDATE CLICK
        /// <summary>
        /// update the changes for customer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void UpdateCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            if (bl.GetCustomer(myCustomer.Id).CustomerName != CustomerNameText_View.Text
                || bl.GetCustomer(myCustomer.Id).PhoneNumber != CustomerPhoneText_View.Text)
            {
                myCustomer.CustomerName = CustomerNameText_View.Text;
                myCustomer.PhoneNumber = CustomerPhoneText_View.Text;
                bl.SetCustomerDetails(myCustomer);
                customerListWindow.refresh();
                MessageBoxResult result = MessageBox.Show("Customer succefully updated");
                Close();
            }
            else
            {
                MessageBoxResult result = MessageBox.Show("you can't update the customer");
            }
        }
        #endregion



        /// <summary>
        /// add customer to list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddCustomer_Click(object sender, RoutedEventArgs e)
        {
            bool closeWindow = true;
            Customer customer = new Customer
            {
                Id = int.Parse(CustomerIdText_View.Text),
                CustomerName = NameCustomerText.Text,
                PhoneNumber = CustomerPhoneText.Text,
                Location = new Location { Longtitude = double.Parse(LongitudeCustomerText.Text), Lattitude = double.Parse(LattitudeCustomerText.Text) },
                
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

        
        private void ListOfCustomerFrom_View_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            //int IdParcel = ((ParcelAtCustomer)ListOfCustomerFrom_View.SelectedItem).Id;
            //ParcelToList customer = bl.GetParcelList().First(x => x.Id == IdParcel);
            //new ParcelWindow(bl, ParcelListWindow).Show();
            
        }

        
    }
}
