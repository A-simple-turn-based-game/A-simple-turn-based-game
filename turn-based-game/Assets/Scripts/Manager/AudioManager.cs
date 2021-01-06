using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : BaseManager
{
    public AudioManager(GameRoot GR) : base(GR)
    {
    }
     
    private AudioSource m_BgAudioSource;
    private AudioSource m_NormalAudioSource;//互动音效
    
    public override void OnInit()
    {
        base.OnInit();
        GameObject audioSourceManager = new GameObject("AudioSource(GameObject)");
        audioSourceManager.transform.SetParent(gameRoot.transform,false);
        m_BgAudioSource = audioSourceManager.AddComponent<AudioSource>();
        m_NormalAudioSource = audioSourceManager.AddComponent<AudioSource>(); 
    }
    public void PlaySound(AudioSource audioClip, AudioClip clip, bool loop = false)
    { 
        audioClip.clip = clip;
        audioClip.loop = loop;
        audioClip.Play(); 
    }

    /// <summary>
    /// 使用默认位置播放
    /// </summary>
    /// <param name="clip"></param>
    /// <param name="loop"></param>
    public void PlaySound(AudioClip clip, bool loop = false)
    {
        m_NormalAudioSource.clip = clip;
        m_NormalAudioSource.loop = loop;
        m_NormalAudioSource.Play();
    }

    public void PlayBgSound(AudioClip clip, bool loop = true)
    {
        m_BgAudioSource.clip = clip;
        m_BgAudioSource.loop = loop;
        m_BgAudioSource.Play();
    }



}
