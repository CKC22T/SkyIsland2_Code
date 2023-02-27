using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    public class GenericUtil
    {
        static public void Swap<T>(ref T t0, ref T t1) where T : class
        {
            T temp = t0;
            t0 = t1;
            t1 = temp;

            return;
        }
    }
}