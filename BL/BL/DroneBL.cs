using BO;
using DO;
using System;
using System.Collections.Generic;
using System.Linq;
using BlApi;


namespace BL
{
    public partial class BL : IBL
    {

        #region AddDrone

        /// <summary>
        /// the function add a drone 
        /// </summary>
        /// <param name="droneToAdd"></param>
        /// <param name="stationId"></param>
        public void AddDrone(BO.Drone droneToAddBO, int stationId)
        {
            try
            {
                //check that the station id to add is not negative
                if (stationId < 0)
                    throw new InvalidInputException("Station id cannot be negative");

                //update drone's details in data base
                dal.AddingDrone((DO.Drone)droneToAddBO.CopyPropertiesToNew(typeof(DO.Drone)));

                //search requested station in the list of stations 
                BO.BaseStation requestedStation
                    = (BO.BaseStation)dal.GetStationsList().First(x => x.Id == stationId).CopyPropertiesToNew(typeof(BO.BaseStation));

                //update location, statue and battery
                droneToAddBO.Location = requestedStation.Location; //drone location will be the same as station location
                droneToAddBO.Status = DroneStatuses.maintenance; // change drone statue to maintenance
                droneToAddBO.Battery = MyRandom.Next(20, 40); //get random between 20%-40%
                droneToListListBL.Add((DroneToList)droneToAddBO.CopyPropertiesToNew(typeof(DroneToList)));

            }
            catch (Exception ex)
            {
                throw new AddingException("Can't add this drone", ex);
            }

        }
        #endregion


        #region SetDroneName
        /// <summary>
        /// update drone's name
        /// </summary>
        /// <param name="droneBO"></param>
        public void SetDroneName(BO.Drone droneBO)
        {
            try
            {
                if (droneBO.Id < 0)      //check that drone's id is not negative
                    throw new InvalidInputException("drone id can not be negative");

                //find drone in data base and set the details
                DO.Drone droneDO = dal.GetDronesList().First(x => x.Id == droneBO.Id);
                droneDO.Model = droneBO.Model;
                dal.UpdateDrone(droneDO);
            }
            catch (Exception ex)
            {
                throw new UpdateException("Cannot update this drone", ex);
            }
        }
        #endregion


        #region SetSendDroneToCharge
        /// <summary>
        /// function that send a drone to charge and change it's details in accordingly
        /// </summary>
        /// <param name="droneId"></param>
        public void SetSendDroneToCharge(int droneId)
        {
            try
            {
                if (droneId < 0)      //check that drone's id is not negative
                    throw new InvalidInputException("Drone id can not be negative");

                //search the drone in the list of drone
                BO.DroneToList droneBO = GetDroneList().First(x => x.Id == droneId);

                if (droneBO.Status != DroneStatuses.available)
                {
                    throw new CheckInDroneToChargeException("Only a free drone can be sent to charging");
                }

                //get list of all free chrage stations DO 
                List<DO.Station> ListStationsDO = dal.GetListFreeChargeStationList().ToList();
                //if there are no Free charge slots in all Base station
                if (ListStationsDO == null || ListStationsDO.Count() == 0)
                {
                    throw new CheckInDroneToChargeException("There are no free charging stations");
                }

                int closestStationId = ListStationsDO.First().Id;


                double minDistance =
                    StationDistance(droneBO.Location, closestStationId);


                //find the closest station to the drone in the list of free charge slots station
                foreach (var obj in dal.GetListFreeChargeStationList())
                {
                    double newDistance =
                        StationDistance(droneBO.Location, obj.Id);

                    if (newDistance < minDistance)
                    {
                        minDistance = newDistance;
                        closestStationId = obj.Id;
                    }
                }

                //checking if there is enough battery to go to the base station
                if (droneBO.Battery - minDistance * available < 0)
                {
                    throw new CheckInDroneToChargeException("This drone does not have enough battery to go to recharge at the nearest available station");
                }

                // find the closest station 
                DO.Station closestStation = dal.GetListFreeChargeStationList().First(x => x.Id == closestStationId);

                droneBO.Battery -= minDistance * available;  //update drone range charge in accordance to the distance between the drone and the station
                droneBO.Location.Longtitude = closestStation.Longtitude;  //drone location = station location
                droneBO.Location.Lattitude = closestStation.Lattitude;
                droneBO.Status = DroneStatuses.maintenance;   //drone statue in maintenance


                droneToListListBL[droneToListListBL.FindIndex(x => x.Id == droneBO.Id)] = droneBO;
                // adds a drone to charge and updates free charge slots's number
                dal.UpdateDroneChargeCheckIn(droneId, closestStationId);
            }
            catch (Exception ex)
            {
                throw new CheckInDroneToChargeException("Can not send the drone to charge", ex);
            }
        }
        #endregion


        #region SetReleaseDroneFromCharge
        /// <summary>
        /// function that release the drone from charge and changes its details accordingly
        /// </summary>
        /// <param name="droneId"></param>
        /// <param name="ChargingTime"></param>
        public void SetReleaseDroneFromCharge(int droneId, TimeSpan ChargingTime)
        {
            try
            {
                if (droneId < 0)      //check that drone's id is not negative
                    throw new InvalidInputException("drone id can not be negative");

                //search the drone in the list of drone
                BO.DroneToList droneBO = GetDroneList().First(x => x.Id == droneId);

                if (droneBO.Status != DroneStatuses.maintenance)
                {
                    throw new CheckOutDroneFromChargeException("the drone statue isn't in maintenance");
                }

                //Battery status update
                droneBO.Battery += ChargingTime.TotalHours * droneChargeRange;
                //change status to be available
                droneBO.Status = DroneStatuses.available;

                //find the station where the drone was charging
                DO.Station dronesStation =
                    dal.GetStationsList().First(x => x.Lattitude == droneBO.Location.Lattitude && x.Longtitude == droneBO.Location.Longtitude);

                dal.UpdateDroneChargeCheckout(droneId, dronesStation.Id);
            }
            catch (Exception ex)
            {
                throw new CheckOutDroneFromChargeException("Can not release the drone from charge", ex);
            }
        }
        #endregion


        #region GetDrone
        /// <summary>
        /// function that gets an id drone number and returns that drone itself 
        /// </summary>
        /// <param name="droneId"></param>
        /// <returns></returns>
        public BO.Drone GetDrone(int droneId)
        {
            try
            {
                if (droneId < 0)
                    throw new InvalidInputException("drone id cannot be negative");

                return (BO.Drone)droneToListListBL.Find(D => D.Id == droneId).CopyPropertiesToNew(typeof(BO.Drone));
            }
            catch (Exception ex)
            {
                throw new GetException("Can't get the drone", ex);
            }
        }
        #endregion


        #region GetDroneList
        /// <summary>
        /// return the whole drone's list 
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BO.DroneToList> GetDroneList()
        {
            try
            {
                return from item in droneToListListBL select item;
            }
            catch (Exception ex)
            {
                throw new GetListException("Can't get the drones list", ex);
            }
        }
        #endregion


        #region GetDroneInChargesToList 
        // return list of the drone that in charge at this station that it's id had given
        private IEnumerable<BO.DroneInCharge> GetDroneInChargesToList(int baseStationId)
        {
            return (from item in dal.GetDroneChargesList()
                    where item.StationId == baseStationId
                    select new BO.DroneInCharge()
                    {
                        Id = item.DroneId,
                        BatteryStatus = droneToListListBL.Find(x => x.Id == item.DroneId).Battery,
                    }
                    ).ToList();
        }
        #endregion


        #region GetDroneInParcel
        /// <summary>
        /// returns drone in parcel that belong to the id
        /// </summary>
        /// <param name="droneId"></param>
        /// <returns></returns>
        private BO.DroneInParcel GetDroneInParcel(int droneId)
        {
            //search the drone and copy properties
            BO.DroneToList drone = droneToListListBL.Find(x => x.Id == droneId);
            return (BO.DroneInParcel)drone.CopyPropertiesToNew(typeof(BO.DroneInParcel));
        }
        #endregion


        #region GetDroneToList
        /// <summary>
        /// returns drone to list
        /// </summary>
        /// <param name="droneId"></param>
        /// <returns></returns>
        private BO.DroneToList GetDroneToList(int droneId)
        {
            return (BO.DroneToList)droneToListListBL.Find(x => x.Id == droneId).CopyPropertiesToNew(typeof(BO.DroneToList));
        }
        #endregion

    }
}