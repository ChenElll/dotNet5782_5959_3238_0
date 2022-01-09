using System;
using System.Collections.Generic;
using DO;


namespace DalApi
{
    public interface IDal
    {
        //--------------------------------------------- ADDING FUNCTIONS -------------------------------------------------
        /// <summary>
        /// add astation to the system
        /// </summary>
        /// <param name="stationToAdd"></param>
        void AddStation(Station stationToAdd);


        /// <summary>
        /// add a drone to the system
        /// </summary>
        /// <param name="droneToAdd"></param>
        void AddingDrone(Drone droneToAdd);


        /// <summary>
        /// add a customer to the system
        /// </summary>
        /// <param name="customerToAdd"></param>
        void AddingCustomer(Customer customerToAdd);


        /// <summary>
        /// add a parcel to the system
        /// </summary>
        /// <param name="parcelToAdd"></param>
        /// <returns></returns>
        int AddingParcel(Parcel parcelToAdd);

        //---------------------------------------------- UPDATE FUNCTIONS ---------------------------------------------------

        /// <summary>
        /// update station's details
        /// </summary>
        /// <param name="updatedStation"></param>
        void UpdateStation(Station updatedStation);

        /// <summary>
        /// update customer's details
        /// </summary>
        /// <param name="updatedCustomer"></param>
        void UpdateCustomer(Customer updatedCustomer);

        /// <summary>
        /// update drone's details
        /// </summary>
        /// <param name="updatedDrone"></param>
        void UpdateDrone(Drone updatedDrone);

        /// <summary>
        /// update parcel's details
        /// </summary>
        /// <param name="updatedParcel"></param>
        void UpdateParcel(Parcel updatedParcel);



        /// <summary>
        ///  update parcel's scheduled 
        /// </summary>
        /// <param name="MyDroneId"></param>
        /// <param name="MyParcelId"></param>
        void UpdateSchdule(int MyDroneId, int MyParcelId);


        /// <summary>
        /// update parcel's picked up by drone
        /// </summary>
        /// <param name="MyParcelId"></param>
        void UpdatePickUp(int MyParcelId);


        /// <summary>
        /// update parcel's delivery to customer
        /// </summary>
        /// <param name="MyParcelId"></param>
        void UpdateDelivery(int MyParcelId);

        /// <summary>
        /// update drone that is sent to charge
        /// </summary>
        /// <param name="MyDrone"></param>
        /// <param name="MyStation"></param>
        public void UpdateDroneChargeCheckIn(int MyDrone, int MyStation);

        /// <summary>
        /// update drone that is released from charge 
        /// </summary>
        /// <param name="MyDrone"></param>
        /// <param name="MyStation"></param>
        public void UpdateDroneChargeCheckout(int MyDrone, int MyStation);


        //----------------------------------------------- GET \ VIEW OPTIONS ---------------------------------------------
        /// <summary>
        /// station's view options
        /// </summary>
        /// <param name="StationId"></param>
        /// <returns></returns>
        public Station GetStation(int StationId);

        /// <summary>
        /// drone's view options
        /// </summary>
        /// <param name="DroneId"></param>
        /// <returns></returns>
        public Drone GetDrone(int DroneId);


        /// <summary>
        /// customer's view options
        /// </summary>
        /// <param name="CustomerId"></param>
        /// <returns> customer </returns>
        public Customer GetCustomer(int CustomerId);


        /// <summary>
        /// parcel's view options
        /// </summary>
        /// <param name="ParcelId"></param>
        public Parcel GetParcel(int ParcelId);


        //----------------------------------------------- GET \ VIEW LIST OPTIONS ---------------------------------------------


        /// <summary>
        /// view list of all the drone
        /// </summary>
        /// <returns>list of all the drones</returns>
        public IEnumerable<Drone> GetDronesList(Func<Drone, bool> predicat = null);

        /// <summary>
        /// view list of all drone's charge
        /// </summary>
        /// <returns>list of all drone's charge</returns>
        public IEnumerable<DroneCharge> GetDroneChargesList(Func<DroneCharge, bool> predicat = null);

        /// <summary>
        /// view list of all the parcels
        /// </summary>
        /// <returns>list of all the parcels</returns>
        public IEnumerable<Parcel> GetParcelsList(Func<Parcel, bool> predicat = null);

        ///// <summary>
        ///// view of parcels that were not attribute\scheduled
        ///// </summary>
        ///// <returns>list of parcels that were not attribute</returns>
        //public IEnumerable<Parcel> GetNotAttributeParcelsList();


        /// <summary>
        /// view list of all the customers
        /// </summary>
        /// <returns>list of all the customers</returns>
        public IEnumerable<Customer> GetCustomersList(Func<Customer, bool> predicat = null);

        /// <summary>
        /// view list of all the stations
        /// </summary>
        /// <returns>list of all the stations</returns>
        public IEnumerable<Station> GetStationsList(Func<Station, bool> predicat = null);

        ///// <summary>
        ///// view of all the free charge slots in the stations
        ///// </summary>
        ///// <returns>list of stations that have free charge slots</returns>
        //public IEnumerable<Station> GetListFreeChargeStationList();

        /// <summary>
        /// returns the range that charged by the drone
        /// </summary>
        /// <returns>array that contain the configurations of drones charging</returns>
        double[] getElectricityUseByDrone();
    }
}
