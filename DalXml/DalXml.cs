using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DalApi;
using DO;
using Dal;
using System.Xml.Linq;

namespace Dal
{
    sealed class DalXml : IDal
    {
        #region singelton
        static readonly IDal instance = new DalXml();
        static DalXml() { }

        public static IDal Instance { get => instance; }
        #endregion

        #region DS XML Files
        string dronesPath = @"DronesXml.xml";
        string customersPath = @"CustomersXml.xml";
        string droneChargesPath = @"DroneChargesXml.xml";
        string parcelsPath = @"ParceslXml.xml";
        string stationsPath = @"StationsXml.xml";
        string configPath = @"ConfigXml.xml";
        #endregion


        #region ---------------------------------------DRONE------------------------------------------

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

        #region AddDrone
        /// <summary>
        /// add a drone to the system
        /// </summary>
        /// <param name="droneToAdd"></param>
        public void AddingDrone(Drone droneToAdd)
        {
            XElement droneRootElement = XMLTools.LoadListFromXMLElement(dronesPath);

            XElement tempDrone = (from item in droneRootElement.Elements()
                                  where int.Parse(item.Element("Id").Value) == droneToAdd.Id
                                  select item).FirstOrDefault();
            if (tempDrone != null)
                throw new AlreadyExistException("The drone already exist in the system");

            //add a drone to drone's xml
            droneRootElement.Add(new XElement("Id", droneToAdd.Id),
                                 new XElement("model", droneToAdd.Model),
                                 new XElement("MaxWeight", droneToAdd.MaxWeight));

            XMLTools.SaveListToXMLElement(droneRootElement, dronesPath);
        }
        #endregion


        #region UpdateDrone
        /// <summary>
        /// update drone id,model,weight
        /// </summary>
        /// <param name="updatedDrone"></param>
        public void UpdateDrone(Drone updatedDrone)
        {

            XElement droneRootElement = XMLTools.LoadListFromXMLElement(dronesPath);

            XElement tempDrone = (from item in droneRootElement.Elements()
                                  where int.Parse(item.Element("Id").Value) == updatedDrone.Id
                                  select item).FirstOrDefault();
            if (tempDrone == null)
                throw new DoesntExistException("Error the drone to update doesn't exis in the system");

            tempDrone.Element("Id").Value = updatedDrone.Id.ToString();
            tempDrone.Element("Model").Value = updatedDrone.Model.ToString();
            tempDrone.Element("MaxWeight").Value = updatedDrone.MaxWeight.ToString();

            XMLTools.SaveListToXMLElement(droneRootElement, dronesPath);
        }
        #endregion


        #region GetDrone
        /// <summary>
        /// gets a drone by the id
        /// </summary>
        /// <param name="DroneId"></param>
        /// <returns>drone</returns>
        public Drone GetDrone(int DroneId) //the function gets the id number of the required drone
        {

            XElement droneRootElement = XMLTools.LoadListFromXMLElement(dronesPath);

            var tempDrone = (from item in droneRootElement.Elements()
                             where int.Parse(item.Element("Id").Value) == DroneId
                             select new Drone()
                             {
                                 Id = int.Parse(item.Element("Id").Value),
                                 Model = item.Element("Model").Value.ToString(),
                                 MaxWeight = (DO.WeightCategories)Enum.Parse(typeof(DO.WeightCategories), item.Element("MaxWeight").Value.ToString())

                             }).FirstOrDefault();

            // if didn't find it throw an Exeption                                                                      
            if (tempDrone.Id == default)
                throw new DoesntExistException("This drone doesn't exist in the system");

            return tempDrone;   // if found it return the drone
        }
        #endregion


        #region GetDronesList
        /// <summary>
        /// get the lidt of the drones
        /// </summary>
        /// <returns>list of drone</returns>
        public IEnumerable<Drone> GetDronesList(Func<Drone, bool> predicat = null)
        {
            XElement droneRootElement = XMLTools.LoadListFromXMLElement(dronesPath);

            var v = (from item in droneRootElement.Elements()
                     select new Drone()
                     {
                         Id = int.Parse(item.Element("Id").Value),
                         Model = item.Element("Model").Value.ToString(),
                         MaxWeight = (DO.WeightCategories)Enum.Parse(typeof(DO.WeightCategories), item.Element("MaxWeight").Value.ToString())
                     }).ToList();

            if (predicat == null)
                return v.AsEnumerable();

            return v.Where(predicat);

        }
        #endregion

        #endregion




        #region ---------------------------------------DRONECHARGE------------------------------------------


        #region GetDroneChargesList
        /// <summary>
        /// get the list of drone in charge
        /// </summary>
        /// <returns>list of  drones in charge</returns>
        public IEnumerable<DroneCharge> GetDroneChargesList(Func<DroneCharge, bool> predicat = null)
        {
            List<DO.DroneCharge> droneChargeRoot = XMLTools.LoadListFromXMLSerializer<DO.DroneCharge>(droneChargesPath);


            var v = from item in droneChargeRoot
                    select item;

            if (predicat == null)
                return v.AsEnumerable();

            return v.Where(predicat);
        }
        #endregion


        #region UpdateSendDroneToCharge
        /// <summary>
        /// update drone that was sent to charge
        /// </summary>
        /// <param name="MyDrone"></param>
        /// <param name="MyStation"></param>
        public void UpdateDroneChargeCheckIn(int MyDrone, int MyStation)
        {
            List<DO.DroneCharge> ListDronesCharge = XMLTools.LoadListFromXMLSerializer<DO.DroneCharge>(droneChargesPath);

            List<DO.Station> ListStations = XMLTools.LoadListFromXMLSerializer<DO.Station>(stationsPath);

            int stationIndex = ListStations.FindIndex(x => x.Id == MyStation); //search station in station's list and update charge slots 
            if (stationIndex < 0)
                throw new DoesntExistException("the station doesn't exist in the system");

            ListDronesCharge.Add // add charge to drone's charge's list
                 (new DroneCharge()
                 {
                     StationId = MyStation,
                     DroneId = MyDrone,
                     EntranceTime = DateTime.Now,
                 });

            XMLTools.SaveListToXMLSerializer<DO.DroneCharge>(ListDronesCharge, droneChargesPath);
        }
        #endregion


        #region ReleaseDroneFromCharge
        /// <summary>
        /// update drone that was released from charge
        /// </summary>
        /// <param name="MyDrone"></param>
        /// <param name="MyStation"></param>
        public void UpdateDroneChargeCheckout(int MyDrone, int MyStation)
        {
            List<DO.DroneCharge> ListDronesCharge = XMLTools.LoadListFromXMLSerializer<DO.DroneCharge>(droneChargesPath);

            //finding the place in drone's charge's list where the drone we need to release is
            int chargeIndex = ListDronesCharge.FindIndex(x => x.DroneId == MyDrone);
            if (chargeIndex < 0)
                throw new DoesntExistException("the drone doesn't exist in the system");

            List<DO.Station> ListStations = XMLTools.LoadListFromXMLSerializer<DO.Station>(stationsPath);

            //search station in station's list and update charge slots 
            int stationIndex = ListStations.FindIndex(x => x.Id == MyStation);
            if (stationIndex < 0)
                throw new DoesntExistException("the station doesn't exist in the system");

            DroneCharge TemproneCharge = ListDronesCharge[chargeIndex];
            TemproneCharge.LeavingTime = DateTime.Now;
            ListDronesCharge[chargeIndex] = TemproneCharge;

            Station tempStation = ListStations[stationIndex];
            tempStation.FreeChargeSlots++;
            ListStations[stationIndex] = tempStation;


            XMLTools.SaveListToXMLSerializer<DO.DroneCharge>(ListDronesCharge, droneChargesPath);
            XMLTools.SaveListToXMLSerializer<DO.Station>(ListStations, stationsPath);
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
