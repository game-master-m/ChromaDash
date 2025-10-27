using UnityEngine;
using UnityEngine.UI;

public class TimeGaugeUI : MonoBehaviour
{
    [SerializeField] private Image currentImage;

    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite blueSprite;
    [SerializeField] private Sprite greenSprite;

    private void Awake()
    {
        if (currentImage == null) currentImage = GetComponent<Image>();
    }
    private void OnEnable()
    {
        GameEvents.OnTimeGaugeChanged += UpdateGaugeLength;
        GameEvents.OnChromaColorChanged += UpdateGaugeColor;
    }
    private void OnDisable()
    {
        GameEvents.OnTimeGaugeChanged -= UpdateGaugeLength;
        GameEvents.OnChromaColorChanged -= UpdateGaugeColor;
    }

    private void UpdateGaugeLength(float current, float max)
    {
        if (currentImage == null) return;
        if (max <= 0)
        {
            currentImage.fillAmount = 0;
            return;
        }
        float ratio = current / max;
        currentImage.fillAmount = ratio;
    }
    private void UpdateGaugeColor(EChromaColor eChromaColor)
    {
        if (currentImage == null) return;
        switch (eChromaColor)
        {
            case EChromaColor.Red:
                currentImage.sprite = redSprite;
                break;
            case EChromaColor.Blue:
                currentImage.sprite = blueSprite;
                break;
            case EChromaColor.Green:
                currentImage.sprite = greenSprite;
                break;
            default:
                currentImage.sprite = redSprite;
                break;
        }
    }
}
