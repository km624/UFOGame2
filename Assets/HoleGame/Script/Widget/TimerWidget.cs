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

    
    public void SetTime(int Min,int Second)
    {
        TotalSecond = Min*60 + Second;
        UpdateTimeText(TotalSecond);
    }
    void Start()
    {
        
    }

    
    void Update()
    {
        
    }

    public void StartTimer()
    {
        StartCoroutine("TimerCount");
    }

    IEnumerator TimerCount()
    {

        while (TotalSecond > 0)
        {
            yield return new WaitForSeconds(1); 
            TotalSecond--;
            if (TotalSecond < 0)
                TotalSecond = 0;
            UpdateTimeText(TotalSecond);
        }


        //타이머 종료
        //게임 종료
        GameState.Instance.GameEnd();
    }
    public void UpdateTimeText(int total)
    {
        int min = TotalSecond / 60;
        int sec = TotalSecond % 60;
        Minute.text = min.ToString("D2")+" :";
        Second.text = sec.ToString("D2");
    }

    public int GetTotalSecond()
    {
        return TotalSecond;
    }

    public void StopTime()
    {
        StopCoroutine("TimerCount");
    }

    public void ChangeTimer(int totalsecond)
    {
        
         TotalSecond = totalsecond;
        if (totalsecond < 0) TotalSecond = 0;
        UpdateTimeText(TotalSecond);

    }

}

