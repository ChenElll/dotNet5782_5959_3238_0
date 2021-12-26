using System;
using System.Reflection;
using System.Collections;

namespace BO
{
	static class ToolsBL
    {
        public static string ToStringProperty<T>(this T t, string suffix = "")
        {
            string str = "";
            foreach (PropertyInfo prop in t.GetType().GetProperties())
            {
                var value = prop.GetValue(t, null);
                if (value is not string && value is IEnumerable)
                    foreach (var item in (IEnumerable)value)
                        str += item.ToStringProperty("   ");
                else
                    str += "\n" + suffix + prop.Name + ": " + value;
            }
            return str;
        }

        public static void CopyPropertiesTo<S, T>(this S from, T to)
        {
            var fromType = from.GetType();
            foreach (PropertyInfo propTo in to.GetType().GetProperties())
            {
                PropertyInfo propFrom = fromType.GetProperty(propTo.Name);
                if (propFrom == null)
                    continue;
                var value = propFrom.GetValue(from, null);
                if (value is ValueType || value is string)
                    propTo.SetValue(to, value);
                else
                {
                    if (value == null)
                        continue;
                    var target = propTo.GetValue(to, null);
                    if (target == null)
                        target = Activator.CreateInstance(propTo.PropertyType);

                    // If the property is a collection...
                    if (value is IEnumerable)
                    {
                        Type itemType = propTo.PropertyType.GetGenericArguments()[0];
                        propTo.PropertyType.GetMethod("Clear").Invoke(target, null);
                        foreach (var item in (value as IEnumerable))
                        {
                            var targetItem = Activator.CreateInstance(itemType);
                            item.CopyPropertiesTo(targetItem);
                            propTo.PropertyType.GetMethod("Add").Invoke(target, new object[] { targetItem });
                        }
                    }
                    else
                        value.CopyPropertiesTo(target);
                }
            }
        }


        public static object CopyPropertiesToNew<S>(this S from, Type type)//get the typy we want to copy to 
        {
            object to = Activator.CreateInstance(type); // new object of the Type
            from.CopyPropertiesTo(to);//copy all value of properties with the same name to the new object
            return to;
        }
    }
}
