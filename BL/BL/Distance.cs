using System;
using BlApi;
using DalApi;
using System.Collections.Generic;
using System.Linq;
using BO;

namespace BL
{
    partial class BL
    {
        #region CalculateDistance
        internal double Distance(Location location1, Location location2)
        {
            //a = sin²(Δφ / 2) + cos φ1 ⋅ cos φ2 ⋅ sin²(Δλ / 2)
            //c = 2 ⋅ atan2( √a, √(1−a) )
            //d = R ⋅ c
            //φ= lattitude
            //λ=longitude
            //R= radius of earth =6371 km
            //להמיר ממעלות לרדיאנים
            double radianLat1 = (Math.PI * location1.Lattitude) / 180; //math.pi מייצג את היחס של המעגל לקוטרו                                                        
            double radianLat2 = (Math.PI * location2.Lattitude) / 180;
            double radianLong1 = (Math.PI * location1.Longtitude) / 180;
            double radianLong2 = (Math.PI * location2.Longtitude) / 180;
            double theta = location1.Longtitude - location2.Longtitude;
            double radianTheta = Math.PI * theta / 180;
            double dist =
                Math.Sin(radianLat1) * Math.Sin(radianLat2) + Math.Cos(radianLat1) *
                Math.Cos(radianLat2) * Math.Cos(radianTheta);

            dist = dist * 180 / Math.PI;
            dist = dist * 60 * 1.1515;

            return dist;

        }
        #endregion
    }
}