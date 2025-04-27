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
        // Enum ��ü ��ȸ
        var enumValues = System.Enum.GetValues(typeof(SoundEnum)).Cast<SoundEnum>().ToList();

        // ������ �׸� �߰�
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

        // �ߺ� ���� (���� type �� ù ��°�� ����)
        sfxList = sfxList
            .GroupBy(e => e.type)
            .Select(g => g.First())
            .ToList();

        UnityEditor.EditorUtility.SetDirty(this); // �ν����� �ڵ� ���� ����
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
       
        float startVol = bgmSource.volume; // ���� ������ ����

        // ���̵�ƿ�
        for (float t = 0; t < time; t += Time.unscaledDeltaTime)
        {
            float volume = Mathf.Lerp(startVol, 0f, t / time);
            SetBgmVolume(volume);
            yield return null;
        }

        SetBgmVolume(0f); // ������ 0����

        // ���� Ʈ�� ����
        bgmSource.Stop();
        bgmSource.clip = next;
        bgmSource.loop = loop;
        bgmSource.Play();

        // ���̵���
        for (float t = 0; t < time; t += Time.unscaledDeltaTime)
        {
            float volume = Mathf.Lerp(0f, startVol, t / time);
            SetBgmVolume(volume);
            yield return null;
        }

        SetBgmVolume(startVol); // ��Ȯ�� ���� �������� ����
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
