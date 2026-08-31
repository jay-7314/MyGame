using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class CoroutineData
{
    #region private 변수

    private static Dictionary<float, WaitForSeconds> DicWaitForSeconds = new Dictionary<float, WaitForSeconds>();

    #endregion

    #region public 변수
    

    #endregion

   
    public static WaitForSeconds GetWaitForSeconds(float seconds)
    {
        if (DicWaitForSeconds.ContainsKey(seconds) == false)
        {
            DicWaitForSeconds.Add(seconds, new WaitForSeconds(seconds));
        }
        
        return DicWaitForSeconds[seconds];
    }
}
