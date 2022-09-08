using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlClient;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using System.Xml.Linq;
using DalApi;
using DO;


namespace DalDB
{
    class DalDB : IDal
    {
        SqlCommand mySqlConnectionCommand;
        ConnectDB inst = new ConnectDB();
        private void connectDB()
        {
            mySqlConnectionCommand = inst.connectDB();

        }
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
        static readonly IDal instance = new DalDB();
        static DalDB() { }

        public static IDal Instance { get => instance; }

        XElement droneRoot;

         ()
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

        /*  #region  paths
          /// <summary>
          /// consts as paths
          /// </summary>
          const string DRONEPATH = @"drones.xml";
          const string PARCELPATH = @"parcels.xml";
          const string BASESTATIONPATH = @"stations.xml";
          const string CUSTOMERPATH = @"customer.xml";
          const string DRONECHARGEPATH = @"droneCharge.xml";
          #endregion
          #region singelton
          /// <summary>
          /// makes sure only one user can approch the code.
          /// </summary>
          class Nested
          {
              static Nested() { }
              internal static readonly DalDB instance = new DalDB();
          }

          private static object syncRoot = new object();
          public static DalDB Instance
          {
              get
              {
                  if (Nested.instance == null)
                  {
                      lock (syncRoot)
                      {
                          if (Nested.instance == null)
                              return Nested.instance;
                      }
                  }

                  return Nested.instance;
              }
          }
          public DalDB()
          {
              //empty charging drone list
              releaseDroneFromCharge();
              connectDB();
          }
          #endregion

          #region customers
          /// <summary>
          /// method to add a customer
          /// </summary>
          /// <param name="customer"></param>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public void AddCustomer(Customer customer)
          {
              mySqlConnectionCommand.CommandText = "SELECT * FROM Customer";
              using SqlDataReader rdr = mySqlConnectionCommand.ExecuteReader();
              while (rdr.Read())
              {
                  if (rdr.GetInt32(0) == customer.Id)
                  {
                      throw new CostumerExeption("id already exist");
                  }
              }
              mySqlConnectionCommand.CommandText = string.Format("INSERT INTO Customer (Name,Phone,Longitude,Latitude) VALUES ({0} {1} {2} {4})", customer.Name, customer.Phone, customer.Longitude, customer.Latitude);
              mySqlConnectionCommand.ExecuteNonQuery();
          }
          /// <summary>
          /// detele element
          /// </summary>
          /// <param name="id"></id element>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public void DeleteCustomer(int id)
          {
              bool flag = true;
              mySqlConnectionCommand.CommandText = "SELECT * FROM Customer";
              using SqlDataReader rdr = mySqlConnectionCommand.ExecuteReader();
              while (rdr.Read())
              {
                  if (rdr.GetInt32(0) == id)
                  {
                      flag = false;
                  }
              }
              if (flag)
              {
                  throw new DO.CostumerExeption($"Customer with {id} as Id does not exist");
              }
              mySqlConnectionCommand.CommandText = string.Format("UPDATE Customer SET Valid=0 WHERE id={0}", id);
              mySqlConnectionCommand.ExecuteNonQuery();
          }

          /// <summary>
          /// gets customer from database and return it to main
          /// </summary>
          /// <param name="id"></param>
          /// <returns></the customer got>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public Customer GetCustomer(int id)
          {
              mySqlConnectionCommand.CommandText = string.Format("SELECT * FROM Customer WHERE Id={0}", id);
              using SqlDataReader rdr = mySqlConnectionCommand.ExecuteReader();
              bool flag = rdr.Read();
              if (!flag)
                  throw new CostumerExeption($"Customer with {id} as Id does not exist");

              return new Customer { Id = rdr.GetInt32(0), Name = rdr.GetString(1), Phone = rdr.GetString(2), Longitude = rdr.GetDouble(3), Latitude = rdr.GetDouble(4) }
          ;
              }

          /// <summary>
          /// func that returns list to print in console
          /// </summary>
          /// <returns></returns>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public IEnumerable<Customer> GetCustomerList(Func<Customer, bool> predicate = null)
          {
              List<Customer> res = new List<Customer>();
              mySqlConnectionCommand.CommandText = "SELECT * FROM Customer";
              using SqlDataReader rdr = mySqlConnectionCommand.ExecuteReader();
              while( rdr.Read())
              {
                  Customer cus= new Customer { Id = rdr.GetInt32(0), Name = rdr.GetString(1), Phone = rdr.GetString(2), Longitude = rdr.GetDouble(3), Latitude = rdr.GetDouble(4) };
                  if(predicate==null || predicate(cus) )
                  {
                      res.Add(cus);
                  }
              }

              if (res.Count==0)
                  throw new CostumerExeption($"Customer with predicate match does not exist");
              return res;
          }


          /// <summary>
          /// update a customer from bl to data saurce
          /// </summary>
          /// <param name="customer"></param>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public void UpdateCustomerInfoFromBL(Customer customer)
          {
              mySqlConnectionCommand.CommandText = $"SELECT * FROM Customer WHERE Id={customer.Id}";
              using SqlDataReader rdr = mySqlConnectionCommand.ExecuteReader();
              bool flag = rdr.Read();
              if (!flag)
              {
                  throw new CostumerExeption($"Customer with {customer.Id} as Id does not exist");
              }
              mySqlConnectionCommand.CommandText = $"UPDATE Customer SET (Name,Phone,Latitude,Longitude) VALUES ({customer.Name}, {customer.Phone},{customer.Longitude},{customer.Longitude})";
              mySqlConnectionCommand.ExecuteNonQuery();

          }
          #endregion

          #region electricity
          ///    /// <summary>
          /// return list of consumation data
          /// </summary>
          /// <returns></returns>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public double[] DroneElectricConsumations()
          {
              var returnedArrays = XMLTools.LoadListFromXMLSerializer<double>(@"configs.xml");
              return returnedArrays.ToArray();
          }
          #endregion

          #region dronecharge
          /// <summary>
          /// method to add a dronecharge unit.
          /// </summary>
          /// <param "id drone, id parcel"></param>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public void AddDroneCharge(int idDrone, int idBase)
          {
              mySqlConnectionCommand.CommandText = $"SELECT * FROM Drones WHERE Id={idDrone}";
              using SqlDataReader rdr = mySqlConnectionCommand.ExecuteReader();
              if(!rdr.Read())
              {
                  throw new DroneException("id of drone not found");
              }
              mySqlConnectionCommand.CommandText = $"SELECT * FROM DroneCharge WHERE Id={idBase}";
              using SqlDataReader rdr2 = mySqlConnectionCommand.ExecuteReader();
              if (!rdr2.Read())
              {
                  throw new DroneException("id of base not found");
              }
              mySqlConnectionCommand.CommandText = $"INSERT INTO DroneCharge (DroneId,StationId) VALUES ({idDrone}, {idBase} ";
              mySqlConnectionCommand.ExecuteNonQuery();
              baseStationDroneIn(idBase);
          }
          /// <summary>
          /// release drone from charge
          /// </summary>
          [MethodImpl(MethodImplOptions.Synchronized)]
          private void releaseDroneFromCharge()
          {
              var drones1 = XMLTools.LoadListFromXMLSerializer<DroneCharge>(DRONECHARGEPATH);
              if (drones1.Count() > 0)
              {
                  foreach (var unit in drones1)//
                      baseStationDroneOut(unit.StationId);
              }
              drones1.Clear();
              XMLTools.SaveListToXMLSerializer(drones1, DRONECHARGEPATH);
          }
          /// <summary>
          /// func that update basestation free slots when drone comes in
          /// </summary>
          /// <param name="baseStationId"></param>
          [MethodImpl(MethodImplOptions.Synchronized)]
          private void baseStationDroneIn(int baseStationId)
          {
              XElement baseStations = XMLTools.LoadListFromXMLElement(BASESTATIONPATH);
              XElement baseStation = (from bs in baseStations.Elements()
                                      where bs.Element("id").Value == $"{baseStationId}"
                                      select bs).FirstOrDefault();
              int availableChargingPorts = Convert.ToInt32(baseStation.Element("numOfSlots").Value);
              --availableChargingPorts;
              baseStation.Element("numOfSlots").Value = availableChargingPorts.ToString();
              XMLTools.SaveListToXMLElement(baseStations, BASESTATIONPATH);
          }
          /// <summary>
          /// func that update basestation free slots when drone goes out
          /// </summary>
          /// <param name="baseStationId"></param>
          [MethodImpl(MethodImplOptions.Synchronized)]
          private void baseStationDroneOut(int baseStationId)
          {
              XElement baseStations = XMLTools.LoadListFromXMLElement(BASESTATIONPATH);
              XElement baseStation = (from bs in baseStations.Elements()
                                      where bs.Element("id").Value == $"{baseStationId}"
                                      select bs).FirstOrDefault();
              int availableChargingPorts = Convert.ToInt32(baseStation.Element("numOfSlots").Value);
              ++availableChargingPorts;
              baseStation.Element("numOfSlots").Value = availableChargingPorts.ToString();

              XMLTools.SaveListToXMLElement(baseStations, BASESTATIONPATH);
          }
          /// <summary>
          /// return the drone in charge
          /// </summary>
          /// <param name="droneId"></param>
          /// <returns></returns>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public int GetDroneChargeBaseStationId(int droneId)
          {
              XElement DroneCharges = XMLTools.LoadListFromXMLElement(DRONECHARGEPATH);
              return (from dc in DroneCharges.Elements()
                      where dc.Element("DroneId").Value == $"{droneId}"
                      select Convert.ToInt32(dc.Element("StationId").Value))
                  .FirstOrDefault();
          }
          /// <summary>
          /// method to delete a dronecharge unit.
          /// </summary>
          /// <param "id drone></param>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public void DeleteDroneCharge(int idDrone)
          {
              //check if drone and charge unit exists
              DroneCharge? myDrone = null;
              List<DroneCharge> dronesCharge = XMLTools.LoadListFromXMLSerializer<DroneCharge>(DRONECHARGEPATH);
              myDrone = dronesCharge.Where(dr => dr.DroneId == idDrone).FirstOrDefault();
              if (myDrone == null)
                  throw new DroneChargeException("id of drone not found");
              //take oout from data
              baseStationDroneOut(myDrone.Value.StationId);
              dronesCharge.RemoveAll(d => d.DroneId == idDrone);
              XMLTools.SaveListToXMLSerializer(dronesCharge, DRONECHARGEPATH);
          }
          #endregion

          #region drones
          /// <summary>
          /// send a new drone to database
          /// </summary>
          /// <param name="drone"></param>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public void AddDrone(Drone drone)
          {
              List<Drone> drones = XMLTools.LoadListFromXMLSerializer<Drone>(DRONEPATH);
              if (drones.Any(dr => dr.Id == drone.Id))
                  throw new DroneException("id already exist");
              drones.Add(drone);
              XMLTools.SaveListToXMLSerializer(drones, DRONEPATH);
          }
          /// <summary>
          /// detele element
          /// </summary>
          /// <param name="id"></id element>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public void DeleteDrone(int id)
          {
              var drones = XMLTools.LoadListFromXMLSerializer<Drone>(DRONEPATH);
              if (!drones.Any(cos => cos.Id == id))
                  throw new DroneException($"Drone with {id} as Id does not exist");
              if (drones.Where(d => d.Id == id).FirstOrDefault().Valid == false)
                  throw new DroneException($"Drone with {id} as Id is alredy deleted");
              drones.RemoveAll(p => p.Id == id);
              XMLTools.SaveListToXMLSerializer(drones, DRONEPATH);
          }
          /// <summary>
          /// gets drone from database and return it to main
          /// </summary>
          /// <param name="id"></param>
          /// <returns></drone>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public Drone GetDrone(int id)
          {
              var drones = XMLTools.LoadListFromXMLSerializer<Drone>(DRONEPATH);
              if (!drones.Any(cos => cos.Id == id))
                  throw new DroneException($"Drone with {id} as Id does not exist");
              if (drones.Where(d => d.Id == id).FirstOrDefault().Valid == false)
                  throw new DroneException($"Drone with {id} as Id is already deleted");
              return drones.Find(dr => dr.Id == id);
          }
          /// <summary>
          /// func that returns list to print in console
          /// </summary>
          /// <returns></returns>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public IEnumerable<Drone> GetDroneList(Predicate<Drone> predicate)
          {
              var drones = XMLTools.LoadListFromXMLSerializer<Drone>(DRONEPATH);

              var d = (from item in drones
                       where predicate == null ? true : predicate(item) && item.Valid == true
                       select item);
              if (d == null)
                  throw new DroneException("empty list");
              return d.ToList();
          }
          /// <summary>
          /// update drone frome bl to data source
          /// </summary>
          /// <param name="dr"></param>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public void UpdateDrone(Drone dr)
          {
              var drones = XMLTools.LoadListFromXMLSerializer<Drone>(DRONEPATH);
              if (!drones.Any(drone => drone.Id == dr.Id))
              {
                  throw new DroneException($"Drone with {dr.Id} as Id does not exist");
              }
              if (drones.Where(d => d.Id == dr.Id).FirstOrDefault().Valid == false)
                  throw new DroneException($"Drone with {dr.Id} as Id is already deleted");
              drones.RemoveAll(d => d.Id == dr.Id);
              drones.Add(dr);
              XMLTools.SaveListToXMLSerializer(drones, DRONEPATH);
          }
          /// <summary>
          /// to set a time for when the drone pick's up a packet
          /// </summary>
          /// <param name="id"></param>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public void UpdateDronePickUp(int id)
          {
              var drones = XMLTools.LoadListFromXMLSerializer<Drone>(DRONEPATH);
              var parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(PARCELPATH);
              if (!drones.Any(cos => cos.Id == id))
              {
                  throw new DroneException($"Drone with {id} as Id does not exist");
              }
              if (drones.Where(d => d.Id == id).FirstOrDefault().Valid == false)
                  throw new DroneException($"Drone with {id} as Id is already deleted");
              int k = parcels.FindIndex(ps => ps.DroneId == id);
              if (k == -1)
                  throw new ParcelExeption("invalid parcel id");
              Parcel tmp = parcels[k];
              tmp.PickedUp = DateTime.Now;
              parcels[k] = tmp;
              XMLTools.SaveListToXMLSerializer(drones, DRONEPATH);
              XMLTools.SaveListToXMLSerializer(parcels, PARCELPATH);
          }
          /// <summary>
          /// to set a parcel to pickup
          /// </summary>
          /// <param name="id"></param>
          public void ParcelPickup(int parcelId)
          {
              var parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(PARCELPATH);
              var parcel = (from p in parcels
                            where p.Id == parcelId
                            select p).FirstOrDefault();


              int k = parcels.FindIndex(ps => ps.Id == parcelId);
              if (k == -1)
                  throw new ParcelExeption("invalid parcel id");
              Parcel tmp = parcels[k];
              tmp.PickedUp = DateTime.Now;
              parcels[k] = tmp;

              XMLTools.SaveListToXMLSerializer(parcels, PARCELPATH);
          }
          /// <summary>
          /// schedule a parcel to a drone
          /// </summary>
          /// <param name="parcelId"> id of the drone</param>
          /// <param name="droneId">id of the parcel</param>
          public void ParcelSchedule(int parcelId, int droneId)
          {
              var parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(PARCELPATH);
              var parcel = (from p in parcels
                            where p.Id == parcelId
                            select p).FirstOrDefault();
              int k = parcels.FindIndex(ps => ps.Id == parcelId);
              if (k == -1)
                  throw new ParcelExeption("invalid parcel id");
              Parcel tmp = parcels[k];
              tmp.DroneId = droneId;
              tmp.Scheduled = DateTime.Now;
              parcels[k] = tmp;

              XMLTools.SaveListToXMLSerializer(parcels, PARCELPATH);
          }
          /// <summary>
          /// to set a parcel to pickup
          /// </summary>
          /// <param name="id"> parcel id</param>
          public void ParcelDelivery(int parcelId)
          {
              var parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(PARCELPATH);
              var parcel = (from p in parcels
                            where p.Id == parcelId
                            select p).FirstOrDefault();


              int k = parcels.FindIndex(ps => ps.Id == parcelId);
              if (k == -1)
                  throw new ParcelExeption("invalid parcel id");
              Parcel tmp = parcels[k];
              tmp.Delivered = DateTime.Now;

              parcels[k] = tmp;

              XMLTools.SaveListToXMLSerializer(parcels, PARCELPATH);
          }
          /// <summary>
          /// send a drone to charge
          /// </summary>
          /// <param name="idD"></param>
          /// <param name="baseName"></param>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public void UpdateDroneToCharge(int idD, string baseName)
          {
              var drones = XMLTools.LoadListFromXMLSerializer<Drone>(PARCELPATH);
              var baseStations = XMLTools.LoadListFromXMLSerializer<BaseStation>(BASESTATIONPATH);
              var droneCharges = XMLTools.LoadListFromXMLSerializer<DroneCharge>(DRONECHARGEPATH);
              Drone? dr;
              dr = (from dro in drones
                    where dro.Valid == true && dro.Id == idD
                    select dro).FirstOrDefault();
              BaseStation? bs;
              bs = (from bas in baseStations
                    where bas.Valid == true && bas.Name == baseName
                    select bas).FirstOrDefault();
              if (dr == null)
                  throw new DroneException("id of drone not found");
              if (bs == null)
                  throw new BaseExeption("id of base station not found");
              if (bs.Value.NumOfSlots == 0)
                  throw new BaseExeption("base station already full");
              BaseStation b = (BaseStation)bs;
              b.NumOfSlots--;
              baseStations.RemoveAll(b => b.Id == bs.Value.Id);
              baseStations.Add(b);
              droneCharges.Add(new DroneCharge { DroneId = dr.Value.Id, StationId = b.Id });
              XMLTools.SaveListToXMLSerializer(droneCharges, DRONECHARGEPATH);
              XMLTools.SaveListToXMLSerializer(baseStations, BASESTATIONPATH);
          }
          /// <summary>
          /// method to release a charging drone from a base station
          /// </summary>
          /// <param name="idD"></param>
          /// <param name="baseName"></param>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public void UpdateReleasDroneCharge(int idD, string baseName)
          {
              var drones = XMLTools.LoadListFromXMLSerializer<Drone>(DRONEPATH);
              var baseStations = XMLTools.LoadListFromXMLSerializer<BaseStation>(BASESTATIONPATH);
              var droneCharges = XMLTools.LoadListFromXMLSerializer<DroneCharge>(DRONECHARGEPATH);
              Drone? dr = (from dro in drones
                           where dro.Valid == true && dro.Id == idD
                           select dro).FirstOrDefault();
              BaseStation? bs = (from bas in baseStations
                                 where bas.Valid == true && bas.Name == baseName
                                 select bas).FirstOrDefault();
              DroneCharge? charge = (from item in droneCharges
                                     where item.DroneId == idD && item.StationId == bs.Value.Id
                                     select item
                                    ).FirstOrDefault();
              if (dr == null)
                  throw new DroneException("id of drone not found");
              if (bs == null)
                  throw new BaseExeption("id of base station not found");
              if (charge == null)
                  throw new DroneChargeException("no such drone charging actually");
              BaseStation basest = (BaseStation)bs;
              basest.NumOfSlots++;
              baseStations.RemoveAll(b => b.Name == baseName);
              baseStations.Add(basest);
              droneCharges.RemoveAll(b => b.DroneId == idD && b.StationId == basest.Id);
              XMLTools.SaveListToXMLSerializer(droneCharges, DRONECHARGEPATH);
              XMLTools.SaveListToXMLSerializer(baseStations, BASESTATIONPATH);
          }

          #endregion

          #region Parcel
          /// <summary>
          /// send a new parcel to database
          /// </summary>
          /// <param name="parcel"></param>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public int AddParcel(Parcel parcel)
          {
              mySqlConnectionCommand.CommandText = $"SELECT * FROM Parcel WHERE Id={parcel.Id}";
              using SqlDataReader rdr = mySqlConnectionCommand.ExecuteReader();
              if (rdr.Read())
              {
                  throw new ParcelExeption("id already exist");
              }
              mySqlConnectionCommand.CommandText = $"";
              //need to be implemented on configs.xml
              var vs = XMLTools.LoadListFromXMLSerializer<double>(@"configs.xml");
              vs[5]++;
              XMLTools.SaveListToXMLSerializer(vs, @"configs.xml");
              return 1;
          }

          /// <summary>
          /// gets parcel from database and return it to main
          /// </summary>
          /// <param name="id"></param>
          /// <returns></the parcel got >
          [MethodImpl(MethodImplOptions.Synchronized)]
          public Parcel GetParcel(int id)
          {
              var parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(@"parcels.xml");
              if (!parcels.Any(pa => pa.Id == id))
              {
                  throw new ParcelExeption($"Parcel with {id} as Id does not exist");
              }
              return parcels.Find(dr => dr.Id == id);
          }
          /// <summary>
          /// 
          /// </summary>
          /// <param name="id"></param>
          /// <returns></returns>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public void UpdateParcelToDrone(int idP, int idD)
          {
              var parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(@"parcels.xml");
              List<Drone> drones = XMLTools.LoadListFromXMLSerializer<Drone>(@"drones.xml");
              if (!parcels.Any(pa => pa.Id == idP))
              {
                  throw new ParcelExeption($"Parcel with {idP} as Id does not exist");
              }
              if (!drones.Any(d => d.Id == idD))
              {
                  throw new DroneException($" Id {idD} of drone not found\n");
              }
              if (drones.Where(d => d.Id == idD).FirstOrDefault().Valid == false)
                  throw new DroneException($"Drone with {idD} as Id is already deleted");
              int i = parcels.FindIndex(ps => ps.Id == idP);
              Parcel myParcel = parcels.Find(ps => ps.Id == idP);
              myParcel.DroneId = idD;
              myParcel.Requested = DateTime.Now;

              parcels[i] = myParcel;
              XMLTools.SaveListToXMLSerializer(parcels, @"parcels.xml");
          }
          /// <summary>
          /// method that sets the delivery time 
          /// </summary>
          /// <param name="id"></param>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public void UpdatesParcelDelivery(int id)
          {
              List<Parcel> parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(@"parcels.xml");
              int k = parcels.FindIndex(ps => ps.DroneId == id);
              if (k == -1)
                  throw new ParcelExeption($"invalid parcel id {id}");
              Parcel tmp = parcels[k];
              tmp.Delivered = DateTime.Now;
              tmp.DroneId = 0;

              parcels[k] = tmp;
              XMLTools.SaveListToXMLSerializer(parcels, @"parcels.xml");
          }
          /// <summary>
          /// func that returns list to print in console
          /// </summary>
          /// <returns></returns>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public IEnumerable<Parcel> GetParcelList(Predicate<Parcel> predicate)
          {
              List<Parcel> parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(@"parcels.xml");

              IEnumerable<Parcel> p = (from item in parcels
                                       where predicate == null ? true : predicate(item)
                                       select item);
              if (p == null)
                  throw new ParcelExeption("empty list");
              return p.ToList();
          }


          /// <summary>
          /// update a parcel
          /// </summary>
          /// <param name="p"></param>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public void UpdateParcel(Parcel p)
          {
              List<Parcel> parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(@"parcels.xml");
              int index = parcels.FindIndex(pc => pc.Id == p.Id);
              if (index == -1)
                  throw new ParcelExeption($"the parcel {p.Id} doesn't exists");
              parcels[index] = p;

              XMLTools.SaveListToXMLSerializer(parcels, @"parcels.xml");
          }
          /// <summary>
          /// detele element
          /// </summary>
          /// <param name="id"></id element>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public void DeleteParcel(int id)
          {
              var parcels = XMLTools.LoadListFromXMLSerializer<Parcel>(@"parcels.xml");
              if (!parcels.Any(p => p.Id == id))
                  throw new ParcelExeption($"parcel with {id}as Id does not exist");
              parcels.RemoveAll(p => p.Id == id);
              XMLTools.SaveListToXMLSerializer(parcels, @"parcels.xml");
          }
          #endregion

          #region BaseStation

          /// <summary>
          /// gets basestation from database and return it to main
          /// </summary>
          /// <param name="id"></param>
          /// <returns></returns the basestation got>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public BaseStation GetBaseStation(int id)
          {
              XElement baseRoot = XMLTools.LoadListFromXMLElement(BASESTATIONPATH);
              BaseStation? bs;
              try
              {
                  bs = (from basestation in baseRoot.Elements()
                        where (basestation.Element("valid").Value) == "true"
                        where int.Parse(basestation.Element("id").Value) == id
                        select new BaseStation()
                        {
                            Id = int.Parse(basestation.Element("id").Value),
                            Latitude = double.Parse(basestation.Element("latitude").Value),
                            Longitude = double.Parse(basestation.Element("longitude").Value),
                            Name = basestation.Element("name").Value,
                            NumOfSlots = int.Parse(basestation.Element("numOfSlots").Value),
                            Valid = bool.Parse(basestation.Element("valid").Value)
                        }).FirstOrDefault();
              }
              catch
              {
                  bs = null;
              }
              if (bs == null)
                  throw new BaseExeption("id of base not found or deleted");
              return (BaseStation)bs;
          }

          /// <summary>
          /// send a new base to database by XElement
          /// </summary>
          /// <param name="baseStation"></param>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public void AddBaseStation(BaseStation baseStation)
          {
              XElement baseRoot = XMLTools.LoadListFromXMLElement(BASESTATIONPATH);
              BaseStation? bs;
              try
              {
                  bs = (from basestation in baseRoot.Elements()
                        where int.Parse(basestation.Element("id").Value) == baseStation.Id
                        select new BaseStation()
                        {
                            Id = int.Parse(basestation.Element("id").Value),
                            Latitude = double.Parse(basestation.Element("latitude").Value),
                            Longitude = double.Parse(basestation.Element("longitude").Value),
                            Name = basestation.Element("name").Value,
                            NumOfSlots = int.Parse(basestation.Element("numOfSlots").Value),
                            Valid = bool.Parse(basestation.Element("valid").Value)
                        }).FirstOrDefault();
              }
              catch
              {
                  bs = null;
              }
              if (bs == null)
              {
                  XElement Id = new XElement("Id", baseStation.Id);
                  XElement Latitude = new XElement("Latitude", baseStation.Latitude);
                  XElement Longitude = new XElement("Longitude", baseStation.Longitude);
                  XElement Name = new XElement("Name", baseStation.Name);
                  XElement NumOfSlots = new XElement("NumOfSlots", baseStation.NumOfSlots);
                  XElement Valid = new XElement("Valid", baseStation.Valid);
                  baseRoot.Add(new XElement("baseStation", Id, Name, NumOfSlots, Latitude, Longitude, Valid));
                  XMLTools.SaveListToXMLElement(baseRoot, BASESTATIONPATH);
              }
              throw new BaseExeption("id already exists");

          }
          /// <summary>
          /// get list of base stations
          /// </summary>
          /// <returns></returns>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public IEnumerable<BaseStation> GetBaseStationsList(Predicate<BaseStation> predicat)
          {
              XElement baseRoot = XMLTools.LoadListFromXMLElement(BASESTATIONPATH);
              IEnumerable<BaseStation> b = from bas in baseRoot.Elements()

                                           let da = new BaseStation
                                           {
                                               Id = int.Parse(bas.Element("id").Value),
                                               Latitude = double.Parse(bas.Element("latitude").Value),
                                               Longitude = double.Parse(bas.Element("longitude").Value),
                                               Name = bas.Element("name").Value,
                                               NumOfSlots = int.Parse(bas.Element("numOfSlots").Value),
                                               Valid = bool.Parse(bas.Element("valid").Value)
                                           }
                                           where predicat == null ? true : predicat(da)
                                       && da.Valid == true
                                           select da;
              if (b == null)
              {
                  throw new CostumerExeption("empty list");
              }
              return b.ToList();

          }
          /// <summary>
          /// update in dal a basestation
          /// </summary>
          /// <param name="bs"></param>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public void UpdateBaseStationFromBl(BaseStation bs)
          {
              XElement baseRoot = XMLTools.LoadListFromXMLElement(BASESTATIONPATH);
              BaseStation? bas;
              // first we search for old base station in the list
              try
              {
                  bas = (from basestation in baseRoot.Elements()
                         where (basestation.Element("valid").Value) == "true"
                         where int.Parse(basestation.Element("id").Value) == bs.Id
                         select new BaseStation()
                         {
                             Id = int.Parse(basestation.Element("id").Value),
                             Latitude = double.Parse(basestation.Element("latitude").Value),
                             Longitude = double.Parse(basestation.Element("longitude").Value),
                             Name = basestation.Element("name").Value,
                             NumOfSlots = int.Parse(basestation.Element("numOfSlots").Value),
                             Valid = bool.Parse(basestation.Element("valid").Value)
                         }).FirstOrDefault();

              }
              catch
              {
                  bas = null;
              }
              // if the base station doesn't exist we throw exception
              if (bas == null)
                  throw new BaseExeption("id of base not found");
              // we delete old base station with same id from the list
              XElement xElement = (from basestation in baseRoot.Elements()
                                   where (basestation.Element("valid").Value) == "true"
                                   where int.Parse(basestation.Element("id").Value) == bs.Id
                                   select basestation).FirstOrDefault();
              xElement.Remove();

              // then we add updated base station to the list and update the file
              XElement Id = new XElement("id", bs.Id);
              XElement Latitude = new XElement("latitude", bs.Latitude);
              XElement Longitude = new XElement("longitude", bs.Longitude);
              XElement Name = new XElement("name", bs.Name);
              XElement NumOfSlots = new XElement("numOfSlots", bs.NumOfSlots);
              XElement Valid = new XElement("valid", bs.Valid);
              baseRoot.Add(new XElement("baseStation", Id, Name, NumOfSlots, Latitude, Longitude, Valid));
              XMLTools.SaveListToXMLElement(baseRoot, BASESTATIONPATH);
          }
          /// <summary>
          /// return the drones charging on a base
          /// </summary>
          /// <param name="idBase"></param>
          /// <returns>list of drones</returns>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public IEnumerable<DroneCharge> GetDroneCharges(int idBase = 0)
          {
              if (idBase == 0)
                  return XMLTools.LoadListFromXMLSerializer<DroneCharge>(DRONECHARGEPATH);
              else
                  return XMLTools.LoadListFromXMLSerializer<DroneCharge>(DRONECHARGEPATH).Where(b => b.StationId == idBase);
          }
          /// <summary>
          /// delete base station XElement
          /// </summary>
          /// <param name="id"></param>
          [MethodImpl(MethodImplOptions.Synchronized)]
          public void DeleteBasestation(int id)
          {
              XElement baseRoot = XMLTools.LoadListFromXMLElement(BASESTATIONPATH);
              BaseStation? bas;
              // first we search for old base station in the list
              try
              {
                  bas = (from basestation in baseRoot.Elements()
                         where (basestation.Element("valid").Value) == "true"
                         where int.Parse(basestation.Element("id").Value) == id
                         select new BaseStation()
                         {
                             Id = int.Parse(basestation.Element("id").Value),
                             Latitude = double.Parse(basestation.Element("latitude").Value),
                             Longitude = double.Parse(basestation.Element("longitude").Value),
                             Name = basestation.Element("name").Value,
                             NumOfSlots = int.Parse(basestation.Element("numOfSlots").Value),
                             Valid = bool.Parse(basestation.Element("valid").Value)
                         }).FirstOrDefault();
              }
              catch
              {
                  bas = null;
              }
              // if the base station doesn't exist we throw exception
              if (bas == null)
                  throw new BaseExeption("id of base not found");
              // we delete old base station with same id from the list
              XElement xElement = (from basestation in baseRoot.Elements()
                                   where (basestation.Element("valid").Value) == "true"
                                   where int.Parse(basestation.Element("id").Value) == id
                                   select basestation).FirstOrDefault();
              xElement.Element("valid").Value = "false";
              XMLTools.SaveListToXMLElement(baseRoot, BASESTATIONPATH);
          }
          #endregion
      }*/


    }


}
