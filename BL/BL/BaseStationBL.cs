using System;
using BO;
using System.Collections.Generic;
using System.Linq;
//using DO;
using BlApi;

namespace BL
{
    public partial class BL : IBL
    {

        #region AddStation
        /// <summary>
        /// add a station to the system
        /// </summary>
        /// <param name="baseStationToAdd"></param>
        public void AddStation(BO.BaseStation baseStationToAdd)
        {
            try  //try to add the station to the system
            {
                if (baseStationToAdd.Id < 0)//check that the station id to add is not negative
                {
                    throw new InvalidInputException("Station id can not be negative");
                }

                if (baseStationToAdd.Location.Lattitude < 35.1252 || baseStationToAdd.Location.Lattitude > 35.2642
                        || baseStationToAdd.Location.Longtitude < 31.7082 || baseStationToAdd.Location.Longtitude > 31.8830)
                {
                    throw new InvalidInputException("The chosen location is not within our service limits, " +
                            "choose location from these values: lattitude(35.1252-35.2642), longtitude(31.7082-31.8830)");
                }

                //copy properties and location to the new station and add it 
                DO.Station newStation = (DO.Station)baseStationToAdd.CopyPropertiesToNew(typeof(DO.Station));
                newStation.Lattitude = baseStationToAdd.Location.Lattitude;
                newStation.Longtitude = baseStationToAdd.Location.Longtitude;
                dal.AddStation(newStation);
            }
            catch (Exception ex)
            {
                throw new AddingException("Can't add this station", ex);
            }
        }
        #endregion


        #region SetStationDetails
        /// <summary>
        /// update station's name and number of free charge slots
        /// </summary>
        /// <param name="stationId"></param>
        /// <param name="stationName"></param>
        /// <param name="allChargeSlots"></param>
        public void SetStationDetails(int stationId, string stationName, string allChargeSlots)
        {
            try
            {
                //get the station requested
                DO.Station myStation = dal.GetStation(stationId);
                //if name's station isn't empty replace it
                if (stationName != default) { myStation.Name = stationName; }
                //if number of charge slots isn't 0, count the free ones from the list of drone charges 
                if (allChargeSlots != default)
                {
                    myStation.FreeChargeSlots =
                    int.Parse(allChargeSlots) - dal.GetDroneChargesList(D => D.StationId == stationId).Count();
                }

                dal.UpdateStation(myStation);
            }
            catch (Exception ex)
            {
                throw new UpdateException("Can't update the station", ex);
            }
        }
        #endregion


        #region GetStation
        /// <summary>
        /// gets a station id and returns the station itself
        /// </summary>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public BO.BaseStation GetStation(int stationId)
        {
            try
            {
                //get the station from station list in dal layer
                DO.Station stationDO = dal.GetStation(stationId);
                //filter from drone charge's list those with the station's id wanted
                //List<DO.DroneCharge> droneChargesListDO =
                //    (List<DO.DroneCharge>)dal.GetDroneChargesList(D => D.StationId == stationId);

                //copy properties from dal station to bl station
                BO.BaseStation baseStationBO = (BO.BaseStation)stationDO.CopyPropertiesToNew(typeof(BO.BaseStation));
                baseStationBO.Location = new Location { Longtitude = stationDO.Longtitude, Lattitude = stationDO.Lattitude };
                baseStationBO.DronesInCharge = (List<BO.DroneInCharge>)GetDroneInChargesToList(stationId);

                return baseStationBO;
            }
            catch (Exception ex)
            {
                throw new GetException("Can't get the station", ex);
            }

        }
        #endregion


        #region GetStationList
        /// <summary>
        /// returns the whole station's list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BO.BaseStationToList> GetStationList(Func<DO.Station, bool> predicat = null)
        {
            try
            {
                return from item in dal.GetStationsList(predicat)
                       select GetStationToList(item.Id);
            }
            catch (Exception ex)
            {
                throw new GetListException("Can't get the stations list", ex);
            }
        }
        #endregion


        #region GetFreeChargeStationList
        public IEnumerable<BO.BaseStationToList> GetFreeChargeStationList()
        {
            //choose all the station that have free charges slots
            return from item in dal.GetStationsList(S => S.FreeChargeSlots > 0)
                   select GetStationToList(item.Id);
        }
        #endregion


        #region ClosestStationToCustomer
        /// <summary>
        /// search the closest station to a specific customer
        /// </summary>
        /// <param name="customerId"></param>
        /// <returns> id of the closest station </returns>
        private int ClosestStationToCustomer(int customerId)
        {
            //checks if invalid input
            if (customerId < 0)
                throw new InvalidInputException("Customer id can not be negative");
            //if exception wasn't throw get the first station in the list
            DO.Station tempStation = dal.GetStationsList().First();


            Location StationLocation = new Location() { Longtitude = tempStation.Longtitude, Lattitude = tempStation.Lattitude };

            double minDistance =
                CustomerDistance(StationLocation, customerId); //find the distance between the first station and the customer

            int closestStationId = tempStation.Id;

            foreach (var obj in dal.GetStationsList())
            {
                double newDistance =
                    CustomerDistance(StationLocation, customerId); //calcul distance for the rest of the stations

                if (newDistance < minDistance) //put in minDistance the smallest distance
                {
                    minDistance = newDistance;
                    closestStationId = obj.Id;
                }

            }
            try
            {
                return closestStationId;
            }
            catch (Exception ex)
            {
                throw new FindClosestStationException("Can not find the closest station", ex);

            }
        }
        #endregion


        #region ClosestStationToDrone
        /// <summary>
        /// search the closest station to a specific drone
        /// </summary>
        /// <param name="droneId"></param>
        /// <returns>id of the closest station</returns>
        private int ClosestStationToDrone(int droneId)
        {
            try
            {
                //checks if invalid input
                if (droneId < 0)
                    throw new InvalidInputException("Drone id can not be negative");

                BO.Location droneLocation = droneToListListBL.Find(x => x.Id == droneId).Location;
                //if exception wasn't throw get the first station in the list
                int closestStationId = dal.GetStationsList().First().Id;

                double minDistance =
                    StationDistance(droneLocation, closestStationId); //find the distance between the first station and the drone

                foreach (var obj in dal.GetStationsList())
                {
                    double newDistance =
                        StationDistance(droneLocation, obj.Id); //calcul distance for the rest of the stations

                    if (newDistance < minDistance)  //put in minDistance the smallest distance
                    {
                        minDistance = newDistance;
                        closestStationId = obj.Id;
                    }
                }

                return closestStationId;
            }

            catch (Exception ex)
            {
                throw new FindClosestStationException("Can not find the closest station", ex);
            }
        }
        #endregion


        #region StationDistance
        /// <summary>
        /// return distance from drone to station
        /// </summary>
        /// <param name="pointLattitude"></param>
        /// <param name="pointLongtitude"></param>
        /// <param name="stationId"></param>
        /// <returns></returns>
        public double StationDistance(Location location, int stationId)
        {
            BO.Location tempSLocation = GetStation(stationId).Location;
            return Distance(location, tempSLocation);
        }
        #endregion


        #region GetStationToList
        /// <summary>
        /// returns station to list
        /// </summary>
        /// <param name="stationId"></param>
        /// <returns></returns>
        private BO.BaseStationToList GetStationToList(int stationId)
        {
            try
            {
                //search the station
                DO.Station stationDO = dal.GetStation(stationId);
                //copy properties 
                BO.BaseStationToList baseStationBO = (BO.BaseStationToList)stationDO.CopyPropertiesToNew(typeof(BO.BaseStationToList));
                baseStationBO.OccupiedChargeSlots =
                    dal.GetDroneChargesList(item => item.StationId == stationDO.Id).Count();

                return baseStationBO;
            }
            catch (Exception ex)
            {
                throw new GetException("there is no parcel with this id", ex);
            }
        }
        #endregion

    }
}