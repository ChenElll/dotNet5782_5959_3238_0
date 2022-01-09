using System;
using System.Collections.Generic;
using System.Linq;
using BO;
using DO;
using BlApi;


namespace BL
{
    public partial class BL : IBL
    {

        #region ADD A CUSTOMER
        /// <summary>
        /// function that add a customer 
        /// </summary>
        /// <param name="customerToAdd"></param>
        public void AddCustomer(BO.Customer customerToAdd)
        {
            try
            {
                if (customerToAdd.Id < 0) //check that the customer id to add is not negative
                    throw new InvalidInputException("Customer id cannot be negative");

                //check if the the location is in our service limits
                if (customerToAdd.Location.Lattitude < 35.1252 || customerToAdd.Location.Lattitude > 35.2642
                      || customerToAdd.Location.Longtitude < 31.7082 || customerToAdd.Location.Longtitude > 31.8830)
                    throw new InvalidInputException("The chosen location is not within our service limits, " +
                            "choose location from these values: lattitude(35.1252-35.2642), longtitude(31.7082-31.8830)");

                //copy customer properties
                DO.Customer newCustomerDO = (DO.Customer)customerToAdd.CopyPropertiesToNew(typeof(DO.Customer));
                newCustomerDO.Lattitude = customerToAdd.Location.Lattitude;
                newCustomerDO.Longtitude = customerToAdd.Location.Longtitude;
                dal.AddingCustomer(newCustomerDO);
            }
            catch (Exception ex)
            {
                throw new AddingException("Can't add this customer", ex);
            }

        }
        #endregion


        #region UPDATE CUSTOMER DETAILS
        /// <summary>
        /// update customer's name and phone number
        /// </summary>
        /// <param name="Updates"></param>
        public void SetCustomerDetails(BO.Customer Updates)
        {
            try
            {
                if (Updates.Id < 0)
                    throw new InvalidInputException("customer id can not be negative");

                //search the customer to update
                DO.Customer customerDO = dal.GetCustomersList(x => x.Id == Updates.Id).First();

                //update the details- name and phone number
                if (Updates.CustomerName != null) customerDO.CustomerName = Updates.CustomerName;
                if (Updates.PhoneNumber != null) customerDO.PhoneNumber = Updates.PhoneNumber;
                dal.UpdateCustomer(customerDO);
            }
            catch (Exception ex)
            {
                throw new UpdateException("Can't update the station", ex);
            }
        }
        #endregion


        #region GET CUSTOMER
        /// <summary>
        /// function that gets a customer id and return the customer itself
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public BO.Customer GetCustomer(int customerId)
        {
            try
            {
                if (customerId < 0) //check that the customer id to get is not negative
                    throw new InvalidInputException("Customer id cannot be negative");
                DO.Customer customerDO = dal.GetCustomer(customerId);   //search the customer

                BO.Customer customerBO = (BO.Customer)customerDO.CopyPropertiesToNew(typeof(BO.Customer));
                customerBO.Location = new Location()
                { Longtitude = customerDO.Longtitude, Lattitude = customerDO.Lattitude };



                customerBO.FromCustomer = new List<ParcelAtCustomer>();
                //the list of parcel from customer - search if our customer id is the same as sender id in each parcel in parcel's list
                foreach (DO.Parcel item in
                    dal.GetParcelsList(item => item.SenderId == customerId))
                {
                    customerBO.FromCustomer.Add(GetParcelAtCustomer(item.Id));
                }


                customerBO.ToCustomer = new List<ParcelAtCustomer>();
                //search if our customer id is the same as target id in each parcel in parcel's list
                foreach (DO.Parcel item in
                    dal.GetParcelsList(item => item.TargetId == customerId))
                {
                    customerBO.ToCustomer.Add(GetParcelAtCustomer(item.Id));
                }
                return customerBO;

            }
            catch (Exception ex)
            {
                throw new GetException("Can't get the customer", ex);
            }

        }
        #endregion


        #region GET CUSTOMER LIST
        /// <summary>
        /// function that returns the whole customer's list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BO.CustomerToList> GetCustomerList(Func<DO.Customer, bool> predicat = null)
        {
            //returns list of all customer
            try
            {
                return from item in dal.GetCustomersList(predicat)
                       select GetCustomerToList(item.Id);
            }
            catch (Exception ex)
            {
                throw new GetListException("Can't get the stations list", ex);
            }
        }
        #endregion


        #region GET CUSTOMER IN PARCEL
        /// <summary>
        /// search and return customer in parcel that matches the id 
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        private BO.CustomerInParcel GetCustomerInParcel(int customerId)
        {
            try
            {
                if (customerId < 0) //check that the customer id to get is not negative
                    throw new InvalidInputException("Customer id cannot be negative");

                return (BO.CustomerInParcel)dal.GetCustomer(customerId).CopyPropertiesToNew(typeof(BO.CustomerInParcel));
            }
            catch (Exception ex)
            {
                throw new GetException("Can't get the customer", ex);
            }
        }
        #endregion


        #region GET CUSTOMER TO LIST
        /// <summary>
        /// return customer to list
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns></returns>
        private BO.CustomerToList GetCustomerToList(int customerId)
        {
            try
            {
                //search the customer
                DO.Customer customerDO = dal.GetCustomer(customerId);

                return new BO.CustomerToList()
                {
                    Id = customerDO.Id,
                    CustomerName = customerDO.CustomerName,
                    PhoneNumber = customerDO.PhoneNumber,

                    //parcels that are on their way to the customer, are scheduled but no delivered yet
                    NumberParcelsOnWay =
                           dal.GetParcelsList(item => item.SenderId == customerId
                                              && item.ScheduledTime != null
                                              && item.DeliveredTime == null).Count(),

                    //parcels that were picked up but no delivered yet
                    NumberSentAnd_Not_ProvidedParcels =
                           dal.GetParcelsList(item => item.SenderId == customerId
                                              && item.PickedUpTime != null
                                              && item.DeliveredTime == null).Count(),
                    //parcels that were piched up and delivered
                    NumberSentAndProvidedParcels =
                           dal.GetParcelsList(item => item.SenderId == customerId
                                              && item.PickedUpTime != null).Count(),

                    NumberParcelsReceived =
                           dal.GetParcelsList(item => item.SenderId == customerId
                                              && item.DeliveredTime != null).Count(),

                };
            }
            catch (Exception ex)
            {
                throw new GetException("Can't get the customer", ex);
            }
        }
        #endregion


        #region CUSTOMER DISTANCE
        /// <summary>
        /// calcul the distance between the drone and the customer
        /// </summary>
        /// <param name="pointLattitude"></param>
        /// <param name="pointLongtitude"></param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        public double CustomerDistance(Location location, int customerId)
        {
            BO.Location tempLocation = GetCustomer(customerId).Location;//(x => x.Id == customerId);
            return Distance(location, tempLocation);
        }
        #endregion

    }
}