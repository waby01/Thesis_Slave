using UnityEngine;
using UnityEngine.UI;

public class MuteButtonVisual : MonoBehaviour
{
    [Header("Sprite Visuals")]
    [SerializeField] private Sprite soundOnSprite;
    [SerializeField] private Sprite soundOffSprite;

    private Image buttonImage;
    private Button buttonComponent;

    private void Awake()
    {
        buttonImage = GetComponent<Image>();
        buttonComponent = GetComponent<Button>();
    }

    private void Start()
    {
        if (buttonComponent != null)
        {
            buttonComponent.onClick.RemoveAllListeners();
            buttonComponent.onClick.AddListener(OnButtonClicked);
        }

        if (AudioManagers.Instance != null)
        {
            AudioManagers.Instance.OnMuteStatusChanged += RefreshSprite;
        }

        RefreshSprite();
    }

    private void OnEnable()
    {
        RefreshSprite();
    }

    private void OnDestroy()
    {
        if (AudioManagers.Instance != null)
        {
            AudioManagers.Instance.OnMuteStatusChanged -= RefreshSprite;
        }
    }

    private void OnButtonClicked()
    {
        if (AudioManagers.Instance != null)
        {
            AudioManagers.Instance.ToggleMute();
        }
    }

    public void RefreshSprite()
    {
        if (buttonImage != null && AudioManagers.Instance != null)
        {
            buttonImage.sprite = AudioManagers.Instance.IsMuted ? soundOffSprite : soundOnSprite;
        }
    }
}