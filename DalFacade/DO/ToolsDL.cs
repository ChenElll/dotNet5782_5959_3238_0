using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;


namespace DO
{
    public static class ToolsDL
    {
        public static IEnumerable<T> ListFilter<T>(this IEnumerable<T> colection, Predicate<T> predicate)
        {

            return colection.ToList().FindAll(predicate);
        }


    }


    public static class ToStringDL
    {
        public static string ToStringProperty<T>(this T t)
        {
            string str = "";
            foreach (PropertyInfo item in t.GetType().GetProperties())
                str += "\n" + item.Name + ": " + item.GetValue(t, null);
            return str;
        }
    }
}
