using System;
using BlApi;
using DalApi;
using System.Collections.Generic;
using System.Linq;
using BO;

namespace BL
{
    sealed partial class BL : IBL
    {
        #region singelton
        static readonly BL instance = new BL();
        static BL() { instance.initializeDrone(); }// static ctor to ensure instance init is done just before first usage
        BL() { } // default => private
        public static BL Instance { get => instance; }// The public Instance property to use
        #endregion

        internal IDal dal = DalFactory.GetDal();

        private List<BO.DroneToList> droneToListListBL = new List<BO.DroneToList>();

        static public double available;
        static public double lightWeightCarry;
        static public double mediumWeightCarry;
        static public double heavyWeightCarry;
        static public double droneChargeRange;

        private static Random MyRandom = new Random(DateTime.Now.Millisecond);

           

        //initialize function
        private void initializeDrone()
        {
            available = dal.getElectricityUseByDrone()[0];
            lightWeightCarry = dal.getElectricityUseByDrone()[1];
            mediumWeightCarry = dal.getElectricityUseByDrone()[2];
            heavyWeightCarry = dal.getElectricityUseByDrone()[3];
            droneChargeRange = dal.getElectricityUseByDrone()[4];

            //list of drone from data layer (DO to BO)
            List<DO.Drone> dronesListDO = dal.GetDronesList().ToList();
            foreach (DO.Drone drone in dal.GetDronesList())
            {
                droneToListListBL.Add((BO.DroneToList)drone.CopyPropertiesToNew(typeof(BO.DroneToList)));
            }


            //list of station from data layer
            List<DO.Station> stationsListDO = dal.GetStationsList().ToList();


            //list of customer from data layer
            List<DO.Customer> customersListDO = dal.GetCustomersList().ToList();


            //list of parcel from data layer
            List<DO.Parcel> parcelsListDO = dal.GetParcelsList().Where(P => P.DeliveredTime == null).ToList();


            foreach (BO.DroneToList drone in droneToListListBL)
            {
                //search index of the drone in parcel list
                int parcelIndex = parcelsListDO.FindIndex(P => P.DroneId == drone.Id);

                if (parcelIndex >= 0)
                {
                    //updates drone's detail and search closest station to customer
                    drone.Status = DroneStatuses.shipped;
                    DO.Parcel tempParcel = parcelsListDO[parcelIndex];
                    DO.Station closestStation = stationsListDO.First(x => x.Id == ClosestStationToCustomer(tempParcel.SenderId));

                    if (tempParcel.PickedUpTime == null)
                    {
                        //if the drone wasn't picked up ,update drone's location to station's location
                        drone.Location.Longtitude = closestStation.Longtitude;
                        drone.Location.Lattitude = closestStation.Lattitude;
                    }
                    else if (tempParcel.DeliveredTime == null)
                    {
                        //if the drone wasn't delivered, update drone's location to sender's location
                        DO.Customer sender = customersListDO.Find(C => C.Id == tempParcel.SenderId);
                        drone.Location.Longtitude = sender.Longtitude;
                        drone.Location.Lattitude = sender.Lattitude;
                    }

                    // calculate the distance that the drone have to do
                    double wayToGo = CustomerDistance(drone.Location, tempParcel.TargetId); // calculate the distance betuwin the drone and the target
                    closestStation = stationsListDO.First(x => x.Id == ClosestStationToCustomer(tempParcel.TargetId)); // find the closest station to the target

                    Location stationLocation = new Location() { Lattitude = closestStation.Lattitude, Longtitude = closestStation.Longtitude };
                    wayToGo += CustomerDistance(stationLocation, tempParcel.TargetId); //calculate the distance betwin the target and the closest station

                    //in dependance of drone's weight calcule battery range
                    switch (tempParcel.Weight)
                    {
                        case DO.WeightCategories.light:
                            drone.Battery = MyRandom.Next((int)(wayToGo * lightWeightCarry), 100) + MyRandom.NextDouble();
                            break;
                        case DO.WeightCategories.medium:
                            drone.Battery = MyRandom.Next((int)(wayToGo * mediumWeightCarry), 100) + MyRandom.NextDouble();
                            break;
                        case DO.WeightCategories.heavy:
                            drone.Battery = MyRandom.Next((int)(wayToGo * heavyWeightCarry), 100) + MyRandom.NextDouble();
                            break;
                        default: // לבדוק מה לעשות במקרה שאין לחבילה רמת משקל
                            break;
                    }
                }
                else // if the drone is not scheduled
                {

                    //mazal tov!!



                    drone.Status = (DroneStatuses)(MyRandom.Next(1, 2));

                    if (drone.Status == DroneStatuses.available)
                    {
                        //if the drone is available
                        DO.Customer oneCustomer = customersListDO[MyRandom.Next(customersListDO.Count)];
                        Location loc = new Location { Lattitude = oneCustomer.Lattitude, Longtitude = oneCustomer.Longtitude };
                        drone.Location = loc;
                        DO.Station closestStation = stationsListDO.First(x => x.Id == ClosestStationToCustomer(oneCustomer.Id)); // find the closest station

                        Location stationLocation = new Location() { Lattitude = closestStation.Lattitude, Longtitude = closestStation.Longtitude };
                        double wayToGo = CustomerDistance(stationLocation, oneCustomer.Id); // find the distance betuwin the station and the customer
                        drone.Battery = (MyRandom.NextDouble() * (100 - (available * wayToGo))) + available * wayToGo;
                    }
                    else if (drone.Status == DroneStatuses.maintenance)
                    {
                        DO.Station oneStation = stationsListDO[MyRandom.Next(stationsListDO.Count)];
                        drone.Location.Lattitude = oneStation.Lattitude;
                        drone.Location.Longtitude = oneStation.Longtitude;

                        drone.Battery = MyRandom.Next(20) + MyRandom.NextDouble();
                    }
                }
            }
        }
    }
}
