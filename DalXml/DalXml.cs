using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DalApi;
using DO;
using Dal;


namespace Dal
{
    sealed class DalXml : IDal
    {
        #region singelton
        static readonly IDal instance = new DalXml();
        static DalXml() { }
        DalXml() { }
        public static IDal Instance { get => instance; }
        #endregion

        #region DS XML Files
        string dronesPath = @"DronesXml.xml";
        string customersPath = @"CustomersXml.xml";
        string droneChargesPath = @"DroneChargesXml.xml";
        string parcelsPath = @"ParceslXml.xml";
        string stationsPath = @"StationsXml.xml";
        #endregion









        #region --------------------------------------STATION-----------------------------------------

        #region AddStation
        /// <summary>
        /// add a station to the system
        /// </summary>
        /// <param name="stationToAdd"></param>
        public void AddStation(DO.Station stationToAdd)
        {
            List<DO.Station> ListStations = XMLTools.LoadListFromXMLSerializer<DO.Station>(stationsPath);

            if (ListStations.Exists(x => x.Id == stationToAdd.Id))
                throw new AlreadyExistException("Station with the same id already exists");

            // add a new station to station's list
            ListStations.Add((DO.Station)stationToAdd.CopyPropertiesToNew(typeof(DO.Station)));

            XMLTools.SaveListToXMLSerializer<DO.Station>(ListStations, stationsPath);
        }
        #endregion


        #region UpdateStation
        /// <summary>
        /// update station id, name, longtitude,lattitude,free charge slots
        /// </summary>
        /// <param name="updatedStation"></param>
        public void UpdateStation(Station updatedStation)
        {
            List<DO.Station> ListStations = XMLTools.LoadListFromXMLSerializer<DO.Station>(stationsPath);

            int stationIndex = ListStations.FindIndex(s => s.Id == updatedStation.Id);
            if (stationIndex == -1)
                throw new DoesntExistException("Error the station to update doesn't exis in the system");

            ListStations[stationIndex] = (DO.Station)updatedStation.CopyPropertiesToNew(typeof(DO.Station));

            XMLTools.SaveListToXMLSerializer<DO.Station>(ListStations, stationsPath);
        }
        #endregion


        #region GetStation
        public Station GetStation(int StationId) // the function gets the id number of the station
        {
            List<DO.Station> ListStations = XMLTools.LoadListFromXMLSerializer<DO.Station>(stationsPath);

            // the function search the station in station's list
            Station station = ListStations.Find(x => x.Id == StationId);
            // if the station was not found throw an exeption 
            if (station.Id == 0)
                throw new DoesntExistException("This station doesn't exist in the system");
            // if the station was found return the station
            return station;
        }
        #endregion


        #region GetStationsList
        ///// <summary>
        ///// get the list of the station
        ///// </summary>
        ///// <returns></returns>
        public IEnumerable<DO.Station> GetStationsList(Func<DO.Station, bool> predicat = null)
        {
            List<DO.Station> listStations = XMLTools.LoadListFromXMLSerializer<DO.Station>(stationsPath);
            var v = from item in listStations
                    select item;

            if (predicat == null)
                return v.AsEnumerable().OrderBy(s => s.Id);

            return v.Where(predicat).OrderBy(s => s.Id);
        }
        #endregion

        #endregion




        #region --------------------------------------PARCEL------------------------------------------

        #region AddParcel
        /// <summary>
        /// add a parcel to the system
        /// </summary>
        /// <param name="parcelToAdd"></param>
        /// <returns></returns>
        public int AddingParcel(Parcel parcelToAdd)
        {
            List<DO.Parcel> listParcel = XMLTools.LoadListFromXMLSerializer<DO.Parcel>(parcelsPath);

            parcelToAdd.Id = listParcel.Last().Id + 1;
            listParcel.Add((DO.Parcel)parcelToAdd.CopyPropertiesToNew(typeof(DO.Parcel)));

            XMLTools.SaveListToXMLSerializer<DO.Parcel>(listParcel, parcelsPath);

            return listParcel.Last().Id;
        }
        #endregion


        #region UpdateParcel
        /// <summary>
        /// update parcel's details
        /// </summary>
        /// <param name="updatedParcel"></param>
        public void UpdateParcel(Parcel updatedParcel)
        {
            List<DO.Parcel> listParcel = XMLTools.LoadListFromXMLSerializer<DO.Parcel>(parcelsPath);

            int parcelIndex = listParcel.FindIndex(d => d.Id == updatedParcel.Id);
            if (parcelIndex == -1)
                throw new DoesntExistException("Error the parcel to update doesn't exis in the system");

            listParcel[parcelIndex] = (DO.Parcel)updatedParcel.CopyPropertiesToNew(typeof(DO.Parcel));

            XMLTools.SaveListToXMLSerializer<DO.Parcel>(listParcel, parcelsPath);
        }
        #endregion


        #region GetParcel
        /// <summary>
        /// get a parcel by its id
        /// </summary>
        /// <param name="ParcelId"></param>
        /// <returns></returns>
        public Parcel GetParcel(int ParcelId) // the function gets the id number of the required parcel
        {
            List<DO.Parcel> listParcel = XMLTools.LoadListFromXMLSerializer<DO.Parcel>(parcelsPath);

            // the function search the parcel in parcel's list
            Parcel parcel = listParcel.Find(d => d.Id == ParcelId);
            // if the parcel was not found throw an exeption
            if (parcel.Id == 0)
                throw new DoesntExistException("This parcel doesn't exist in the system");
            // if the parcel was found return the parcel
            return parcel;
        }
        #endregion


        #region GetParcelsList
        /// <summary>
        /// get athe list of all the parcels
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Parcel> GetParcelsList(Func<DO.Parcel, bool> predicat = null)
        {
            List<DO.Parcel> listParcel = XMLTools.LoadListFromXMLSerializer<DO.Parcel>(parcelsPath);

            var v = from item in listParcel
                    select item;

            if (predicat == null)
                return v.AsEnumerable().OrderBy(P => P.Id);

            return v.Where(predicat).OrderBy(P => P.Id);
        }
        #endregion


        #region UpdateScheduled
        /// <summary>
        /// update scheduled parcel
        /// </summary>
        /// <param name="MyDroneId"></param>
        /// <param name="MyParcelId"></param>
        public void UpdateSchdule(int MyDroneId, int MyParcelId)
        {
            List<DO.Parcel> listParcel = XMLTools.LoadListFromXMLSerializer<DO.Parcel>(parcelsPath);

            int ParcelIndex = listParcel.FindIndex(x => x.Id == MyParcelId); // finding the parcel by id number
            if (ParcelIndex == -1)
                throw new DoesntExistException("This parcel doesn't exist in the system");

            List<DO.Drone> listDrone = XMLTools.LoadListFromXMLSerializer<DO.Drone>(dronesPath);

            int DroneIndex = listDrone.FindIndex(x => x.Id == MyDroneId);
            if (DroneIndex < 0)
                throw new DoesntExistException("This drone doesn't exist in the system");

            // update the parcel
            Parcel tempParcel = listParcel[ParcelIndex];
            tempParcel.DroneId = MyDroneId;
            tempParcel.ScheduledTime = DateTime.Now;
            listParcel[ParcelIndex] = tempParcel;

            XMLTools.SaveListToXMLSerializer<DO.Parcel>(listParcel, parcelsPath);
        }
        #endregion


        #region UpdatePickUp
        /// <summary>
        /// update parcel to pick up
        /// </summary>
        /// <param name="MyParcelId"></param>
        public void UpdatePickUp(int MyParcelId)
        {
            List<DO.Parcel> listParcel = XMLTools.LoadListFromXMLSerializer<DO.Parcel>(parcelsPath);

            int ParcelIndex = listParcel.FindIndex(x => x.Id == MyParcelId); // finding the parcel by id number
            // updates the pacel 
            if (ParcelIndex < 0)
                throw new DoesntExistException("This parcel doesn't exist in the system");

            Parcel tempParcel = listParcel[ParcelIndex];
            tempParcel.PickedUpTime = DateTime.Now;
            listParcel[ParcelIndex] = tempParcel;

            XMLTools.SaveListToXMLSerializer<DO.Parcel>(listParcel, parcelsPath);
        }
        #endregion


        #region UpdateDelivery
        /// <summary>
        /// update parcel delivery
        /// </summary>
        /// <param name="MyParcelId"></param>
        public void UpdateDelivery(int MyParcelId)
        {
            List<DO.Parcel> listParcel = XMLTools.LoadListFromXMLSerializer<DO.Parcel>(parcelsPath);

            int ParcelIndex = listParcel.FindIndex(x => x.Id == MyParcelId); // finding the parcel by id number
                                                                             // updates the pacel 
            if (ParcelIndex < 0)
                throw new DoesntExistException("This parcel doesn't exist in the system");

            Parcel tempParcel = listParcel[ParcelIndex];
            tempParcel.DeliveredTime = DateTime.Now;
            listParcel[ParcelIndex] = tempParcel;

            XMLTools.SaveListToXMLSerializer<DO.Parcel>(listParcel, parcelsPath);
        }
        #endregion

        #endregion




        #region   -------------------------------------CUSTOMER-----------------------------------------

        #region AddCustomer
        /// <summary>
        /// add a customer to the system
        /// </summary>
        /// <param name="customerToAdd"></param>
        public void AddingCustomer(Customer customerToAdd)
        {
            List<DO.Customer> listCustomer = XMLTools.LoadListFromXMLSerializer<DO.Customer>(customersPath);
            //add a new customer to customer's list
            if (listCustomer.Exists(x => x.Id == customerToAdd.Id))
                throw new AlreadyExistException("The customer already exist in the system");

            listCustomer.Add((DO.Customer)customerToAdd.CopyPropertiesToNew(typeof(DO.Customer)));

            XMLTools.SaveListToXMLSerializer<DO.Customer>(listCustomer, customersPath);

        }
        #endregion


        #region UpdateCustomer
        /// <summary>
        /// update customer details
        /// </summary>
        /// <param name="updatedCustomer"></param>
        public void UpdateCustomer(Customer updatedCustomer)
        {
            List<DO.Customer> listCustomer = XMLTools.LoadListFromXMLSerializer<DO.Customer>(customersPath);

            int customerIndex = listCustomer.FindIndex(c => c.Id == updatedCustomer.Id);
            if (customerIndex == -1)
                throw new DoesntExistException("Error the customer to update doesn't exis in the system");

            listCustomer[customerIndex] = (DO.Customer)updatedCustomer.CopyPropertiesToNew(typeof(DO.Customer));

            XMLTools.SaveListToXMLSerializer<DO.Customer>(listCustomer, customersPath);
        }
        #endregion


        #region GetCustomer
        /// <summary>
        /// gets a customer
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns></returns>
        public Customer GetCustomer(int CustomerId) // the function gets the id numver of the new customer 
        {
            List<DO.Customer> listCustomer = XMLTools.LoadListFromXMLSerializer<DO.Customer>(customersPath);

            // the function search the customer in customer's list
            Customer customer = listCustomer.Find(x => x.Id == CustomerId);

            // if the customer was not found throw an exception
            if (customer.Id == 0)
                throw new DoesntExistException("This customer doesn't exist in the system");

            // if the customer was found return the customer
            return customer;
        }
        #endregion


        #region GetCustomersList
        /// <summary>
        /// return list of customers
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Customer> GetCustomersList(Func<DO.Customer, bool> predicat = null)
        {
            List<DO.Customer> listCustomer = XMLTools.LoadListFromXMLSerializer<DO.Customer>(customersPath);

            var v = from item in listCustomer
                    select item;

            if (predicat == null)
                return v.AsEnumerable().OrderBy(C => C.Id);

            return v.Where(predicat).OrderBy(C => C.Id);
        }
        #endregion

        #endregion

    }
}
