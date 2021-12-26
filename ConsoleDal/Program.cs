using System;
using DalApi;

using DO;

namespace ConsoleDal
{
    #region enums
    public enum Options
    {
        add = 1, update, view, listView, exit, distance
    }

    public enum Add
    {
        addStation = 1, addDrone, receptionNewCustomer, receptionNewParcel
    }

    public enum Update
    {
        attributeParcelToDrone = 1, collectionParcel, deliveryToCustomer, SendToCharge, releaseDrone
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

    class Program
    {
        // accesibility to the entity function
        static IDal dal = DalFactory.GetDal();

        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");




            #region Add Functions


            // adding station's option

            void getAddstation()
            {
                /// get details of the new station 
                Station stationToAdd = new Station();

                Console.WriteLine("Please enter station's id (1 digit)\n");
                stationToAdd.Id = int.Parse(Console.ReadLine());

                Console.WriteLine("Please enter station's name\n");
                stationToAdd.Name = Console.ReadLine();

                Console.WriteLine("Please enter station's longtitude\n");
                stationToAdd.Longtitude = double.Parse(Console.ReadLine());

                Console.WriteLine("Please enter station's lattitude\n");
                stationToAdd.Lattitude = double.Parse(Console.ReadLine());

                Console.WriteLine("Please enter station's number of charge slots\n");
                stationToAdd.FreeChargeSlots = int.Parse(Console.ReadLine());

                dal.AddStation(stationToAdd);
            }

            // adding drone's option
            void getAddDrone()
            {
                ///get details of the new drone 
                Drone droneToAdd = new Drone();

                Console.WriteLine("Please enter drone's id (4 digits)\n");
                droneToAdd.Id = int.Parse(Console.ReadLine());

                Console.WriteLine("Please enter drone's model\n");
                droneToAdd.Model = Console.ReadLine();


                Console.WriteLine("Please enter drone's weight ability (light/medium/heavy)\n");
                droneToAdd.MaxWeight = (WeightCategories)int.Parse(Console.ReadLine());


                dal.AddingDrone(droneToAdd);
            }

            // adding customer's option
            void getReceptionNewCustomer()
            {

                ///get details of the new customer
                Customer customerToAdd = new Customer();

                Console.WriteLine("Please enter customer's id (9 digits)\n");
                customerToAdd.Id = int.Parse(Console.ReadLine());

                Console.WriteLine("Please enter customer's name\n");
                customerToAdd.CustomerName = Console.ReadLine();

                Console.WriteLine("Please enter customer's phone (10 digits)\n");
                customerToAdd.PhoneNumber = Console.ReadLine();

                Console.WriteLine("Please enter customer's longtitude\n");
                customerToAdd.Longtitude = double.Parse(Console.ReadLine());

                Console.WriteLine("Please enter customer's lattitude\n");
                customerToAdd.Lattitude = double.Parse(Console.ReadLine());

                dal.AddingCustomer(customerToAdd);
            }

            // adding parcel's option
            void getReceptionNewParcel()
            {
                /// get details of the new customer
                Parcel parcelToAdd = new Parcel();

                Console.WriteLine("Please enter sender's id (9 digits)\n");
                parcelToAdd.SenderId = int.Parse(Console.ReadLine());

                Console.WriteLine("Please enter target's id (9 digits)\n");
                parcelToAdd.TargetId = int.Parse(Console.ReadLine());

                Console.WriteLine("Please enter parcel's weight (light 1/medium 2/heavy 3)\n");
                parcelToAdd.Weight = (WeightCategories)int.Parse(Console.ReadLine());

                Console.WriteLine("Please enter parcel's priority (regular 1/fast 2/emergency 3)\n");
                parcelToAdd.Priority = (Priorities)int.Parse(Console.ReadLine());


                Console.WriteLine("Please enter scheduled drone's id (4 digits)\n");
                parcelToAdd.DroneId = int.Parse(Console.ReadLine());

                Console.WriteLine("Please enter parcel's request time\n");
                parcelToAdd.RequestedTime = DateTime.Parse(Console.ReadLine());

                Console.WriteLine("Please enter parcel's Scheduled time\n");
                parcelToAdd.ScheduledTime = DateTime.Parse(Console.ReadLine());

                Console.WriteLine("Please enter parcel's picked Up time\n");
                parcelToAdd.PickedUpTime = DateTime.Parse(Console.ReadLine());

                Console.WriteLine("Please enter parcel's delivered time\n");
                parcelToAdd.DeliveredTime = DateTime.Parse(Console.ReadLine());

                dal.AddingParcel(parcelToAdd);
            }

            #endregion



            #region Update Functions
            ///update scheduled drone to parcel
            void GetUpdateAttributeParcel()
            {
                Console.WriteLine("Please enter the package's number scheduled\n");
                int MyParcelId = int.Parse(Console.ReadLine());

                Console.WriteLine("Please enter drone id\n");
                int MyDroneId = int.Parse(Console.ReadLine());

                dal.UpdateSchdule(MyDroneId, MyParcelId);
            }


            /// update collection parcel
            void GetUpdateCollectionParcel()
            {
                Console.WriteLine("Please enter the package's number picked up\n");
                int MyParcelId = int.Parse(Console.ReadLine());

                dal.UpdatePickUp(MyParcelId);
            }


            /// update delivery to customer
            void GetUpdateDeliveryToCustomer()
            {
                Console.WriteLine("Please enter the package's number delivered\n");
                int MyParcelId = int.Parse(Console.ReadLine());

                dal.UpdateDelivery(MyParcelId);
            }

            ///update send to charge
            void GetUpdateSendToCharge()
            {
                int MyStationId, MyDroneId;

                Console.WriteLine("Please enter drone id\n");
                MyDroneId = int.Parse(Console.ReadLine());

                Console.WriteLine("Please choose station id\n");
                MyStationId = int.Parse(Console.ReadLine());

                dal.UpdateDroneChargeCheckIn(MyDroneId, MyStationId);
            }


            //update releasing drone
            void GetUpdateReleaseDrone()
            {
                int MyStationId, MyDroneId;

                Console.WriteLine("Please enter drone id\n");
                while (!int.TryParse(Console.ReadLine(), out MyDroneId)) ;

                Console.WriteLine("Please enter station id\n"); //האם צריך לקבל גם תחנה
                while (!int.TryParse(Console.ReadLine(), out MyStationId)) ;

                dal.UpdateDroneChargeCheckout(MyDroneId, MyStationId);
            }

            #endregion



            #region Display Single Entity Functions
            // view station's detail
            void getviewStation()
            {
                Console.WriteLine("Please enter station's id\n");          //ask the user to enter id number
                int StationId = int.Parse(Console.ReadLine());
                Console.WriteLine(dal.GetStation(StationId).ToString());
            }

            // view drone's detail 
            void getviewDrone()
            {
                Console.WriteLine("Please enter drone's id (4 digits)\n");          //ask the user to enter id number
                int DroneId = int.Parse(Console.ReadLine());
                Console.WriteLine(dal.GetDrone(DroneId).ToString());
            }

            // view customer's detail
            void getviewCustomer()
            {
                Console.WriteLine("Please enter customer's id (9 digits)\n");          //ask the user to enter id number
                int CustomerId = int.Parse(Console.ReadLine());
                Console.WriteLine(dal.GetCustomer(CustomerId).ToString());
            }

            // view parcel's detail
            void getviewParcel()
            {
                Console.WriteLine("Please enter parcel's id (5 digits)\n");          //ask the user to enter id number
                int ParcelId = int.Parse(Console.ReadLine());
                Console.WriteLine(dal.GetParcel(ParcelId).ToString());
            }

            #endregion



            #region Display List Fanctions
            //view list's detail

            //view station's list
            void GetListViewStation()
            {
                Console.WriteLine("Station list:\n");

                foreach (Station s in dal.GetStationsList())
                {
                    Console.WriteLine(s.ToString());
                }
            }

            // view drone's list
            void GetListViewDrone()
            {
                Console.WriteLine("Drone list:\n");

                foreach (Drone d in dal.GetDronesList())
                {
                    Console.WriteLine(d.ToString());
                }
            }

            //view customer's list
            void GetListViewCustomer()
            {
                Console.WriteLine("Customer list:\n");

                foreach (Customer c in dal.GetCustomersList())
                {
                    Console.WriteLine(c.ToString());
                }
            }

            //view parcel's list
            void GetListViewParcel()
            {
                Console.WriteLine("Parcel list:\n");

                foreach (Parcel p in dal.GetParcelsList())
                {
                    Console.WriteLine(p.ToString());
                }
            }


            //view unattributed parcel's list
            void GetListViewNotAttributeParcel()
            {
                Console.WriteLine("Not attribute parcel's list:\n");

                foreach (Parcel p in dal.GetNotAttributeParcelsList())
                {
                    Console.WriteLine(p.ToString());
                }
            }

            //view free bases-sation's list
            void GetListViewFreeChargeStation()
            {
                Console.WriteLine("free charge station's list:\n");

                foreach (Station s in dal.GetListFreeChargeStationList())
                {
                    Console.WriteLine(s.ToString());
                }
            }
            #endregion


            // default function
            void getdefault()
            {
                Console.WriteLine("This option does not exist");
            }

            

            // initialize function
            Dal.DalObject.DalObject.DataSource.Initialize();

        }
    }
}
