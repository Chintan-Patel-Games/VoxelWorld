using UnityEngine;
using VoxelWorld.Core.Utilities;
using VoxelWorld.Sound;

public class GlobalSoundService : GenericMonoSingleton<GlobalSoundService>
{
    public SoundService SoundService { get; private set; }

    [SerializeField] private SoundSO soundSO;

    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(gameObject);

        SoundService = new SoundService(soundSO);
        LoadVolumes();
    }

    public void RegisterAudioSources(AudioSource sfx, AudioSource bgm) =>
        SoundService.AssignAudioSources(sfx, bgm);

    public static float MusicVolume
    {
        get => Instance.SoundService.MusicVolume;
        set => Instance.SoundService.MusicVolume = value;
    }

    public static float SFXVolume
    {
        get => Instance.SoundService.SFXVolume;
        set => Instance.SoundService.SFXVolume = value;
    }

    public static void SaveVolumes()
    {
        PlayerPrefs.SetFloat("BGM_VOLUME", MusicVolume);
        PlayerPrefs.SetFloat("SFX_VOLUME", SFXVolume);
    }

    public static void LoadVolumes()
    {
        MusicVolume = PlayerPrefs.GetFloat("BGM_VOLUME", 0.8f);
        SFXVolume = PlayerPrefs.GetFloat("SFX_VOLUME", 0.8f);
    }
}