using Daemon.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1
{
    public static class Extensions
    {
        public static bool IsEqual<T>(this T objA, T objB)
        {

            if (objA == null && objB == null)
                return true;
            if (objA == null || objB == null)
                return false;

            foreach (var item in objA.GetType().GetProperties())
            {
                if (item.GetValue(objA)!.ToString() != item.GetValue(objB)!.ToString())
                    return false;
            }

            return true;
        }
        public static bool IsEqualConfig(this Config configA, Config configB)
        {
            if (!configA.IsEqual(configB))
                return false;

            if (!(configA.Sources!.All(s1 => configB.Sources!.Any(s2 => s1.IsEqual(s2)))))
                return false;
            if(!(configA.Destinations!.All(s1 => configB.Destinations!.Any(s2 => s1.IsEqual(s2)))))
                return false;

            return true;
        }
    }
}

