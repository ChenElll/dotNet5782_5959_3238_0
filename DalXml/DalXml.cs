using DalApi;
using DO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

namespace Dal
{
    sealed class DalXml : IDal
    {
        #region DS XML Files
        //the data base
        string dronesPath = @"DronesXml.xml";
        string customersPath = @"CustomersXml.xml";
        string droneChargesPath = @"DroneChargesXml.xml";
        string parcelsPath = @"ParceslXml.xml";
        string stationsPath = @"StationsXml.xml";
        string configPath = @"ConfigXml.xml";
        #endregion

        #region singelton
        //oneaccess t he data
        static readonly IDal instance = new DalXml();
        static DalXml() { }

        public static IDal Instance { get => instance; }

        XElement droneRoot;

        DalXml()
        {
            if (!File.Exists(dronesPath))
                CreateFiles();
            else
                LoadData();
        }

        private void CreateFiles()
        {
            droneRoot = new XElement("drones");
            droneRoot.Save(dronesPath);
        }

        private void LoadData()
        {
            try
            {
                droneRoot = XElement.Load(dronesPath);
            }
            catch
            {
                throw new LoadingException("File upload problem");
            }
        }
        #endregion


        #region ---------------------------------------DRONE------------------------------------------



        #region AddDrone
        /// <summary>
        /// add a drone to the system
        /// </summary>
        /// <param name="newDrone"></param>
        public void AddingDrone(Drone newDrone)
        {
            List<DO.Drone> dronesList = XMLTools.LoadListFromXMLSerializer<DO.Drone>(dronesPath);

            if (dronesList.Any(d => d.Id == newDrone.Id))
                throw new AlreadyExistException("The drone already exist in the system");
            dronesList.Add((DO.Drone)newDrone.CopyPropertiesToNew(typeof(DO.Drone)));

            XMLTools.SaveListToXMLSerializer<DO.Drone>(dronesList, dronesPath);
        }
        #endregion


        #region UpdateDrone
        /// <summary>
        /// update drone id,model,weight
        /// </summary>
        /// <param name="updatedDrone"></param>
        public void UpdateDrone(Drone updatedDrone)
        {
            List<DO.Drone> dronesList = XMLTools.LoadListFromXMLSerializer<DO.Drone>(dronesPath);

            int droneIndex = dronesList.FindIndex(d => d.Id == updatedDrone.Id);
            if (droneIndex == -1)
                throw new DoesntExistException("Error the drone to update doesn't exis in the system");
            dronesList[droneIndex] = (DO.Drone)updatedDrone.CopyPropertiesToNew(typeof(DO.Drone));

            XMLTools.SaveListToXMLSerializer<DO.Drone>(dronesList, dronesPath);
        }
        #endregion


        #region GetDrone
        /// <summary>
        /// gets a drone by the id
        /// </summary>
        /// <param name="droneId">the function gets the id number of the required drone</param>
        /// <returns>drone</returns>
        public Drone GetDrone(int droneId)
        {
            List<DO.Drone> dronesList = XMLTools.LoadListFromXMLSerializer<DO.Drone>(dronesPath);

            if (!dronesList.Any(d => d.Id == droneId))
                throw new DoesntExistException("This drone doesn't exist in the system");
            return dronesList.Find(d => d.Id == droneId);
        }
        #endregion


        #region GetDronesList
        /// <summary>
        /// get the list of the drones
        /// </summary>
        /// <param name="predicat">filter the items to return, acording the requested predicat</param>
        /// <returns></returns>
        public IEnumerable<Drone> GetDronesList(Func<Drone, bool> predicat = null)
        {
            List<DO.Drone> dronesList = XMLTools.LoadListFromXMLSerializer<DO.Drone>(dronesPath);

            if (predicat == null)
                return from item in dronesList
                       orderby item.Id
                       select item;
            return from item in dronesList
                   orderby item.Id
                   where predicat(item)
                   select item;
        }
        #endregion

        #endregion




        #region ---------------------------------------DRONECHARGE------------------------------------------


        #region GetDroneChargesList
        /// <summary>
        /// get the list of drone in charge
        /// </summary>
        /// <param name="predicat">filter the items to return, acording the requested predicat</param>
        /// <returns></returns>
        public IEnumerable<DroneCharge> GetDroneChargesList(Func<DroneCharge, bool> predicat = null)
        {
            List<DO.DroneCharge> droneChargesList = XMLTools.LoadListFromXMLSerializer<DO.DroneCharge>(droneChargesPath);

            if (predicat == null)
                return droneChargesList.AsEnumerable();
            return from item in droneChargesList
                   where predicat(item)
                   select item;
        }
        #endregion


        #region UpdateSendDroneToCharge
        /// <summary>
        /// update drone that was sent to charge
        /// </summary>
        /// <param name="myDrone"></param>
        /// <param name="myStation"></param>
        public void UpdateDroneChargeCheckIn(int myDrone, int myStation)
        {
            //search station in station's list and update charge slots 
            List<DO.Station> stationsList = XMLTools.LoadListFromXMLSerializer<DO.Station>(stationsPath);
            List<DO.DroneCharge> droneChargesList = XMLTools.LoadListFromXMLSerializer<DO.DroneCharge>(droneChargesPath);

            int stationIndex = stationsList.FindIndex(s => s.Id == myStation);
            if (stationIndex == -1)
                throw new DoesntExistException("the station doesn't exist in the system");
            Station tempStation = stationsList[stationIndex];
            tempStation.FreeChargeSlots--;
            stationsList[stationIndex] = tempStation;
            // add charge to dronecharge list
            droneChargesList.Add
                 (new DroneCharge()
                 {
                     StationId = myStation,
                     DroneId = myDrone,
                     EntranceTime = DateTime.Now,
                 });

            XMLTools.SaveListToXMLSerializer<DO.DroneCharge>(droneChargesList, droneChargesPath);
            XMLTools.SaveListToXMLSerializer<DO.Station>(stationsList, stationsPath);
        }
        #endregion


        #region ReleaseDroneFromCharge
        /// <summary>
        /// update drone that was released from charge
        /// </summary>
        /// <param name="myDrone"></param>
        /// <param name="myStation"></param>
        public void UpdateDroneChargeCheckout(int myDrone, int myStation)
        {
            //finding the place in drone's charge's list where the drone we need to release is
            List<DO.DroneCharge> droneChargesList = XMLTools.LoadListFromXMLSerializer<DO.DroneCharge>(droneChargesPath);
            List<DO.Station> stationsList = XMLTools.LoadListFromXMLSerializer<DO.Station>(stationsPath);

            int droneChargeIndex = droneChargesList.FindIndex(d => d.DroneId == myDrone);
            if (droneChargeIndex == -1)
                throw new DoesntExistException("the drone doesn't exist in the system");
            DroneCharge TemproneCharge = droneChargesList[droneChargeIndex];
            TemproneCharge.LeavingTime = DateTime.Now;
            droneChargesList[droneChargeIndex] = TemproneCharge;

            //search station in station's list and update charge slots 
            int stationIndex = stationsList.FindIndex(s => s.Id == myStation);
            if (stationIndex == -1)
                throw new DoesntExistException("the station doesn't exist in the system");
            Station tempStation = stationsList[stationIndex];
            tempStation.FreeChargeSlots++;
            stationsList[stationIndex] = tempStation;

            XMLTools.SaveListToXMLSerializer<DO.DroneCharge>(droneChargesList, droneChargesPath);
            XMLTools.SaveListToXMLSerializer<DO.Station>(stationsList, stationsPath);
        }
        #endregion


        #region getRangeChargeByDrone
        /// <summary>
        /// get the request of use of electricity
        /// </summary>
        /// <returns></returns>
        public double[] getElectricityUseByDrone()
        {
            List<double> electricityUseByDrone1RootElement = XMLTools.LoadListFromXMLSerializer<double>(configPath);

            return electricityUseByDrone1RootElement.ToArray();
        }
        #endregion


        #endregion




        #region --------------------------------------STATION-----------------------------------------

        #region AddStation
        /// <summary>
        /// add a station to the system
        /// </summary>
        /// <param name="newStation"></param>
        public void AddStation(DO.Station newStation)
        {
            List<DO.Station> stationsList = XMLTools.LoadListFromXMLSerializer<DO.Station>(stationsPath);

            if (stationsList.Any(s => s.Id == newStation.Id))
                throw new AlreadyExistException("Station with the same id already exists");
            stationsList.Add((DO.Station)newStation.CopyPropertiesToNew(typeof(DO.Station)));

            XMLTools.SaveListToXMLSerializer<DO.Station>(stationsList, stationsPath);
        }
        #endregion


        #region UpdateStation
        /// <summary>
        /// update station id, name, longtitude,lattitude,free charge slots
        /// </summary>
        /// <param name="updatedStation"></param>
        public void UpdateStation(Station updatedStation)
        {
            List<DO.Station> stationsList = XMLTools.LoadListFromXMLSerializer<DO.Station>(stationsPath);

            int stationIndex = stationsList.FindIndex(s => s.Id == updatedStation.Id);
            if (stationIndex == -1)
                throw new DoesntExistException("Error the station to update doesn't exis in the system");
            stationsList[stationIndex] = (DO.Station)updatedStation.CopyPropertiesToNew(typeof(DO.Station));

            XMLTools.SaveListToXMLSerializer<DO.Station>(stationsList, stationsPath);
        }
        #endregion


        #region GetStation
        public Station GetStation(int stationId) // the function gets the id number of the station
        {
            List<DO.Station> stationsList = XMLTools.LoadListFromXMLSerializer<DO.Station>(stationsPath);

            if (!stationsList.Any(s => s.Id == stationId))
                throw new DoesntExistException("This station doesn't exist in the system");
            return stationsList.Find(s => s.Id == stationId);
        }
        #endregion

        #region GetStationsList
        /// <summary>
        /// get the list of the station
        /// </summary>
        /// <param name="predicat">filter the items to return, acording the requested predicat</param>
        /// <returns></returns>
        public IEnumerable<DO.Station> GetStationsList(Func<DO.Station, bool> predicat = null)
        {
            List<DO.Station> stationsList = XMLTools.LoadListFromXMLSerializer<DO.Station>(stationsPath);

            if (predicat == null)
                return from item in stationsList
                       orderby item.Id
                       select item;
            return from item in stationsList
                   orderby item.Id
                   where predicat(item)
                   select item;
        }
        #endregion

        #endregion




        #region --------------------------------------PARCEL------------------------------------------

        #region AddParcel
        /// <summary>
        /// add a parcel to the system
        /// </summary>
        /// <param name="newParcel"></param>
        /// <returns></returns>
        public int AddingParcel(Parcel newParcel)
        {
            List<DO.Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<DO.Parcel>(parcelsPath);

            newParcel.Id = parcelsList.Last().Id + 1; //Give a new number to the new parcel
            parcelsList.Add((DO.Parcel)newParcel.CopyPropertiesToNew(typeof(DO.Parcel)));
            //save and return
            XMLTools.SaveListToXMLSerializer<DO.Parcel>(parcelsList, parcelsPath);
            return newParcel.Id;
        }
        #endregion


        #region UpdateParcel
        /// <summary>
        /// update parcel's details
        /// </summary>
        /// <param name="updatedParcel"></param>
        public void UpdateParcel(Parcel updatedParcel)
        {
            List<DO.Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<DO.Parcel>(parcelsPath);

            int parcelIndex = parcelsList.FindIndex(p => p.Id == updatedParcel.Id);
            if (parcelIndex == -1)
                throw new DoesntExistException("Error the parcel to update doesn't exis in the system");
            parcelsList[parcelIndex] = (DO.Parcel)updatedParcel.CopyPropertiesToNew(typeof(DO.Parcel));

            XMLTools.SaveListToXMLSerializer<DO.Parcel>(parcelsList, parcelsPath);
        }
        #endregion


        #region GetParcel
        /// <summary>
        /// get a parcel by its id
        /// </summary>
        /// <param name="parcelId">id number of the required parcel</param>
        /// <returns></returns>
        public Parcel GetParcel(int parcelId)
        {
            List<DO.Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<DO.Parcel>(parcelsPath);

            if (!parcelsList.Any(p => p.Id == parcelId))
                throw new DoesntExistException("This parcel doesn't exist in the system");
            return parcelsList.Find(p => p.Id == parcelId);
        }
        #endregion


        #region GetParcelsList
        /// <summary>
        /// get the list of the parcels
        /// </summary>
        /// <param name="predicat">filter the items to return, acording the requested predicat</param>
        /// <returns></returns>
        public IEnumerable<Parcel> GetParcelsList(Func<DO.Parcel, bool> predicat = null)
        {
            List<DO.Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<DO.Parcel>(parcelsPath);

            if (predicat == null)
                return from item in parcelsList
                       orderby item.Id
                       select item;
            return from item in parcelsList
                   orderby item.Id
                   where predicat(item)
                   select item;
        }
        #endregion


        #region UpdateScheduled
        /// <summary>
        /// update parcel's schedule
        /// </summary>
        /// <param name="myDroneId">id of the drone that wil take the parcel</param>
        /// <param name="myParcelId"></param>
        public void UpdateSchdule(int myDroneId, int myParcelId)
        {
            List<DO.Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<DO.Parcel>(parcelsPath);
            List<DO.Drone> dronesList = XMLTools.LoadListFromXMLSerializer<DO.Drone>(dronesPath);

            int parcelIndex = parcelsList.FindIndex(p => p.Id == myParcelId);
            if (parcelIndex == -1)
                throw new DoesntExistException("This parcel doesn't exist in the system");
            if (!dronesList.Any(d => d.Id == myDroneId))
                throw new DoesntExistException("This drone doesn't exist in the system");

            Parcel tempParcel = parcelsList[parcelIndex];
            tempParcel.DroneId = myDroneId;
            tempParcel.ScheduledTime = DateTime.Now;
            parcelsList[parcelIndex] = tempParcel;

            XMLTools.SaveListToXMLSerializer<DO.Parcel>(parcelsList, parcelsPath);
        }
        #endregion


        #region UpdatePickUp
        /// <summary>
        /// update parcel's pick up
        /// </summary>
        /// <param name="myParcelId"></param>
        public void UpdatePickUp(int myParcelId)
        {
            List<DO.Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<DO.Parcel>(parcelsPath);

            int parcelIndex = parcelsList.FindIndex(p => p.Id == myParcelId);
            if (parcelIndex == -1)
                throw new DoesntExistException("This parcel doesn't exist in the system");

            Parcel tempParcel = parcelsList[parcelIndex];
            tempParcel.PickedUpTime = DateTime.Now;
            parcelsList[parcelIndex] = tempParcel;

            XMLTools.SaveListToXMLSerializer<DO.Parcel>(parcelsList, parcelsPath);
        }
        #endregion


        #region UpdateDelivery
        /// <summary>
        /// update parcel's delivery
        /// </summary>
        /// <param name="myParcelId"></param>
        public void UpdateDelivery(int myParcelId)
        {
            List<DO.Parcel> parcelsList = XMLTools.LoadListFromXMLSerializer<DO.Parcel>(parcelsPath);

            int ParcelIndex = parcelsList.FindIndex(p => p.Id == myParcelId);
            if (ParcelIndex <= -1)
                throw new DoesntExistException("This parcel doesn't exist in the system");

            Parcel tempParcel = parcelsList[ParcelIndex];
            tempParcel.DeliveredTime = DateTime.Now;
            parcelsList[ParcelIndex] = tempParcel;

            XMLTools.SaveListToXMLSerializer<DO.Parcel>(parcelsList, parcelsPath);
        }
        #endregion

        #endregion




        #region   -------------------------------------CUSTOMER-----------------------------------------

        #region AddCustomer
        /// <summary>
        /// add a new customer to the system
        /// </summary>
        /// <param name="newCustomer"></param>
        public void AddingCustomer(Customer newCustomer)
        {
            List<DO.Customer> customersList = XMLTools.LoadListFromXMLSerializer<DO.Customer>(customersPath);

            if (customersList.Any(C => C.Id == newCustomer.Id))
                throw new AlreadyExistException("The customer already exist in the system");
            customersList.Add((DO.Customer)newCustomer.CopyPropertiesToNew(typeof(DO.Customer)));

            XMLTools.SaveListToXMLSerializer<DO.Customer>(customersList, customersPath);

        }
        #endregion


        #region UpdateCustomer
        /// <summary>
        /// update customer details
        /// </summary>
        /// <param name="updatedCustomer"></param>
        public void UpdateCustomer(Customer updatedCustomer)
        {
            List<DO.Customer> customersList = XMLTools.LoadListFromXMLSerializer<DO.Customer>(customersPath);

            int customerIndex = customersList.FindIndex(c => c.Id == updatedCustomer.Id);
            if (customerIndex == -1)
                throw new DoesntExistException("Error the customer to update doesn't exis in the system");
            customersList[customerIndex] = (DO.Customer)updatedCustomer.CopyPropertiesToNew(typeof(DO.Customer));

            XMLTools.SaveListToXMLSerializer<DO.Customer>(customersList, customersPath);
        }
        #endregion


        #region GetCustomer
        /// <summary>
        /// gets customer by the id
        /// </summary>
        /// <param name="customerId">id numver of the request customer</param>
        /// <returns></returns>
        public Customer GetCustomer(int customerId)
        {
            List<DO.Customer> customersList = XMLTools.LoadListFromXMLSerializer<DO.Customer>(customersPath);

            Customer customer = customersList.Find(c => c.Id == customerId);
            if (!customersList.Any(c => c.Id == customerId))
                throw new DoesntExistException("This customer doesn't exist in the system");

            return customersList.Find(c => c.Id == customerId);
        }
        #endregion


        #region GetCustomersList
        /// <summary>
        /// get the list of the customers
        /// </summary>
        /// <param name="predicat">filter the items to return, acording the requested predicat</param>
        /// <returns></returns>
        public IEnumerable<Customer> GetCustomersList(Func<DO.Customer, bool> predicat = null)
        {
            List<DO.Customer> customersList = XMLTools.LoadListFromXMLSerializer<DO.Customer>(customersPath);

            if (predicat == null)
                return from item in customersList
                       orderby item.Id
                       select item;
            return from item in customersList
                   orderby item.Id
                   where predicat(item)
                   select item;
        }
        #endregion

        #endregion

    }
}
