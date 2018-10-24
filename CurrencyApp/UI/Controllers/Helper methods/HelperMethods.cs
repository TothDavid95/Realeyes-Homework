using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UI.Controllers.Helper_methods
{
    public static class HelperMethods
    {
        public static int IncludesAt(this string str, string[] strToCompare)
        {
            for (int i = 0; i < strToCompare.Length; i++)
            {
                if (String.Compare(strToCompare[i], str) == 0)
                {
                    return i;
                }
            }
            return -1;
        }

        public static long ConvertToUnixTime(this string str)
        {
            string[] tempData = str.Split('-');
            DateTime date = new DateTime(Int32.Parse(tempData[0]), Int32.Parse(tempData[1]), Int32.Parse(tempData[2]));
            DateTimeOffset timeOffset = new DateTimeOffset(date);
            return timeOffset.ToUnixTimeSeconds();
        }
    }
}