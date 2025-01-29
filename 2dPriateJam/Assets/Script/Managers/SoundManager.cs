using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    public AudioSource AudioSource;
    public AudioSource OverAudioSource;

    public AudioClip normalBGM;
    public List<AudioClip> battleBGMs;
    public List<AudioClip> critBattleBGMs;
    public AudioClip criticalBGM;
    public AudioClip winBGM;
    public AudioClip loseBGM;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
            Destroy(gameObject);
    }

    public void PlayCritBGM() 
    {
        AudioSource.clip = critBattleBGMs[Random.Range(0, critBattleBGMs.Count)];
        AudioSource.Play();
        OverAudioSource.clip = criticalBGM;
        OverAudioSource.Play();
    }

    public void PlayBattleBGM() 
    {
        AudioSource.clip = battleBGMs[Random.Range(0, battleBGMs.Count)];
        AudioSource.Play();
    }

    public void PlayLoseBGM()
    {
        OverAudioSource.Stop();
        AudioSource.clip = loseBGM;
        AudioSource.Play();       
    }

    public void PlayWinBGM()
    {
        OverAudioSource.Stop();
        AudioSource.clip = winBGM;
        AudioSource.Play();
    }

    public void PlayNormalBGM() 
    {
        AudioSource.clip = normalBGM;
        AudioSource.Play();
    }

    private void Start()
    {
        PlayNormalBGM();
    }
}
