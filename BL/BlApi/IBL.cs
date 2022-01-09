using System;
using System.Collections.Generic;
using BO;

namespace BlApi
{
    public interface IBL
    {
        void AddStation(BaseStation addBaseStation);

        void AddDrone(Drone addDrone, int stationId);

        void AddCustomer(Customer addCustomer);

        void AddParcel(Parcel addParcel);

        void SetDroneName(BO.Drone droneToUpdate);

        void SetStationDetails(int stationId, string stationName = "", string allChargeSlots = "");

        void SetCustomerDetails(BO.Customer UpdatCustomer);

        void SetSendDroneToCharge(int droneId);

        void SetReleaseDroneFromCharge(int droneId, TimeSpan ChargingTime);

        void UpdateScheduleParcel(int droneId);

        void UpdatePickUpParcel(int droneId);

        void UpdateDeliveryToCustomer(int droneId);

        BO.BaseStation GetStation(int stationId);

        BO.Drone GetDrone(int droneId);

        BO.Customer GetCustomer(int customerId);

        BO.Parcel GetParcel(int parcelId);

        IEnumerable<BO.BaseStationToList> GetStationList(Func<DO.Station, bool> predicat = null);

        IEnumerable<BO.DroneToList> GetDroneList(Func<DroneToList, bool> predicat = null);

        IEnumerable<BO.CustomerToList> GetCustomerList(Func<DO.Customer, bool> predicat = null);

        IEnumerable<BO.ParcelToList> GetParcelList(Func<DO.Parcel, bool> predicat = null);

        IEnumerable<ParcelToList> GetNotScheduleParcelList();

        IEnumerable<BO.BaseStationToList> GetFreeChargeStationList();
    }
}
