using UnityEngine;
using TMPro;
using System.Collections;
using static UnityEngine.InputSystem.LowLevel.InputStateHistory;
using NUnit.Framework.Constraints;

public class TimerWidget : MonoBehaviour
{
    public TMP_Text Minute;
    public TMP_Text Second;

    private int TotalSecond;

    
    public void SetTime(float totaltime)
    {
        TotalSecond = (int)totaltime;
        CallBack_UpdateTimeText(TotalSecond);
    }
   
    public void CallBack_UpdateTimeText(int total)
    {
        int min = total / 60;
        int sec = total % 60;
        Minute.text = min.ToString("D2")+" :";
        Second.text = sec.ToString("D2");
        
    }

  
}

