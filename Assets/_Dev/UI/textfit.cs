using UnityEngine;
using static UnityEngine.Rendering.DebugUI.Table;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [Header("Audio Sources")]
    public AudioSource musicSource;       // Background music
    public AudioSource sfxSource;         // UI sounds
    public AudioSource npcTalkingSource;  // NPC dialogue

    [Header("Volume Settings")]
    [Range(0f, 1f)] public float musicVolume = 1f;
    [Range(0f, 1f)] public float sfxVolume = 1f;
    [Range(0f, 1f)] public float npcVolume = 1f;

    [Header("UI Clips")]
    public AudioClip buttonClick;
    public AudioClip sliderMove;
    public AudioClip arrowClick;

    private void Awake()
    {
        // Singleton pattern
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        musicSource.volume = musicVolume;
        sfxSource.volume = sfxVolume;
        npcTalkingSource.volume = npcVolume;
    }

    #region Music
    public void PlayMusic(AudioClip clip, bool loop = true)
    {
        if (musicSource.clip == clip) return;

        musicSource.clip = clip;
        musicSource.loop = loop;
        musicSource.volume = musicVolume;
        musicSource.Play();
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void SetMusicVolume(float volume)
    {
        musicVolume = volume;
        musicSource.volume = volume;
    }
    #endregion

    #region UI Sounds
    public void PlayButtonClick() => PlaySFX(buttonClick);
    public void PlaySliderMove() => PlaySFX(sliderMove);
    public void PlayArrowClick() => PlaySFX(arrowClick);

    public void PlaySFX(AudioClip clip)
    {
        if (clip == null) return;
        sfxSource.PlayOneShot(clip, sfxVolume);
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = volume;
        sfxSource.volume = volume;
    }
    #endregion

    #region NPC Sounds
    public void PlayNPCSound(AudioClip clip)
    {
        if (clip == null) return;
        npcTalkingSource.clip = clip;
        npcTalkingSource.volume = npcVolume;
        npcTalkingSource.Play();
    }

    public void SetNPCVolume(float volume)
    {
        npcVolume = volume;
        npcTalkingSource.volume = volume;
    }
    #endregion
}

///

// How to use it

//Play background music:

//AudioManager.Instance.PlayMusic(backgroundMusicClip);

//Play UI sounds:

//AudioManager.Instance.PlayButtonClick();
//AudioManager.Instance.PlaySliderMove();
//AudioManager.Instance.PlayArrowClick();

//Play NPC dialogue:

//AudioManager.Instance.PlayNPCSound(npcClip);

//Adjust volume at runtime (optional):

//AudioManager.Instance.SetMusicVolume(0.5f);
//AudioManager.Instance.SetSFXVolume(0.8f);
//AudioManager.Instance.SetNPCVolume(1f);