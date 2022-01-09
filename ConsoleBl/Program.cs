using System;
using System.Collections.Generic;
using BlApi;
using BO;
using DO;

namespace ConsoleBl
{
    class Program
    {
        #region enums
        public enum Options
        {
            add = 1, update, view, listView, exit
        }

        public enum Add
        {
            addNewStation = 1, addNewDrone, addNewCustomer, addNewParcel
        }

        public enum Update
        {
            updateDroneName = 1, updateStationDetails, updateCustomerDetails, schduleParcelToDrone, pickUpParcel, deliveryToCustomer, SendToCharge, releaseDrone
        }


        public enum View
        {
            viewStation = 1, viewDrone, viewCustomer, viewParcel
        }

        public enum ListView
        {
            stationList = 1, droneList, CustomerList, parcelList, notAttributeParcel, freeChargeStation
        }
        #endregion

        static IBL bl = BlFactory.GetBl();
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            //-----------------------------adding options-----------------------------------

            #region AddStation

            void getAddstation()
            {

                BaseStation baseStationToAdd = new();
                BO.Location stationLocation = new();
                Console.WriteLine("Please enter station's id (1 digit)\n");
                baseStationToAdd.Id = int.Parse(Console.ReadLine());

                Console.WriteLine("Please enter station's name\n");
                baseStationToAdd.Name = Console.ReadLine();

                Console.WriteLine("Please enter station's longtitude\n");
                stationLocation.Longtitude = double.Parse(Console.ReadLine());

                Console.WriteLine("Please enter station's lattitude\n");
                stationLocation.Lattitude = double.Parse(Console.ReadLine());

                baseStationToAdd.Location = stationLocation;

                Console.WriteLine("Please enter station's number of charge slots\n");
                baseStationToAdd.FreeChargeSlots = int.Parse(Console.ReadLine());

                baseStationToAdd.DronesInCharge = new List<DroneInCharge>();

                //call to function

                bl.AddStation(baseStationToAdd);



            }
            #endregion

            #region AddDrone

            void getAddDrone()
            {
                BO.Drone droneToAdd = new BO.Drone();

                Console.WriteLine("Please enter drone's id (4 digits)\n");
                droneToAdd.Id = int.Parse(Console.ReadLine());

                Console.WriteLine("Please enter drone's model\n");
                droneToAdd.Model = Console.ReadLine();

                Console.WriteLine("Please enter drone's weight ability (light/medium/heavy)\n");
                droneToAdd.MaxWeight = (WeightCategories)int.Parse(Console.ReadLine());

                Console.WriteLine("Please enter station id where would you put on for first charge\n");
                int stationId = int.Parse(Console.ReadLine());

                bl.AddDrone(droneToAdd, stationId);

            }

            #endregion

            #region AddCustomer


            void getAddNewCustomer()
            {
                BO.Customer customerToAdd = new BO.Customer();
                BO.Location customerLocation = new();

                Console.WriteLine("Please enter customer's id (9 digits)\n");
                customerToAdd.Id = int.Parse(Console.ReadLine());

                Console.WriteLine("Please enter customer's name\n");
                customerToAdd.CustomerName = Console.ReadLine();

                Console.WriteLine("Please enter customer's phone (10 digits)\n");
                customerToAdd.PhoneNumber = Console.ReadLine();

                Console.WriteLine("Please enter customer's longtitude\n");
                customerLocation.Longtitude = double.Parse(Console.ReadLine());

                Console.WriteLine("Please enter customer's lattitude\n");
                customerLocation.Lattitude = double.Parse(Console.ReadLine());

                customerToAdd.Location = customerLocation;

                bl.AddCustomer(customerToAdd);
            }
            #endregion

            #region AddParcelToDeliver

            void getAddNewParcel()
            {
                BO.Parcel parcelToAdd = new BO.Parcel();
                CustomerInParcel senderInParcel = new();
                CustomerInParcel targetInParcel = new();
                DroneInParcel droneInParcel = new();

                Console.WriteLine("Please enter sender's id (9 digits)\n");
                senderInParcel.Id = int.Parse(Console.ReadLine());

                parcelToAdd.Sender = senderInParcel;

                Console.WriteLine("Please enter target's id (9 digits)\n");
                targetInParcel.Id = int.Parse(Console.ReadLine());

                parcelToAdd.Target = targetInParcel;

                Console.WriteLine("Please enter parcel's weight (light 1/medium 2/heavy 3)\n");
                parcelToAdd.Weight = (WeightCategories)int.Parse(Console.ReadLine());

                Console.WriteLine("Please enter parcel's priority (regular 1/fast 2/emergency 3)\n");
                parcelToAdd.Priority = (Priorities)int.Parse(Console.ReadLine());

                Console.WriteLine("Please enter scheduled drone's id (4 digits)\n");
                droneInParcel.Id = int.Parse(Console.ReadLine());

                parcelToAdd.Drone = droneInParcel;

                parcelToAdd.RequestedTime = DateTime.Now;
                parcelToAdd.ScheduleTime = DateTime.MinValue;
                parcelToAdd.PickUpTime = DateTime.MinValue;
                parcelToAdd.DeliveredTime = DateTime.MinValue;

                bl.AddParcel(parcelToAdd);
            }

            #endregion


            //------------------------------Update options--------------------------------------


            #region UpdateDroneName
            void getUpdateDroneName()
            {
                Console.WriteLine("Please enter the id and the model of the drone you want to update\n");
                bl.SetDroneName(new BO.Drone()
                {
                    Id = int.Parse(Console.ReadLine()),
                    Model = Console.ReadLine(),
                });
            }
            #endregion

            #region UpdateStationDetails

            void getUpdateStationDetails()
            {
                Console.WriteLine("Please enter station id\n");
                int stationId = int.Parse(Console.ReadLine());

                Console.WriteLine("Please enter station name\n");
                string stationName = Console.ReadLine();

                Console.WriteLine("Please enter number of charge slots\n");
                string numberOfChargeSlots = Console.ReadLine();

                bl.SetStationDetails(stationId, stationName, numberOfChargeSlots);
            }

            #endregion

            #region UpdateCustomerDetails

            void getUpdateCustomerDetails()
            {
                Console.WriteLine("Please enter customer id, name and phone number\n");
                bl.SetCustomerDetails(new BO.Customer()
                {
                    Id = int.Parse(Console.ReadLine()),
                    CustomerName = Console.ReadLine(),
                    PhoneNumber = Console.ReadLine()

                });
            }


            #endregion

            #region UpdateSendToCharge

            void GetUpdateSendToCharge()
            {
                Console.WriteLine("Please enter drone id\n");
                int droneId = int.Parse(Console.ReadLine());

                bl.SetSendDroneToCharge(droneId);

            }

            #endregion

            #region UpdateReleaseDrone

            void GetUpdateReleaseDrone()
            {
                Console.WriteLine("Please enter drone id and charging time\n");
                int droneId = int.Parse(Console.ReadLine());
                TimeSpan ChargingTime = TimeSpan.Parse(Console.ReadLine());

                bl.SetReleaseDroneFromCharge(droneId, ChargingTime);
            }

            #endregion

            #region UpdateScheduleParcel

            void GetUpdateScheduleParcel()
            {
                Console.WriteLine("Please enter drone id\n");
                int droneId = int.Parse(Console.ReadLine());

                bl.UpdateScheduleParcel(droneId);
            }

            #endregion

            #region UpdatePickUpParcel

            void GetUpdatePickUpParcel()
            {
                Console.WriteLine("Please enter drone id\n");
                int droneId = int.Parse(Console.ReadLine());

                bl.UpdatePickUpParcel(droneId);
            }

            #endregion

            #region UpdateDeliveryToCustomer

            void GetUpdateDeliveryToCustomer()
            {
                Console.WriteLine("Please enter drone id\n");
                int droneId = int.Parse(Console.ReadLine());

                bl.UpdateDeliveryToCustomer(droneId);
            }

            #endregion


            //------------------------------View options----------------------------------

            #region viewStation
            void getviewStation()
            {
                Console.WriteLine("Please enter station id\n");
                int stationId = int.Parse(Console.ReadLine());
                Console.WriteLine(bl.GetStation(stationId));

            }
            #endregion

            #region viewDrone
            void getviewDrone()
            {
                Console.WriteLine("Please enter drone id\n");
                int droneId = int.Parse(Console.ReadLine());
                Console.WriteLine(bl.GetDrone(droneId));
            }
            #endregion

            #region viewCustomer
            void getviewCustomer()
            {
                Console.WriteLine("Please enter customer id\n");
                int customerId = int.Parse(Console.ReadLine());
                Console.WriteLine(bl.GetCustomer(customerId));
            }
            #endregion

            #region viewParcel
            void getviewParcel()
            {
                Console.WriteLine("Please enter parcel id\n");
                int parcelId = int.Parse(Console.ReadLine());
                Console.WriteLine(bl.GetParcel(parcelId));
            }
            #endregion


            //-----------------------------View list options-------------------------------

            #region ViewStation
            void GetListViewStation()
            {
                foreach (BaseStationToList station in bl.GetStationList())
                    Console.WriteLine(station);
            }
            #endregion

            #region ListViewDrone
            void GetListViewDrone()
            {
                foreach (DroneToList drone in bl.GetDroneList())
                    Console.WriteLine(drone);
            }
            #endregion

            #region ListViewCustomer
            void GetListViewCustomer()
            {
                foreach (CustomerToList customer in bl.GetCustomerList())
                    Console.WriteLine(customer);
            }
            #endregion

            #region ListViewParcel
            void GetListViewParcel()
            {
                foreach (ParcelToList parcel in bl.GetParcelList())
                    Console.WriteLine(parcel);
            }
            #endregion

            #region ViewNotScheduleParcel
            void GetListViewNotScheduleParcel()
            {
                bl.GetNotScheduleParcelList();
            }
            #endregion

            #region ViewFreeChargeStation
            void GetListViewFreeChargeStation()
            {
                bl.GetStationList(S => S.FreeChargeSlots > 0);
            }
            #endregion



            //-----------default function-------------

            void getdefault()
            {
                Console.WriteLine("This option does not exist");
            }


            //---------------------------------------------------------------------------
            Console.WriteLine("Hello Evreyone!\n");
            int choice = 0;
            do
            {
                try
                {
                    Console.WriteLine("Please, enter your choice:\n"
                      + "to add a station/drone/customer/parcel, press 1\n"
                      + "to update  information, press 2\n"
                      + "to see view options, press 3\n"
                      + "to see view lists, press 4\n"
                      + "to measure distance, press 6\n"
                      + "to exit, press 5\n");
                    // prints the main menu

                    choice = int.Parse(Console.ReadLine());


                    switch ((Options)choice)
                    {
                        case Options.add:
                            {
                                Console.WriteLine("to add a new station to the list choose 1\n" +
                                    "to add a new drone to the list choose 2\n+" +
                                    "to reception a new customer to the list choose 3\n" +
                                    "to reception a new parcel to delivery choose 4\n");
                                //prints all adding options

                                int chooseAdd = int.Parse(Console.ReadLine());

                                switch ((Add)chooseAdd)
                                {
                                    case Add.addNewStation:
                                        getAddstation();
                                        break;
                                    case Add.addNewDrone:
                                        getAddDrone();
                                        break;
                                    case Add.addNewCustomer:
                                        getAddNewCustomer();
                                        break;
                                    case Add.addNewParcel:
                                        getAddNewParcel();
                                        break;
                                    default:
                                        getdefault();
                                        break;
                                }
                                break;
                            }

                        case Options.update:
                            {
                                //update options
                                Console.WriteLine("to update drones name, choose 1\n" +
                                    "to update stations details, choose 2\n" +
                                    "to update customers details, choose 3\n" +
                                    "to update an schedule of a parcel to a drone, choose 4\n" +
                                    "to update a pick up of a parcel by a drone, choose 5\n" +
                                    "to update a delivery of a parcel to a customer, choose 6\n" +
                                    "to update a sending of a parcelto charge at a base station, choose 7\n" +
                                    "to update a released drone, choose 8\n");

                                int chooseUpdate = int.Parse(Console.ReadLine());

                                switch ((Update)chooseUpdate)
                                {
                                    case Update.updateDroneName:
                                        getUpdateDroneName();
                                        break;
                                    case Update.updateStationDetails:
                                        getUpdateStationDetails();
                                        break;
                                    case Update.updateCustomerDetails:
                                        getUpdateCustomerDetails();
                                        break;
                                    case Update.schduleParcelToDrone:
                                        GetUpdateScheduleParcel();
                                        break;
                                    case Update.pickUpParcel:
                                        GetUpdatePickUpParcel();
                                        break;
                                    case Update.deliveryToCustomer:
                                        GetUpdateDeliveryToCustomer();
                                        break;
                                    case Update.SendToCharge:
                                        GetUpdateSendToCharge();
                                        break;
                                    case Update.releaseDrone:
                                        GetUpdateReleaseDrone();
                                        break;
                                    default:
                                        getdefault();
                                        break;
                                }

                                break;
                            }

                        case Options.view:
                            {
                                //view options
                                Console.WriteLine("to view options for station, choose 1\n" +
                                    "to view options for drone, choose 2\n" +
                                    "to view options for customer, choose 3\n" +
                                    "to view options for parcel, choose 4\n");

                                int chooseViewOptions = int.Parse(Console.ReadLine());

                                switch ((View)chooseViewOptions)
                                {
                                    case View.viewStation:
                                        getviewStation();
                                        break;
                                    case View.viewDrone:
                                        getviewDrone();
                                        break;
                                    case View.viewCustomer:
                                        getviewCustomer();
                                        break;
                                    case View.viewParcel:
                                        getviewParcel();
                                        break;
                                    default:
                                        getdefault();
                                        break;
                                }
                                break;
                            }

                        case Options.listView:
                            {
                                Console.WriteLine("to see the list's view options of the base-station, choose 1\n" +
                                 "to see the list's view options of the drone, choose 2\n" +
                                 "to see the list's view options of the customer, choose 3\n" +
                                 "to see the list's view options of the parcel, choose 4\n");

                                int chooselistView = int.Parse(Console.ReadLine());

                                switch ((ListView)chooselistView)
                                {
                                    case ListView.stationList:
                                        GetListViewStation();
                                        break;

                                    case ListView.droneList:
                                        GetListViewDrone();
                                        break;

                                    case ListView.CustomerList:
                                        GetListViewCustomer();
                                        break;

                                    case ListView.parcelList:
                                        GetListViewParcel();
                                        break;

                                    case ListView.notAttributeParcel:
                                        GetListViewNotScheduleParcel();
                                        break;

                                    case ListView.freeChargeStation:
                                        GetListViewFreeChargeStation();
                                        break;

                                    default:
                                        getdefault();
                                        break;
                                }
                            }
                            break;
                        case Options.exit: break;
                        default:
                            getdefault();
                            break;
                    }

                }
                catch (FormatException) { Console.WriteLine("ERROR"); }
                catch (Exception ex) { Console.WriteLine(ex.ToString()); }
            } while (choice != 5);
        }
    }

}












