using UnityEngine;
using UnityEngine.UI;

public class TimeGaugeUI : MonoBehaviour
{
    [Header("데이터 참조")]
    [SerializeField] private PlayerStatsData playerStatsData;

    [Header("UI 컴포넌트")]
    [SerializeField] private Image currentImage;
    [SerializeField] private Sprite redSprite;
    [SerializeField] private Sprite blueSprite;
    [SerializeField] private Sprite greenSprite;
    [Header("UI 컬러옵션")]
    [SerializeField] private Image leftArrow;   //color red
    [SerializeField] private Image rightArrow;  //color blue
    [SerializeField] private Image middlePoint; //color green

    private Color redColor;
    private Color greenColor;
    private Color blueColor;
    private void Awake()
    {
        if (currentImage == null) currentImage = GetComponent<Image>();
        redColor = leftArrow.color;
        greenColor = middlePoint.color;
        blueColor = rightArrow.color;
    }
    private void OnEnable()
    {
        if (playerStatsData == null) return;
        playerStatsData.onTimeGaugeChange += UpdateGaugeLength;
        playerStatsData.onChromaColorChange += UpdateGaugeColor;

        UpdateGaugeLength(playerStatsData.CurrentGauge, playerStatsData.MaxGauge);
        UpdateGaugeColor(playerStatsData.CurrentChromaColor);
    }
    private void OnDisable()
    {
        if (playerStatsData == null) return;
        playerStatsData.onTimeGaugeChange -= UpdateGaugeLength;
        playerStatsData.onChromaColorChange -= UpdateGaugeColor;
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
                middlePoint.color = redColor;
                leftArrow.color = greenColor;
                rightArrow.color = blueColor;
                break;
            case EChromaColor.Blue:
                currentImage.sprite = blueSprite;
                middlePoint.color = blueColor;
                leftArrow.color = redColor;
                rightArrow.color = greenColor;
                break;
            case EChromaColor.Green:
                currentImage.sprite = greenSprite;
                middlePoint.color = greenColor;
                leftArrow.color = blueColor;
                rightArrow.color = redColor;
                break;
            default:
                currentImage.sprite = redSprite;
                middlePoint.color = redColor;
                leftArrow.color = greenColor;
                rightArrow.color = blueColor;
                break;
        }
    }
}
