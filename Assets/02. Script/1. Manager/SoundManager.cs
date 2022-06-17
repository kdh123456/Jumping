using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SoundManager : MonoSingleton<SoundManager>
{
    [SerializeField]
    private AudioClip[] BgmClip;
    [SerializeField]
    private AudioClip[] EfectClip;

    private AudioSource bgm = null;
    private AudioSource effectSound = null;

    [SerializeField]
    private Scrollbar bgmScrollbar;
    [SerializeField]
    private Scrollbar effectScrollbar;

    private SOUND_VALUE soundValue;
    private readonly string SAVE_FILENAME = "/SoundValue.txt";

    void Awake()
    {
        TryGetComponent(out bgm);
        this.transform.GetChild(0).TryGetComponent(out effectSound);
        soundValue = GameManager.Instance.LoadJsonFile<SOUND_VALUE>(GameManager.Instance.SAVE_PATH, SAVE_FILENAME);
    }

    void Start()
    {
        //GameManager.Instance.SaveJson<SOUND_VALUE>(GameManager.Instance.SAVE_PATH, SAVE_FILENAME, soundValue);
        if (soundValue != null)
        {
            bgmScrollbar.value = soundValue.bgmSound;
            effectScrollbar.value = soundValue.effectSound;
        }
    }

    public void SetBackGroundSoundClip(BackGroundSoundState state)
    {
        bgm.Stop();
        bgm.clip = BgmClip[(int)state];
        bgm.Play();
    }
    public void SetEffectSoundClip(EffectSoundState state)
    {
        effectSound.Stop();
        effectSound.clip = EfectClip[(int)state];
        effectSound.Play();
    }
    public void bgmSetVolume()
    {
        bgm.volume = bgmScrollbar.value;
    }
    public void effetSetVolume()
    {
        effectSound.volume = effectScrollbar.value;
    }
    //蹂쇰ⅷ媛????

    public void BgmValueSave()
    {
        soundValue.bgmSound = bgmScrollbar.value;
        GameManager.Instance.SaveJson<SOUND_VALUE>(GameManager.Instance.SAVE_PATH, SAVE_FILENAME, soundValue);
    }

    public void EffectValueSave()
    {
        soundValue.effectSound = effectScrollbar.value;
        GameManager.Instance.SaveJson<SOUND_VALUE>(GameManager.Instance.SAVE_PATH, SAVE_FILENAME, soundValue);
    }

    void Update()
    {
        bgmSetVolume();
        effetSetVolume();
    }

    public float GetEffectVolume()
    {
        if (soundValue == null)
            soundValue = GameManager.Instance.LoadJsonFile<SOUND_VALUE>(GameManager.Instance.SAVE_PATH, SAVE_FILENAME);

        return soundValue.effectSound;
    }
}
