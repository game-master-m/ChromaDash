using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryItemSlotPrefab : MonoBehaviour
{
    [Header("UI 컴포넌트")]
    [SerializeField] private Image itemIcon;
    [SerializeField] private TextMeshProUGUI itemNameText;
    [SerializeField] private TextMeshProUGUI itemCountText;
    [SerializeField] private Button selectButton;
    [SerializeField] private Button sellButton;

    [Header("하이라이트 관련")]
    [SerializeField] private Image selectHighligtImage;
    [SerializeField] private float targetAlpha = 0.7f;
    [SerializeField] private float fadeDuration = 0.2f;

    private InventorySlotData slotInstance;

    private Action<InventorySlotData, InventoryItemSlotPrefab> onSelectCallBack;
    private Action<InventorySlotData> onSellCallBack;

    private Coroutine runningFade;

    private void Awake()
    {
        if (selectHighligtImage != null)
        {
            Color oriColor = new Color(
                selectHighligtImage.color.r, selectHighligtImage.color.g, selectHighligtImage.color.b, 0f);
            selectHighligtImage.color = oriColor;
        }
    }
    public void Init(InventorySlotData instance, Action<InventorySlotData, InventoryItemSlotPrefab> onSelect, Action<InventorySlotData> onSell)
    {
        slotInstance = instance;
        onSelectCallBack = onSelect;
        onSellCallBack = onSell;

        itemIcon.sprite = slotInstance.itemTemplate.itemIcon;
        itemIcon.color = Color.white;
        itemNameText.text = slotInstance.itemTemplate.itemName;
        itemCountText.text = $"{slotInstance.itemCount}";

        if (selectButton != null)
        {
            selectButton.onClick.RemoveAllListeners();
            selectButton.onClick.AddListener(OnSelect);
        }
        if (sellButton != null)
        {
            sellButton.onClick.RemoveAllListeners();
            sellButton.onClick.AddListener(OnSell);
        }
    }
    public void Select()
    {
        if (runningFade != null) StopCoroutine(runningFade);

        runningFade = StartCoroutine(FadeHighlight(targetAlpha));
    }
    public void DeSelect()
    {
        if (runningFade != null) StopCoroutine(runningFade);

        runningFade = StartCoroutine(FadeHighlight(0.0f));
    }
    private void OnSelect()
    {
        onSelectCallBack?.Invoke(slotInstance, this);
    }
    private void OnSell()
    {
        onSellCallBack?.Invoke(slotInstance);
    }
    private IEnumerator FadeHighlight(float targetAlpha)
    {
        float elapsedTime = 0.0f;
        Color currentColor = selectHighligtImage.color;
        float startAlpha = currentColor.a;

        while (elapsedTime < fadeDuration)
        {
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            selectHighligtImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        selectHighligtImage.color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
        runningFade = null;
    }


}
