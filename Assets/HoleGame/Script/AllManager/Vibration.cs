using UnityEngine;
using System.Collections;

public class Vibration : MonoBehaviour {

    public static AndroidJavaClass unityPlayer;
    public static AndroidJavaObject vibrator;
    public static AndroidJavaObject currentActivity;
    public static AndroidJavaClass vibrationEffectClass;
    public static int defaultAmplitude;

    /*
     * "CreateOneShot": One time vibration
     * "CreateWaveForm": Waveform vibration
     * 
     * Vibration Effects class (Android API level 26 or higher)
     * Milliseconds: long: milliseconds to vibrate. Must be positive.
     * Amplitude: int: Strenght of vibration. Between 1-255. (Or default value: -1)
     * Timings: long: If submitting a array of amplitudes, then timings are the duration of each of these amplitudes in millis.
     * Repeat: int: index of where to repeat, -1 for no repeat
     */

    
    void OnEnable() {
#if UNITY_ANDROID && !UNITY_EDITOR
        unityPlayer = new AndroidJavaClass("com.unity3d.player.UnityPlayer");
        currentActivity = unityPlayer.GetStatic<AndroidJavaObject>("currentActivity");
        vibrator = currentActivity.Call<AndroidJavaObject>("getSystemService", "vibrator");
        if (getSDKInt() >= 26) {
            vibrationEffectClass = new AndroidJavaClass("android.os.VibrationEffect");
            defaultAmplitude = vibrationEffectClass.GetStatic<int>("DEFAULT_AMPLITUDE");
        }
#endif
    }

    //Works on API > 25
    public static void CreateOneShot(long milliseconds) {
        
        if(isAndroid()) {
            //If Android 8.0 (API 26+) or never use the new vibrationeffects
            if (getSDKInt() >= 26) {
                CreateOneShot(milliseconds, defaultAmplitude);
            }
            else {
                OldVibrate(milliseconds);
            }
        }
        //If not android do simple solution for now
        else {
            Handheld.Vibrate();
        }   
    }

    public static void CreateOneShot(long milliseconds, int amplitude) {

       
        if (isAndroid()) {
            //If Android 8.0 (API 26+) or never use the new vibrationeffects
            if (getSDKInt() >= 26) {
                CreateVibrationEffect("createOneShot", new object[] { milliseconds, amplitude });
            }
            else {
                OldVibrate(milliseconds);
            }
        }
        //If not android do simple solution for now
        else {
            //Handheld.Vibrate();
        }
    }

    //Works on API > 25
    public static void CreateWaveform(long[] timings, int repeat) {
        //Amplitude array varies between no vibration and default_vibration up to the number of timings
        
        if (isAndroid()) {
            //If Android 8.0 (API 26+) or never use the new vibrationeffects
            if (getSDKInt() >= 26) {
                CreateVibrationEffect("createWaveform", new object[] { timings, repeat });
            }
            else {
                OldVibrate(timings, repeat);
            }
        }
        //If not android do simple solution for now
        else {
            Handheld.Vibrate();
        }
    }

    public static void CreateWaveform(long[] timings, int[] amplitudes, int repeat) {
        if (isAndroid()) {
            //If Android 8.0 (API 26+) or never use the new vibrationeffects
            if (getSDKInt() >= 26) {
                CreateVibrationEffect("createWaveform", new object[] { timings, amplitudes, repeat });
            }
            else {
                OldVibrate(timings, repeat);
            }
        }
        //If not android do simple solution for now
        else {
            Handheld.Vibrate();
        }

    }

    //Handels all new vibration effects
    private static void CreateVibrationEffect(string function, params object[] args) {

        AndroidJavaObject vibrationEffect = vibrationEffectClass.CallStatic<AndroidJavaObject>(function, args);
        vibrator.Call("vibrate", vibrationEffect);
    }

    //Handles old vibration effects
    private static void OldVibrate(long milliseconds) {
        vibrator.Call("vibrate", milliseconds);
    }
    private static void OldVibrate(long[] pattern, int repeat) {
        vibrator.Call("vibrate", pattern, repeat);
    }

    public static bool HasVibrator() {
        return vibrator.Call<bool>("hasVibrator");
    }

    public static bool HasAmplituideControl() {
        if (getSDKInt() >= 26) {
            return vibrator.Call<bool>("hasAmplitudeControl"); //API 26+ specific
        }
        else {
            return false; //If older than 26 then there is no amplitude control at all
        }

    }

    public static void Cancel() {
        if (isAndroid())
            vibrator.Call("cancel");

       
    }

    private static int getSDKInt() {
        if(isAndroid()) {
            using (var version = new AndroidJavaClass("android.os.Build$VERSION")) {
                return version.GetStatic<int>("SDK_INT");
            }
        }
        else {
            return -1;
        }
        
    }

    private static bool isAndroid() {
#if UNITY_ANDROID && !UNITY_EDITOR
	    return true;
#else
        return false;
#endif
    }
}