using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
   
    [SerializeField] private AudioSource bgmSource;
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private List<SfxEntry> sfxList;

    private Dictionary<SoundEnum, AudioClip> sfxMap =new Dictionary<SoundEnum, AudioClip>();

    private bool isBgmMuted = false;
    private bool isSfxMuted = false;

    private bool isSoundPuased = false;

    private AudioClip CurrentBGM;
   

#if UNITY_EDITOR
    private void OnValidate()
    {
        // Enum 전체 순회
        var enumValues = System.Enum.GetValues(typeof(SoundEnum)).Cast<SoundEnum>().ToList();

        // 누락된 항목 추가
        foreach (var enumValue in enumValues)
        {
            if (!sfxList.Any(e => e.type == enumValue))
            {
                sfxList.Add(new SfxEntry
                {
                    type = enumValue,
                    clip = null
                });
            }
        }

        // 중복 제거 (동일 type 중 첫 번째만 유지)
        sfxList = sfxList
            .GroupBy(e => e.type)
            .Select(g => g.First())
            .ToList();

        UnityEditor.EditorUtility.SetDirty(this); // 인스펙터 자동 저장 갱신
    }
#endif

    private void Awake()
    {

        InitSoundManager();
    }

    private void InitSoundManager()
    {
        sfxMap.Clear();
        sfxMap = new Dictionary<SoundEnum, AudioClip>();
        foreach (var entry in sfxList)
            if (!sfxMap.ContainsKey(entry.type))
                sfxMap.Add(entry.type, entry.clip);
    }

    public void PlayBgm(SoundEnum type, float fadeTime = 0.5f, bool loop = true )
    {
        if (isBgmMuted) return;
        if(isSoundPuased) return;

    
        if (sfxMap.TryGetValue(type, out var clip))
        {
            StartCoroutine(FadeBgmCoroutine(clip, fadeTime, loop));
            CurrentBGM = clip;
        }
           
    }

    public void PlayBGMOneShot(SoundEnum type, float fadeTime = 0.5f, bool loop = true)
    {
        if (isBgmMuted) return;
        if (isSoundPuased) return;


        if (sfxMap.TryGetValue(type, out var clip))
        {
            StartCoroutine(FadeBgmCoroutine(clip, fadeTime, loop));
           
        }
    }

    public void PlayBgm(AudioClip next, float fadeTime = 0.5f, bool loop = true)
    {
        if (isBgmMuted) return;
        if (isSoundPuased) return;

        StartCoroutine(FadeBgmCoroutine(next, fadeTime, loop));
        CurrentBGM = next;
    }

    public void ResumeBgm(float fadeTime = 0.5f, bool loop = true)
    {
        if (isBgmMuted) return;
        if (isSoundPuased) return;

        if(CurrentBGM!=null)
            StartCoroutine(FadeBgmCoroutine(CurrentBGM, fadeTime, loop));
    }


    public void StopBgm(float fadeTime = 0.5f)
    {
        StartCoroutine(FadeOutBgmCoroutine(fadeTime));
    }

    private System.Collections.IEnumerator FadeBgmCoroutine(AudioClip next, float time, bool loop)
    {
       
        float startVol = bgmSource.volume; // 현재 볼륨을 읽음

        // 페이드아웃
        for (float t = 0; t < time; t += Time.unscaledDeltaTime)
        {
            float volume = Mathf.Lerp(startVol, 0f, t / time);
            SetBgmVolume(volume);
            yield return null;
        }

        SetBgmVolume(0f); // 완전히 0으로

        // 다음 트랙 세팅
        bgmSource.Stop();
        bgmSource.clip = next;
        bgmSource.loop = loop;
        bgmSource.Play();

        // 페이드인
        for (float t = 0; t < time; t += Time.unscaledDeltaTime)
        {
            float volume = Mathf.Lerp(0f, startVol, t / time);
            SetBgmVolume(volume);
            yield return null;
        }

        SetBgmVolume(startVol); // 정확히 원래 볼륨으로 복구
    }
    private IEnumerator FadeOutBgmCoroutine(float time)
    {
        float startVol = bgmSource.volume;

        for (float t = 0; t < time; t += Time.unscaledDeltaTime)
        {
            SetBgmVolume(Mathf.Lerp(startVol, 0f, t / time));
            yield return null;
        }

        bgmSource.Stop();
        SetBgmVolume(startVol);
    }

    private void SetBgmVolume(float volume)
    {
        bgmSource.volume = Mathf.Clamp01(volume);
    }

    public void PlaySfx(SoundEnum type, float volume = 1f)
    {
        if (isSfxMuted) return;
        if (sfxMap.TryGetValue(type, out var clip))
            sfxSource.PlayOneShot(clip, volume);
    }
    public void PlaySfxLoop(SoundEnum type, float volume = 1f)
    {
        if (isSfxMuted) return;
        if (sfxMap.TryGetValue(type, out var clip))
        {
            sfxSource.clip = clip;
            sfxSource.loop = true;
            sfxSource.volume = volume;
            sfxSource.Play();
        }
    }

    public void StopSfxLoop()
    {
        sfxSource.loop = false;
        sfxSource.Stop();
        sfxSource.clip = null;
    }
    
    public void OnSoundPauseActive(bool bpause)
    {
       
        if (bpause)
        {
            
            bgmSource.Pause();
            sfxSource.Pause();
        }
        else
        {
          
            bgmSource.UnPause();
            sfxSource.UnPause();
        }
        isSoundPuased = bpause;
    }

    public void SetBgmMute(bool mute)
    {
        isBgmMuted = mute;
        bgmSource.mute = mute;

      /*  if (mute)
            bgmSource.Pause();
        else
            bgmSource.UnPause();*/
    }

    public void SetSfxMute(bool mute)
    {
        isSfxMuted = mute;
        sfxSource.mute = mute;

       /* if (mute)
            sfxSource.Pause();
        else
            sfxSource.UnPause();*/
    }

    [System.Serializable]
    public struct SfxEntry
    {
        public SoundEnum type;
        public AudioClip clip;
    }
}
