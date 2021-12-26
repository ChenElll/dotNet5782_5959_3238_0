using System;
using DalApi;
using Dal.DO;
using System.Collections.Generic;
using System.Linq;

namespace Dal
{
    sealed class DalObject : IDal
    {
        static readonly IDal instance = new DalObject();
        public static IDal Instance { get => instance; }

        public DalObject()
        {
            DataSource.Initialize();
        }
        #region DATASOURCE
        static class DataSource
        {
            internal static List<Drone> DronesList = new List<Drone>();

            internal static List<Station> StationsList = new List<Station>();

            internal static List<Customer> CustomersList = new List<Customer>();

            internal static List<Parcel> ParcelsList = new List<Parcel>();

            internal static List<DroneCharge> DroneChargesList = new List<DroneCharge>();

            internal static Random MyRandom = new Random(DateTime.Now.Millisecond);


            public class Config
            {
                internal static int ParcelsNumber = 25439;  // the number of the last initialize's parcel

                static public double available = 0.01;
                static public double lightWeightCarry = 0.02;
                static public double mediumWeightCarry = 0.04;
                static public double heavyWeightCarry = 0.08;
                static public double droneChargeRange = 0.1;
            }

            /// <summary>
            /// initialize the program with drones
            /// </summary>        
            public static void Initialize()
            {
                StationsList.Add(
                    new Station
                    {
                        Id = 1,
                        Name = "NorthJerusalem",
                        Longtitude = 31.87,
                        Lattitude = 35.1695,
                        FreeChargeSlots = MyRandom.Next(4),
                    });

                StationsList.Add(
                        new Station
                        {
                            Id = 3,
                            Name = "SouthJerusalem",
                            Longtitude = 31.709,
                            Lattitude = 35.1695,
                            FreeChargeSlots = MyRandom.Next(4),
                        });

                for (int i = 0; i < 5; i++) // initializing the drone's list with 5 drones
                {

                    DronesList.Add(
                        new Drone
                        {
                            Id = MyRandom.Next(1000, 10000),
                            Model = ("Drone" + i),
                            MaxWeight = (WeightCategories)MyRandom.Next(1, 4)
                        });
                }


                string[] CustomersNames = new string[10] { "Abby", "Adam", "Roy", "Dan", "Shon", "John", "James", "Robert", "Mary", "Sarah" };
                for (int i = 0; i < 10; i++) // initialize the customer's list with ten customers
                {
                    CustomersList.Add(
                        new Customer
                        {
                            Id = MyRandom.Next(100000000, 1000000000),
                            CustomerName = CustomersNames[i],
                            PhoneNumber = $"0{MyRandom.Next(50, 59)}{MyRandom.Next(1000000, 10000000)}",
                            Lattitude = 35.1052 + MyRandom.Next(2, 15) / 100,
                            Longtitude = 31.7082 + MyRandom.Next(17) / 100,
                        });
                }

                for (int i = 0; i < 10; i++) // initialize the parcel's list with ten parcels
                {
                    Config.ParcelsNumber++;

                    ParcelsList.Add(
                        new Parcel
                        {
                            Id = Config.ParcelsNumber, // each parcel has to be bigger than the previous
                            SenderId = CustomersList[MyRandom.Next(5)].Id,
                            // calcul to avoid a situation where the sender send to himself
                            TargetId = CustomersList[MyRandom.Next(5, 10)].Id,
                            // verify that the parcel weighs no more than the drone
                            Priority = (Priorities)MyRandom.Next(1, 4),
                            Weight = (WeightCategories)MyRandom.Next(1, 4),
                            DroneId = 0,
                            RequestedTime = DateTime.Now,
                        });
                }
            }

        }
        #endregion


        #region ---------------------------------------DRONE------------------------------------------


        #region AddDrone
        /// <summary>
        /// add a drone to the system
        /// </summary>
        /// <param name="droneToAdd"></param>
        public void AddingDrone(Drone droneToAdd)
        {
            if (DataSource.DronesList.Exists(x => x.Id == droneToAdd.Id))
                throw new AlreadyExistException("The drone already exist in the system");
            //add a drone to drone's list
            DataSource.DronesList.Add(
                new Drone
                {
                    Id = droneToAdd.Id,
                    Model = droneToAdd.Model,
                    MaxWeight = droneToAdd.MaxWeight,
                });
        }
        #endregion


        #region UpdateDrone
        /// <summary>
        /// update drone id,model,weight
        /// </summary>
        /// <param name="updatedDrone"></param>
        public void UpdateDrone(Drone updatedDrone)
        {

            int droneIndex = DataSource.DronesList.FindIndex(d => d.Id == updatedDrone.Id);
            if (droneIndex == -1)
                throw new DoesntExistException("Error the drone to update doesn't exis in the system");
            DataSource.DronesList[droneIndex] =
                new Drone
                {
                    Id = updatedDrone.Id,
                    Model = updatedDrone.Model,
                    MaxWeight = updatedDrone.MaxWeight,
                };
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
            Drone drone = DataSource.DronesList.FirstOrDefault(x => x.Id == DroneId);// finding the drone by id number
                                                                                     // if didn't find it throw an Exeption
            if (drone.Id == default)
                throw new DoesntExistException("This drone doesn't exist in the system");
            return drone;   // if found it return the drone
        }
        #endregion


        #region GetDronesList
        /// <summary>
        /// get the lidt of the drones
        /// </summary>
        /// <returns>list of drone</returns>
        public IEnumerable<Drone> GetDronesList()
        {
            var list = new List<Drone>();
            foreach (Drone item in DataSource.DronesList)
            {
                list.Add(item);
            }
            return list;
            //return from item in DataSource.DronesList
            //       orderby item.Id
            //       select new Drone()
            //       {
            //           Id = item.Id,
            //           Model = item.Model,
            //           MaxWeight = item.MaxWeight
            //       };
        }
        #endregion


        #region GetDroneChargesList
        /// <summary>
        /// get the list of drone in charge
        /// </summary>
        /// <returns>list of  drones in charge</returns>
        public IEnumerable<DroneCharge> GetDroneChargesList()
        {
            return from item in DataSource.DroneChargesList
                   orderby item.DroneId
                   select new DroneCharge()
                   {
                       DroneId = item.DroneId,
                       StationId = item.StationId,
                       EntranceTime = item.EntranceTime,
                       LeavingTime = item.LeavingTime
                   };

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
            int stationIndex = DataSource.StationsList.FindIndex(x => x.Id == MyStation); //search station in station's list and update charge slots 
            if (stationIndex < 0)
                throw new DoesntExistException("the station doesn't exist in the system");

            DataSource.DroneChargesList.Add // add charge to drone's charge's list
                (
                new DroneCharge
                {
                    StationId = MyStation,
                    DroneId = MyDrone,
                    EntranceTime = DateTime.Now,

                });
            Station tempStation = DataSource.StationsList[stationIndex];
            tempStation.FreeChargeSlots--;
            DataSource.StationsList[stationIndex] = tempStation;

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
            //finding the place in drone's charge's list where the drone we need to release is
            int chargeIndex = DataSource.DroneChargesList.FindIndex(x => x.DroneId == MyDrone);
            if (chargeIndex < 0)
                throw new DoesntExistException("the drone doesn't exist in the system");

            //search station in station's list and update charge slots
            int stationIndex = DataSource.StationsList.FindIndex(x => x.Id == MyStation);
            if (stationIndex < 0)
                throw new DoesntExistException("the station doesn't exist in the system");

            Station tempStation = DataSource.StationsList[stationIndex];
            tempStation.FreeChargeSlots++;
            DataSource.StationsList[stationIndex] = tempStation;

            DataSource.DroneChargesList.Remove(DataSource.DroneChargesList[chargeIndex]);

        }
        #endregion


        #region getRangeChargeByDrone
        /// <summary>
        /// get the request of use of electricity
        /// </summary>
        /// <returns></returns>
        public double[] getElectricityUseByDrone()
        {
            double[] electricityUseByDrone = { DataSource.Config.available, DataSource.Config.lightWeightCarry,
            DataSource.Config.mediumWeightCarry, DataSource.Config.heavyWeightCarry, DataSource.Config.droneChargeRange};

            return electricityUseByDrone;
        }
        #endregion




        #endregion
        
        
        #region --------------------------------------STATION-----------------------------------------

        #region AddStation
        /// <summary>
        /// add a station to the system
        /// </summary>
        /// <param name="stationToAdd"></param>
        public void AddStation(Station stationToAdd)
        {
            if (DataSource.StationsList.Exists(x => x.Id == stationToAdd.Id))
                throw new AlreadyExistException("The station already exist in the system");

            // add a new station to station's list
            DataSource.StationsList.Add(
                     new Station()
                     {
                         Id = stationToAdd.Id,
                         Name = stationToAdd.Name,
                         Longtitude = stationToAdd.Longtitude,
                         Lattitude = stationToAdd.Lattitude,
                         FreeChargeSlots = stationToAdd.FreeChargeSlots,
                     });
        }
        #endregion


        #region UpdateStation
        /// <summary>
        /// update station id, name, longtitude,lattitude,free charge slots
        /// </summary>
        /// <param name="updatedStation"></param>
        public void UpdateStation(Station updatedStation)
        {
            int stationIndex = DataSource.StationsList.FindIndex(s => s.Id == updatedStation.Id);
            if (stationIndex == -1)
                throw new DoesntExistException("Error the station to update doesn't exis in the system");

            DataSource.StationsList[stationIndex] =
            new Station
            {
                Id = updatedStation.Id,
                Name = updatedStation.Name,
                Lattitude = updatedStation.Lattitude,
                Longtitude = updatedStation.Longtitude,
                FreeChargeSlots = updatedStation.FreeChargeSlots,
            };
        }
        #endregion


        #region GetStation
        public Station GetStation(int StationId) // the function gets the id number of the station
        {
            // the function search the station in station's list
            Station station = DataSource.StationsList.Find(x => x.Id == StationId);
            // if the station was not found throw an exeption 
            if (station.Id == 0)
                throw new DoesntExistException("This station doesn't exist in the system");
            // if the station was found return the station
            return station;
        }
        #endregion


        #region GetStationsList
        /// <summary>
        /// get the list of the station
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Station> GetStationsList()
        {
            var list = from item in DataSource.StationsList
                       orderby item.Id
                       select new Station()
                       {
                           Id = item.Id,
                           Name = item.Name,
                           FreeChargeSlots = item.FreeChargeSlots,
                           Lattitude = item.Lattitude,
                           Longtitude = item.Longtitude
                       };
            return list;
        }
        #endregion


        #region GetListFreeChargeStation
        /// <summary>
        /// get list of free charge station
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Station> GetListFreeChargeStationList()
        {
            return from item in DataSource.StationsList.ListFilter(P => P.FreeChargeSlots > 0)
                   orderby item.Id
                   select new Station()
                   {
                       Id = item.Id,
                       Name = item.Name,
                       FreeChargeSlots = item.FreeChargeSlots,
                       Lattitude = item.Lattitude,
                       Longtitude = item.Longtitude
                   };

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

            DataSource.ParcelsList.Add(
               new Parcel
               {
                   Id = ++DataSource.Config.ParcelsNumber, // מעלה את המספר הרץ של החבילות ומעדכן אותו בחבילה החדשה
                   SenderId = parcelToAdd.SenderId,
                   TargetId = parcelToAdd.TargetId,
                   Weight = parcelToAdd.Weight,
                   Priority = parcelToAdd.Priority,
                   DroneId = parcelToAdd.DroneId,
                   RequestedTime = parcelToAdd.RequestedTime,
                   ScheduledTime = parcelToAdd.ScheduledTime,
                   PickedUpTime = parcelToAdd.PickedUpTime,
                   DeliveredTime = parcelToAdd.DeliveredTime
               });
            return DataSource.ParcelsList.Last().Id;
        }
        #endregion


        #region UpdateParcel
        /// <summary>
        /// update parcel's details
        /// </summary>
        /// <param name="updatedParcel"></param>
        public void UpdateParcel(Parcel updatedParcel)
        {
            int parcelIndex = DataSource.ParcelsList.FindIndex(d => d.Id == updatedParcel.Id);
            if (parcelIndex == -1)
                throw new DoesntExistException("Error the parcel to update doesn't exis in the system");
            DataSource.ParcelsList[parcelIndex] =
            new Parcel
            {
                Id = updatedParcel.Id,
                Weight = updatedParcel.Weight,
                SenderId = updatedParcel.SenderId,
                TargetId = updatedParcel.TargetId,
                DroneId = updatedParcel.DroneId,
                Priority = updatedParcel.Priority,
                ScheduledTime = updatedParcel.ScheduledTime,
                RequestedTime = updatedParcel.RequestedTime,
                DeliveredTime = updatedParcel.DeliveredTime,
                PickedUpTime = updatedParcel.PickedUpTime,
            };
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
            // the function search the parcel in parcel's list
            Parcel parcel = DataSource.ParcelsList.Find(d => d.Id == ParcelId);
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
        public IEnumerable<Parcel> GetParcelsList()
        {
            return from item in DataSource.ParcelsList
                   orderby item.Id
                   select new Parcel()
                   {
                       Id = item.Id,
                       DroneId = item.DroneId,
                       SenderId = item.SenderId,
                       TargetId = item.TargetId,
                       Priority = item.Priority,
                       Weight = item.Weight,
                       RequestedTime = item.RequestedTime,
                       ScheduledTime = item.ScheduledTime,
                       PickedUpTime = item.PickedUpTime,
                       DeliveredTime = item.DeliveredTime
                   };

        }
        #endregion


        #region GetListNotAttributeParcel
        /// <summary>
        /// gets all the unscheduled parcels
        /// </summary>
        /// <returns></returns>
        public IEnumerable<Parcel> GetNotAttributeParcelsList()
        {
            return from item in DataSource.ParcelsList.ListFilter(P => P.DroneId == 0)
                   select new Parcel()
                   {
                       Id = item.Id,
                       DroneId = item.DroneId,
                       SenderId = item.SenderId,
                       TargetId = item.TargetId,
                       Priority = item.Priority,
                       Weight = item.Weight,
                       RequestedTime = item.RequestedTime,
                       ScheduledTime = item.ScheduledTime,
                       PickedUpTime = item.PickedUpTime,
                       DeliveredTime = item.DeliveredTime
                   };
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
            int ParcelIndex = DataSource.ParcelsList.FindIndex(x => x.Id == MyParcelId); // finding the parcel by id number
            if (ParcelIndex == -1)
                throw new DoesntExistException("This parcel doesn't exist in the system");

            int DroneIndex = DataSource.DronesList.FindIndex(x => x.Id == MyDroneId);
            if (DroneIndex == -1)
                throw new DoesntExistException("This drone doesn't exist in the system");

            // update the parcel
            if (ParcelIndex >= 0)
            {
                Parcel tempParcel = DataSource.ParcelsList[ParcelIndex];
                tempParcel.DroneId = MyDroneId;
                tempParcel.ScheduledTime = DateTime.Now;
                DataSource.ParcelsList[ParcelIndex] = tempParcel;
            }

        }
        #endregion


        #region UpdatePickUp
        /// <summary>
        /// update parcel to pick up
        /// </summary>
        /// <param name="MyParcelId"></param>
        public void UpdatePickUp(int MyParcelId)
        {

            int ParcelIndex = DataSource.ParcelsList.FindIndex(x => x.Id == MyParcelId); // finding the parcel by id number
            // updates the pacel 
            if (ParcelIndex == -1)
                throw new DoesntExistException("This parcel doesn't exist in the system");

            if (ParcelIndex >= 0)
            {
                Parcel tempParcel = DataSource.ParcelsList[ParcelIndex];
                tempParcel.PickedUpTime = DateTime.Now;
                DataSource.ParcelsList[ParcelIndex] = tempParcel;

            }

        }
        #endregion


        #region UpdateDelivery
        /// <summary>
        /// update parcel delivery
        /// </summary>
        /// <param name="MyParcelId"></param>
        public void UpdateDelivery(int MyParcelId)
        {
            int ParcelIndex = DataSource.ParcelsList.FindIndex(x => x.Id == MyParcelId); // finding the parcel by id number
                                                                                         // updates the pacel 
            if (ParcelIndex == -1)
                throw new DoesntExistException("This parcel doesn't exist in the system");
            if (ParcelIndex >= 0)
            {
                Parcel tempParcel = DataSource.ParcelsList[ParcelIndex];
                tempParcel.DeliveredTime = DateTime.Now;
                DataSource.ParcelsList[ParcelIndex] = tempParcel;

            }
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
            //add a new customer to customer's list
            if (DataSource.CustomersList.Exists(x => x.Id == customerToAdd.Id))
                throw new AlreadyExistException("The customer already exist in the system");

            DataSource.CustomersList.Add(
                new Customer
                {
                    Id = customerToAdd.Id,
                    CustomerName = customerToAdd.CustomerName,
                    PhoneNumber = customerToAdd.PhoneNumber,
                    Longtitude = customerToAdd.Longtitude,
                    Lattitude = customerToAdd.Lattitude,
                });
        }
        #endregion


        #region UpdateCustomer
        /// <summary>
        /// update customer details
        /// </summary>
        /// <param name="updatedCustomer"></param>
        public void UpdateCustomer(Customer updatedCustomer)
        {
            int customerIndex = DataSource.CustomersList.FindIndex(s => s.Id == updatedCustomer.Id);
            if (customerIndex == -1)
                throw new DoesntExistException("Error the customer to update doesn't exis in the system");
            DataSource.CustomersList[customerIndex] =
            new Customer
            {
                Id = updatedCustomer.Id,
                CustomerName = updatedCustomer.CustomerName,
                Lattitude = updatedCustomer.Lattitude,
                Longtitude = updatedCustomer.Longtitude,
                PhoneNumber = updatedCustomer.PhoneNumber,
            };
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
            // the function search the customer in customer's list
            Customer customer = DataSource.CustomersList.Find(x => x.Id == CustomerId);
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
        public IEnumerable<Customer> GetCustomersList()
        {
            return from item in DataSource.CustomersList
                   orderby item.CustomerName
                   select new Customer()
                   {
                       Id = item.Id,
                       CustomerName = item.CustomerName,
                       PhoneNumber = item.PhoneNumber,
                       Lattitude = item.Lattitude,
                       Longtitude = item.Longtitude,
                   };
        }
        #endregion

        #endregion

    }

}


