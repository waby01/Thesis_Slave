using UnityEngine;
using UnityEngine.SceneManagement;

public class AudioManagers : MonoBehaviour
{
    public static AudioManagers Instance { get; private set; }

    [Header("Audio Components")]
    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource bgmSource;

    [Header("BGM Audio Clips")]
    public AudioClip mainMenuBGM;
    public AudioClip gameplayBGM;

    [Header("SFX Audio Clips")]
    public AudioClip playCardSFX;
    public AudioClip enemyAttackSFX;
    public AudioClip winSFX;
    public AudioClip loseSFX;
    public AudioClip buttonClickSFX;

    public System.Action OnMuteStatusChanged;

    public bool IsMuted { get; private set; } = false;
    private bool isPanelOverridingMute = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        PlayCorrectBGM(SceneManager.GetActiveScene().name);
    }

    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void Update()
    {
        if (UnityEngine.InputSystem.Keyboard.current != null &&
            UnityEngine.InputSystem.Keyboard.current.eKey.wasPressedThisFrame)
        {
            ToggleMute();
        }
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        isPanelOverridingMute = false;
        PlayCorrectBGM(scene.name);

        OnMuteStatusChanged?.Invoke();
    }

    private void PlayCorrectBGM(string sceneName)
    {
        AudioClip targetClip = (sceneName == "MainMenu") ? mainMenuBGM : gameplayBGM;

        if (bgmSource != null) bgmSource.mute = IsMuted;

        if (bgmSource != null && targetClip != null && bgmSource.clip != targetClip)
        {
            bgmSource.Stop();
            bgmSource.clip = targetClip;
            bgmSource.loop = true;
            bgmSource.Play();
        }
    }

    public void ToggleMute()
    {
        IsMuted = !IsMuted;

        if (sfxSource != null) sfxSource.mute = IsMuted;
        if (bgmSource != null) bgmSource.mute = IsMuted;

        if (bgmSource != null && !isPanelOverridingMute)
        {
            if (IsMuted) bgmSource.Pause();
            else bgmSource.UnPause();
        }

        OnMuteStatusChanged?.Invoke();
    }

    public void SetPanelMuteOverride(bool active)
    {
        isPanelOverridingMute = active;

        if (bgmSource != null)
        {
            if (isPanelOverridingMute)
            {
                bgmSource.Pause();
            }
            else
            {
                if (!IsMuted)
                {
                    bgmSource.UnPause();
                }
            }
        }

        OnMuteStatusChanged?.Invoke();
    }

    public void PlaySFX(AudioClip clip)
    {
        if (sfxSource != null && clip != null)
        {
            sfxSource.PlayOneShot(clip);
        }
    }
}