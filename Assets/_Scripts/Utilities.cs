using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities : MonoBehaviour
{
    /// <summary>
    /// Returns an int that "wraps around" from zero to max, including both. 
    /// </summary>
    public static int IntWrapAround(int target, int maxInclusive)
    {
        if (maxInclusive + 1 < 1) return 0;

        target = target % (maxInclusive + 1);

        if (target < 0)
        {
            target += (maxInclusive + 1);
        }

        return target;
    }
}
