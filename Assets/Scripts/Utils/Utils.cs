using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public static class Utils
{
    public static float Percent(int val, int max)
    {
        if (max == 0) return 0;
        return ((float)val / max);
    }

    public static float Percent(float val, float max)
    {
        if (max == 0) return 0;
        return (val / max);
    }
}