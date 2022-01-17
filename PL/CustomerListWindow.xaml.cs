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
    /// Interaction logic for CustomerListWindow.xaml
    /// </summary>
    public partial class CustomerListWindow : Window
    {
        IBL bl;
        public ObservableCollection<BO.CustomerToList> customerToListsBL;
        public CustomerListWindow(IBL ibl)
        {
            InitializeComponent();
            bl = ibl;
            customerToListsBL =
            new ObservableCollection<BO.CustomerToList>(from item in bl.GetCustomerList()
                                                        orderby item.Id
                                                        select item);
            Customers_ListBox.DataContext = customerToListsBL;
            Customers_ListBox.ItemsSource = customerToListsBL;
            customerToListsBL.CollectionChanged += CustomerToListsBL_CollectionChanged;

        }

        private void CustomerToListsBL_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Customers_ListBox= (ListView)(from customer in customerToListsBL
                              select customer);
        }

        /// <summary>
        /// double Click event that select a customer from the list and open customer window for "options"
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ChooseCustomerFromTheList(object sender, MouseButtonEventArgs e)
        {
            CustomerWindow open =
                new CustomerWindow(bl, bl.GetCustomer(((CustomerToList)Customers_ListBox.SelectedItem).Id), this);
            open.Show();
        }

        /// <summary>
        /// button to open customer window and add a customer
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AddingCustomerButton_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Controls.Button addCustomerBn = sender as System.Windows.Controls.Button;
            if (addCustomerBn != null)
            {
                new CustomerWindow(bl, this).Show();
            }
        }


        /// <summary>
        /// refresh function
        /// </summary>
        internal void refresh()
        {
            Close();
            new CustomerListWindow(bl).Show();
        }


        /// <summary>
        /// cancel adding action and close window
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

    }
}
