using UnityEngine;
using System.Collections;

public class VibrationManager : MonoBehaviour
{
    private bool bIsVibrated = true;


    public bool bisLiftLoop { get; private set; } = false;

    private bool bPauseVibrated = false;

   
    private Coroutine currentDelayLiftRoutine;
    private Coroutine vibrationLoopRoutine;

    public void OnVibration(bool active)
    {
        bIsVibrated = active;
        if (!active)
            Vibration.Cancel();
    }

    public void OnStopVibration()
    {
        StopLiftLoopVibration();
        Vibration.Cancel();
    }

    public void OnPauseVibration(bool active)
    {
        if (active)
        {
            StopLiftLoopVibration();
            Vibration.Cancel();
        }
    
        bPauseVibrated = active;
    }

    
  
    public void DelayLiftLoopactive(float delay = 0.3f)
    {
        if (currentDelayLiftRoutine != null)
            return;
        currentDelayLiftRoutine = StartCoroutine(LiftLoopDelayed(delay));
    }

    IEnumerator LiftLoopDelayed(float delay)
    {
        yield return new WaitForSecondsRealtime(0.3f);
        bisLiftLoop = false;
        currentDelayLiftRoutine = null;
    }


    public void Play(VibrationEnum type)
    {

        if (!bIsVibrated) return;
       

        Vibration.Cancel();
        StopLiftLoopVibration();

        switch (type)
        {
            case VibrationEnum.ButtonClick:
                Vibration.CreateOneShot(50, 50);
                break;

            case VibrationEnum.AbsorbObject:
                if (bPauseVibrated) return;
                Vibration.CreateOneShot(50, 150);
                break;
            case VibrationEnum.UFOHit:
                if (bPauseVibrated) return;
                Vibration.CreateOneShot(200, 150);
                break;

            case VibrationEnum.LevelUp:
                if (bPauseVibrated) return;
                Vibration.CreateOneShot(150, 150);
                break;

            case VibrationEnum.None:
                break;
        }
        //Debug.Log(type.ToString() + " Vibration Start " + Time.time);
       
    }

    public void StartLiftLoopVibration(float duration = 0.15f, float interval = 0.1f, int amplitude = 10)
    {
        if (!bIsVibrated || bisLiftLoop|| bPauseVibrated) return;

        bisLiftLoop = true;
        vibrationLoopRoutine = StartCoroutine(LoopVibrationCoroutine(duration, interval, amplitude));
    }

    public void StopLiftLoopVibration()
    {
        if (!bisLiftLoop) return;

        //bisLiftLoop = false;

        if (vibrationLoopRoutine != null)
        {
            StopCoroutine(vibrationLoopRoutine);
            //Debug.Log("Stop Lift!! " + Time.time);
            vibrationLoopRoutine = null;
        }

        DelayLiftLoopactive(0.3f);

        //Vibration.Cancel(); // 혹시 남아있는 진동 제거
    }

    private IEnumerator LoopVibrationCoroutine(float duration, float interval, int amplitude)
    {
        WaitForSecondsRealtime delay = new WaitForSecondsRealtime(interval + duration);

        while (bisLiftLoop)
        {
           // Debug.Log("LiftLoop "  + Time.time);
            Vibration.CreateOneShot((long)(duration * 1000f), amplitude); // ms 단위로 변환
            //Debug.Log(interval + duration);
            yield return delay;
        }
    }

}
