using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace eegs_back_end.Helpers
{
    public class ObjectSqlMapping
    {

        public static string MapInsert<T>()
        {
            string returnString = "";
            PropertyInfo[] formProperty;
            formProperty = typeof(T).GetProperties();
            string objString = "";
            string sqlString = "";
            foreach (PropertyInfo prop in formProperty)
            {
                if (!prop.PropertyType.Name.Contains("List`1") && prop.PropertyType.IsSerializable)
                {
                    if (objString == "" && sqlString == "")
                    {
                        objString = prop.Name.Insert(0, "@");
                        sqlString = "`" + prop.Name + "`";
                    }
                    else
                    {
                        objString = objString + ", " + prop.Name.Insert(0, "@");
                        sqlString = sqlString + ", `" + prop.Name + "`";
                    }
                }
            }

            returnString = sqlString + ") VALUES (" + objString + "";
            return returnString;

            
        }

        public static string MapUpdate<T>()
        {
            PropertyInfo[] formProperty;
            formProperty = typeof(T).GetProperties();
            string objString = "";

            foreach (PropertyInfo prop in formProperty)
            {
                if (!(prop.PropertyType.Name.Contains("List`1")) && prop.PropertyType.IsSerializable)
                {
                    if (objString == "")
                    {
                        objString = prop.Name + " = " + prop.Name.Insert(0, "@");
                    }
                    else
                    {
                        objString = objString + ", " + prop.Name + " = " + prop.Name.Insert(0, "@");
                    }
                }
            }


            return objString;
        }

        public static string SetParam(string[] param)
        {
            string result = "";
            for (int i = 0; i < param.Length; i++)
            {
                if (result == "")
                {
                    result = "'" + param[i] + "'";
                }
                else
                {
                    result = result + ", '" + param[i] + "'";
                }
            }

            return result;
        }

    }
}
