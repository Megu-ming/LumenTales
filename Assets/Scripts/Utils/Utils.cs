using UnityEngine.EventSystems;
using System.Collections.Generic;
using UnityEngine;

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

    /// <summary> 레이캐스트하여 얻은 UI에서 컴포넌트 찾아 리턴 </summary>
    public static T RaycastAndGetComponent<T>(List<RaycastResult> rrList, PointerEventData ped) where T : Component
    {
        rrList?.Clear();
        
        EventSystem.current.RaycastAll(ped, rrList);

        if (rrList.Count == 0)
            return null;

        foreach (var rr in rrList)
        {
            var result = rr.gameObject.GetComponent<T>();
            if (result != null)
                return result;
        }

        return null;
    }
}