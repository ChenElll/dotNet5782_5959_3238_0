using System;
using System.Collections.Generic;
using System.Linq;
using BO;
using DO;
using BlApi;


namespace BL
{
    public partial class BL : IBL
    {

        #region AddParcel
        /// <summary>
        /// function that add a parcel 
        /// </summary>
        /// <param name="parcelToAddBO"></param>
        public void AddParcel(BO.Parcel parcelToAddBO)
        {
            try
            {
                if (parcelToAddBO.Id < 0)//check that the parcel id to add is not negative
                    throw new InvalidInputException("Parcel id cannot be negative");

                parcelToAddBO.RequestedTime = DateTime.Now;  //update Requested time to now 

                DO.Parcel parcelToAddDO = (DO.Parcel)parcelToAddBO.CopyPropertiesToNew(typeof(DO.Parcel));
                parcelToAddDO.SenderId = parcelToAddBO.Sender.Id;
                parcelToAddDO.TargetId = parcelToAddBO.Target.Id;

                dal.AddingParcel(parcelToAddDO);
            }
            catch (Exception ex)
            {
                throw new AddingException("Can't add this parcel", ex);
            }
        }
        #endregion


        #region UpdateScheduleParcel
        /// <summary>
        /// update a parcel to schedule
        /// </summary>
        /// <param name="droneId"></param>
        public void UpdateScheduleParcel(int droneId)
        {
            try
            {
                if (droneId < 0)//check that the drone id to add is not negative
                    throw new InvalidInputException("Drone id cannot be negative");

                DO.Drone droneDO = dal.GetDrone(droneId); //exception if not exist

                BO.DroneToList droneBO = droneToListListBL.Find(x => x.Id == droneId);

                if (droneBO == default)
                    throw new DronesException("No signal from the drone");


                if (droneBO.Status != DroneStatuses.available)
                    throw new SchedulParcelException("This drone isn't available");


                var parcelsToSchedule = (from P in dal.GetNotAttributeParcelsList()
                                             //filter parcels that are too heavy
                                         where (int)P.Weight < (int)droneBO.MaxWeight
                                         //order the parcels in that group by descending order of priority and weight 
                                         orderby (int)P.Priority descending, P.Weight descending
                                         // group according to weight parcel
                                         group P by (int)P.Weight into newGroup
                                         //oredr group by descending order of weight
                                         orderby newGroup.Key descending
                                         // choose the new list
                                         select newGroup).ToList();

                foreach (object parcel in parcelsToSchedule)
                {
                    double backAndForthCharging = CustomerDistance(droneBO.Location, ((DO.Parcel)parcel).SenderId);
                    DO.Customer targetCustomer = dal.GetCustomer(((DO.Parcel)parcel).TargetId);
                    Location targetLocation = new Location() { Longtitude = targetCustomer.Longtitude, Lattitude = targetCustomer.Lattitude };
                    backAndForthCharging +=
                        StationDistance(targetLocation, ClosestStationToCustomer(((DO.Parcel)parcel).TargetId));
                    backAndForthCharging *= available;


                    DO.Customer senderCustomer = dal.GetCustomer(((DO.Parcel)parcel).SenderId);
                    Location senderLocation = new Location() { Longtitude = senderCustomer.Longtitude, Lattitude = senderCustomer.Lattitude };
                    double carringTime =
                        CustomerDistance(senderLocation, ((DO.Parcel)parcel).TargetId);


                    switch (((DO.Parcel)parcel).Weight)
                    {
                        case DO.WeightCategories.light:
                            backAndForthCharging += carringTime * lightWeightCarry;
                            break;
                        case DO.WeightCategories.medium:
                            backAndForthCharging += carringTime * mediumWeightCarry;
                            break;
                        case DO.WeightCategories.heavy:
                            backAndForthCharging += carringTime * heavyWeightCarry;
                            break;
                        default:
                            break;
                    }

                    if (droneBO.Battery >= backAndForthCharging)
                    {
                        dal.UpdateSchdule(droneId, ((DO.Parcel)parcel).Id);
                        droneBO.ParcelNumberInTransfer = ((DO.Parcel)parcel).Id;
                        droneBO.Status = DroneStatuses.shipped;
                        droneToListListBL[droneToListListBL.FindIndex(x => x.Id == droneId)] = droneBO;
                        break;
                    }

                }

                if (droneBO.Status != DroneStatuses.shipped)
                    throw new SchedulParcelException("Can not find a suitable parcel");

            }
            catch (Exception ex)
            {
                throw new SchedulParcelException("the program can not update shceduled parcel to drone", ex);
            }
        }
        #endregion


        #region UpdatePickUpParcel
        /// <summary>
        /// update details of parcel to pick up
        /// </summary>
        /// <param name="droneId"></param>
        public void UpdatePickUpParcel(int droneId)
        {
            try
            {
                if (droneId < 0)//check that the drone id to add is not negative
                    throw new InvalidInputException("Drone id cannot be negative");

                //find the parcel that is attribute to this drone
                DO.Parcel parcelDO = dal.GetParcelsList().First(x => x.DroneId == droneId);
                BO.Parcel parcelBO = GetParcel(parcelDO.Id);
                BO.DroneToList droneBO = droneToListListBL.Find(x => x.Id == droneId);

                if (droneBO == default)
                    throw new DronesException("No signal from the drone");

                if (parcelDO.ScheduledTime == null)
                    throw new SchedulParcelException("The parcel was not schedule to this drone");

                //update details in drone and parcel

                DO.Customer customerSender = dal.GetCustomersList().First(x => x.Id == parcelDO.SenderId);

                BO.Customer senderBO = GetCustomer(customerSender.Id);


                double wayToGo =
                    CustomerDistance(droneBO.Location, parcelDO.SenderId);

                droneBO.Battery -= wayToGo * available;
                droneBO.Location.Lattitude = customerSender.Lattitude;
                droneBO.Location.Longtitude = customerSender.Longtitude;

                parcelDO.PickedUpTime = DateTime.Now;

                dal.UpdateParcel(parcelDO);
            }
            catch (Exception ex)
            {
                throw new PickUpParcelException("the program can not update pick up parcel", ex);
            }
        }
        #endregion


        #region UpdateDeliveryToCustomer
        /// <summary>
        /// update parcel's details to delivery
        /// </summary>
        /// <param name="droneId"></param>
        public void UpdateDeliveryToCustomer(int droneId)
        {
            try
            {
                if (droneId < 0)//check that the drone id to add is not negative
                    throw new InvalidInputException("Drone id cannot be negative");

                DO.Parcel parcelDO = dal.GetParcelsList().First(x => x.DroneId == droneId);
                BO.Parcel parcelBO = GetParcel(parcelDO.Id);

                if (parcelDO.PickedUpTime == null)//if the parcel was not picked up yet
                    throw new PickUpParcelException("the parcel has not been collected yet");

                if (parcelDO.DeliveredTime != null)  //or parcel was already delivered
                    throw new DeliveryException("the parcel is already delivered");

                BO.DroneToList droneBO = droneToListListBL.Find(x => x.Id == droneId);
                if (droneBO == default)
                    throw new DronesException("No signal from the drone");

                double wayToGo =
                    CustomerDistance(droneBO.Location, parcelBO.Target.Id);

                double wayToGoCharge = 0;

                switch (parcelDO.Weight)
                {
                    case DO.WeightCategories.light:
                        wayToGoCharge += wayToGo * lightWeightCarry;
                        break;
                    case DO.WeightCategories.medium:
                        wayToGoCharge += wayToGo * mediumWeightCarry;
                        break;
                    case DO.WeightCategories.heavy:
                        wayToGoCharge += wayToGo * heavyWeightCarry;
                        break;
                    default:
                        break;
                }

                droneBO.Battery -= wayToGoCharge;

                DO.Customer customerTarget = dal.GetCustomersList().First(x => x.Id == parcelDO.TargetId);
                droneBO.Location.Lattitude = customerTarget.Lattitude;
                droneBO.Location.Longtitude = customerTarget.Longtitude;

                droneBO.Status = DroneStatuses.available;
                droneToListListBL[droneToListListBL.FindIndex(x => x.Id == droneBO.Id)] = droneBO;

                parcelDO.DeliveredTime = DateTime.Now;
                dal.UpdateParcel(parcelDO);
            }
            catch (Exception ex)
            {
                throw new DeliveryException("The drone can not deliver the parcel", ex);
            }
        }
        #endregion


        #region GetParcel
        /// <summary>
        /// gets a parcel id and returns the parcel itself
        /// </summary>
        /// <param name="parcelId"></param>
        /// <returns></returns>
        public BO.Parcel GetParcel(int parcelId)
        {
            try
            {
                if (parcelId < 0)//check that the Parcel id to add is not negative
                    throw new InvalidInputException("Parcel id cannot be negative");

                DO.Parcel parcelDO = dal.GetParcel(parcelId);
                BO.Parcel parcelBO = (BO.Parcel)parcelDO.CopyPropertiesToNew(typeof(BO.Parcel));

                parcelBO.Sender = GetCustomerInParcel(parcelDO.SenderId);
                parcelBO.Target = GetCustomerInParcel(parcelDO.TargetId);

                parcelBO.Drone = GetDroneInParcel(parcelDO.DroneId);

                return parcelBO;
            }
            catch (Exception ex)
            {
                throw new GetException("Can't get the parcel", ex);
            }

        }
        #endregion


        #region GetParcelList
        /// <summary>
        /// returns the whole parcel's list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<BO.ParcelToList> GetParcelList()
        {
            try
            {
                var listOfParcels = new List<ParcelToList>();
                foreach (DO.Parcel item in dal.GetParcelsList())
                {
                    listOfParcels.Add(GetParcelToList(item.Id));
                }
                return listOfParcels;
            }
            catch (Exception ex)
            {
                throw new GetListException("Can't get the parcels list", ex);
            }
        }
        #endregion


        #region GetNotScheduleParcelList
        /// <summary>
        /// returns the whole not scheduled parcel's list
        /// </summary>
        /// <returns></returns>
        public IEnumerable<ParcelToList> GetNotScheduleParcelList()
        {
            try
            {
                List<BO.ParcelToList> parcelsListBO = new List<ParcelToList>();
                foreach (DO.Parcel item in dal.GetNotAttributeParcelsList())
                {
                    BO.ParcelToList newParcel = (BO.ParcelToList)item.CopyPropertiesToNew(typeof(BO.ParcelToList));
                    newParcel.ParcelStatus = ParcelStatus.Requested;
                }

                return parcelsListBO;
            }
            catch (Exception ex)
            {
                throw new GetException("Can't get the not scheduled parcels list", ex);
            }
        }
        #endregion


        #region ParcelAtCustomer
        /// <summary>
        /// returns parcel at customer
        /// </summary>
        /// <param name="parcelId"></param>
        /// <returns></returns>
        private BO.ParcelAtCustomer GetParcelAtCustomer(int parcelId)
        {

            if (parcelId < 0)//check that the Parcel id to add is not negative
                throw new InvalidInputException("Parcel id cannot be negative");
            try
            {
                DO.Parcel parcelDO = dal.GetParcel(parcelId);   //search the parcel  
                BO.ParcelAtCustomer parcelBO = new ParcelAtCustomer();
                parcelBO = (BO.ParcelAtCustomer)parcelDO.CopyPropertiesToNew(typeof(BO.ParcelAtCustomer));

                if (parcelDO.RequestedTime != null)
                    parcelBO.Status = ParcelStatus.Requested;
                else if (parcelDO.ScheduledTime != null)
                    parcelBO.Status = ParcelStatus.scheduled;
                else if (parcelDO.PickedUpTime != null)
                    parcelBO.Status = ParcelStatus.pickedUp;
                else if (parcelDO.DeliveredTime != null)
                    parcelBO.Status = ParcelStatus.delivered;

                //parcelBO.SourceOrDestination = default; // update before using

                return parcelBO;
            }
            catch (Exception ex)
            {
                throw new GetException("Can not get this parcel", ex);
            }
        }
        #endregion


        #region ParcelToList
        /// <summary>
        /// returns parcel to list
        /// </summary>
        /// <param name="parcelId"></param>
        /// <returns></returns>
        private BO.ParcelToList GetParcelToList(int parcelId)
        {
            try
            {
                if (parcelId < 0) //check that the Parcel id to add is not negative
                    throw new InvalidInputException("Parcel id cannot be negative");

                DO.Parcel parcelDO = dal.GetParcel(parcelId);   //search the parcel 

                BO.ParcelToList parcelBO = (BO.ParcelToList)parcelDO.CopyPropertiesToNew(typeof(BO.ParcelToList));
                parcelBO.NameSender = dal.GetCustomersList().First(x => x.Id == parcelDO.SenderId).CustomerName;
                parcelBO.NameTarget = dal.GetCustomersList().First(x => x.Id == parcelDO.TargetId).CustomerName;

                if (parcelDO.RequestedTime != null)
                    parcelBO.ParcelStatus = ParcelStatus.Requested;
                else if (parcelDO.ScheduledTime != null)
                    parcelBO.ParcelStatus = ParcelStatus.scheduled;
                else if (parcelDO.PickedUpTime != null)
                    parcelBO.ParcelStatus = ParcelStatus.pickedUp;
                else if (parcelDO.DeliveredTime != null)
                    parcelBO.ParcelStatus = ParcelStatus.delivered;

                return parcelBO;
            }
            catch (Exception ex)
            {
                throw new GetException("Can not get this parcel", ex);
            }
        }
        #endregion

    }
}