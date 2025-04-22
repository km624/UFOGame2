using UnityEngine;

public class VibrationManager : MonoBehaviour
{
    private bool bIsVibrated = true;

    private Vibration vibration;
    public void Awake()
    {
        
    }

    public void OnVibration(bool active)
    {
        bIsVibrated = active;
    }

    public void Play(VibrationEnum type)
    {
        
        if (!bIsVibrated) return;
        switch (type)
        {
            case VibrationEnum.ButtonClick:
                
                Vibration.CreateOneShot(100, 50);
                break;

            case VibrationEnum.AbsorbObject:
                Vibration.CreateOneShot(60, 200);
                break;

            case VibrationEnum.UFOHit:
                Vibration.CreateOneShot(300, 255);
                break;

            case VibrationEnum.LevelUp:
                long[] timings = { 0, 100, 50, 150 };
                int[] amplitudes = { 0, 200, 0, 255 };
                Vibration.CreateWaveform(timings, amplitudes, -1);
                break;

            case VibrationEnum.Custom:
                // 따로 제공되는 방식이 있을 때 쓰기
                break;
        }
    }

}
