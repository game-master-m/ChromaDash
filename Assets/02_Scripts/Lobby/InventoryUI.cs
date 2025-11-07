using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("데이터 참조")]
    [SerializeField] private PlayerInventoryData playerData;

    [Header("이벤트 발행")]
    [SerializeField] private InventorySlotEventChannelSO onEquipToQuickSlotRequest; //인벤토리 매니저가 구독
    [SerializeField] private InventorySlotEventChannelSO onSellItemRequest;             //인벤토리 매니저가 구독
    [SerializeField] private IntEventChannelSO onUnEquipQuickSlotRequest;           //인벤토리 매니저가 구독

    [Header("이벤트 구독")]
    [SerializeField] private ItemEventChannelSO onBuyItemRequest;       //ShopUI 가 발행

    [Header("UI 프리팹 및 부모")]
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private Transform mainInventoryContainer;

    [Header("로비 퀵슬롯 UI")]
    [SerializeField] private Image[] quickSlotIcons;
    [SerializeField] private TextMeshProUGUI[] quickSlotAmounts;
    [SerializeField] private Button[] EquipButton;
    [SerializeField] private Button[] UnEquipButton;

    [Header("퀵슬롯 하이라이트 관련")]
    [SerializeField] private Image[] quickSlotHighlight;
    [SerializeField] private float targetAlpha = 0.7f;
    [SerializeField] private float fadeDuration = 0.2f;

    [Header("UI 컴포넌트")]
    [SerializeField] private Button sellButton;
    [SerializeField] private Button addCountButton;
    [SerializeField] private Button subCountButton;
    [SerializeField] private Transform rootForSell;
    [SerializeField] private TMP_InputField sellCountInputText;
    [SerializeField] private TextMeshProUGUI totalSellPriceText;
    [SerializeField] private TextMeshProUGUI goldChangeText;

    [Header("골드변화 관련")]
    [SerializeField] private float fadeOutGoldChangeDuration = 0.5f;


    private Coroutine runningFadeQuickSlotCo;
    private Coroutine runningFadeOutGoldChangeCo;

    private InventorySlotData selectedItemInstance = null;
    private InventoryItemSlotPrefab selectedSlotPrefab = null;

    private void Awake()
    {
        for (int i = 0; i < quickSlotHighlight.Length; i++)
        {
            if (quickSlotHighlight[i] != null)
            {
                Color oriColor = new Color(
                    quickSlotHighlight[i].color.r, quickSlotHighlight[i].color.g, quickSlotHighlight[i].color.b, 0f);
                quickSlotHighlight[i].color = oriColor;
            }
        }
        if (sellButton != null)
        {
            sellButton.onClick.RemoveAllListeners();
            sellButton.onClick.AddListener(OnClickSellButton);
        }
        if (addCountButton != null)
        {
            addCountButton.onClick.RemoveAllListeners();
            addCountButton.onClick.AddListener(OnClickedAddCountButton);
        }
        if (subCountButton != null)
        {
            subCountButton.onClick.RemoveAllListeners();
            subCountButton.onClick.AddListener(OnClickedSubCountButton);
        }
        if (sellCountInputText != null)
        {
            sellCountInputText.onEndEdit.RemoveAllListeners();
            sellCountInputText.onValueChanged.RemoveAllListeners();
            sellCountInputText.onEndEdit.AddListener(OnEndEditInputTextField);
            sellCountInputText.onValueChanged.AddListener(OnValueChangedInputTextField);
        }
    }

    private void OnEnable()
    {
        onBuyItemRequest.OnEvent += HandleBuyItemRequest;

        if (playerData != null)
        {
            playerData.OnMainInventoryChange += RefreshMainInventory;
            playerData.OnQuickSlotChange += RefreshSingleQuickSlot;

            RefreshMainInventory();
            RefreshAllQuickSlots();
        }
    }
    private void OnDisable()
    {
        onBuyItemRequest.OnEvent -= HandleBuyItemRequest;

        if (playerData != null)
        {
            playerData.OnMainInventoryChange -= RefreshMainInventory;
            playerData.OnQuickSlotChange -= RefreshSingleQuickSlot;
        }
    }

    private void HandleBuyItemRequest(ItemData itemFromShop)
    {
        if (playerData.Gold < itemFromShop.buyPrice)
        {
            //구매 실패
            return;
        }
        if (runningFadeOutGoldChangeCo != null) StopCoroutine(runningFadeOutGoldChangeCo);
        string price = $"{itemFromShop.buyPrice} G";
        runningFadeOutGoldChangeCo = StartCoroutine(FadeOutGoldChangeText(price, fadeOutGoldChangeDuration, '-'));
    }

    private void RefreshMainInventory()
    {
        foreach (Transform child in mainInventoryContainer)
        {
            Destroy(child.gameObject);
        }
        foreach (InventorySlotData slotData in playerData.MainInventory)
        {
            GameObject slotGo = Instantiate(inventorySlotPrefab, mainInventoryContainer);
            InventoryItemSlotPrefab slotPrefab = slotGo.GetComponent<InventoryItemSlotPrefab>();
            if (slotPrefab != null)
            {
                slotPrefab.Init(slotData, OnSelectItem);
            }
        }
        selectedItemInstance = null;
        selectedSlotPrefab = null;
        RefreshAllQuickSlots();
        RefreshMisc(false);
    }
    private void RefreshMisc(bool isShow)
    {
        rootForSell.gameObject.SetActive(isShow);
        if (selectedItemInstance != null && isShow)
        {
            sellCountInputText.text = $"{selectedItemInstance.itemCount}";
            if (int.TryParse(sellCountInputText.text, out int sellCount))
            {
                totalSellPriceText.text = $"{sellCount * selectedItemInstance.itemTemplate.sellPrice} G";
                goldChangeText.color = new Color(goldChangeText.color.r, goldChangeText.color.g, goldChangeText.color.b, 0);
            }
        }
    }
    private void OnEndEditInputTextField(string inputText)
    {
        if (selectedItemInstance == null) return;

        int maxCount = selectedItemInstance.itemCount;
        if (!int.TryParse(inputText, out int sellCount))
        {
            sellCountInputText.text = maxCount.ToString();
            return;
        }

        if (sellCount > maxCount)
        {
            sellCountInputText.text = maxCount.ToString();
        }
        else if (sellCount < 1)
        {
            sellCountInputText.text = "1";
        }
    }
    private void OnValueChangedInputTextField(string inputText)
    {
        if (selectedItemInstance == null) return;
        int maxCount = selectedItemInstance.itemCount;
        if (!int.TryParse(inputText, out int sellCount))
        {
            sellCountInputText.text = maxCount.ToString();
            sellCount = maxCount;
            return;
        }

        if (sellCount > maxCount)
        {
            sellCountInputText.text = maxCount.ToString();
            sellCount = maxCount;
        }
        else if (sellCount < 1)
        {
            sellCountInputText.text = "1";
            sellCount = 1;
        }
        totalSellPriceText.text = $"{sellCount * selectedItemInstance.itemTemplate.sellPrice} G";
    }
    private void OnClickedAddCountButton()
    {
        if (!int.TryParse(sellCountInputText.text, out int sellCount)) return;
        Managers.Sound.PlaySFX(ESfxName.ItemClick);
        if (sellCount < selectedItemInstance.itemCount) sellCount++;
        sellCountInputText.text = sellCount.ToString();

    }
    private void OnClickedSubCountButton()
    {
        if (int.TryParse(sellCountInputText.text, out int sellCount))
        {
            if (sellCount > 1) sellCount--;
        }
        Managers.Sound.PlaySFX(ESfxName.ItemClick);
        sellCountInputText.text = sellCount.ToString();
    }
    private void RefreshSingleQuickSlot(InventorySlotData slotData, int index)
    {
        if (index < 0 || index >= quickSlotIcons.Length) return;
        Image icon = quickSlotIcons[index];
        TextMeshProUGUI slotAmount = quickSlotAmounts[index];
        if (slotData != null && slotData.itemTemplate != null)
        {
            icon.sprite = slotData.itemTemplate.itemIcon;
            icon.color = Color.white;
            slotAmount.text = $"{slotData.itemCount}";
            UnEquipButton[index].gameObject.SetActive(true);
            EquipButton[index].gameObject.SetActive(false);
            runningFadeQuickSlotCo = StartCoroutine(FadeHighlightQuickSlotCo(0, index));
        }
        else
        {
            icon.sprite = null;
            icon.color = new Color(1, 1, 1, 0.0f);
            slotAmount.text = "";
            UnEquipButton[index].gameObject.SetActive(false);
            if (selectedItemInstance != null)
            {
                EquipButton[index].gameObject.SetActive(true);
                runningFadeQuickSlotCo = StartCoroutine(FadeHighlightQuickSlotCo(targetAlpha, index));
            }
            else
            {
                EquipButton[index].gameObject.SetActive(false);
                runningFadeQuickSlotCo = StartCoroutine(FadeHighlightQuickSlotCo(0, index));
            }
        }
    }
    private void RefreshAllQuickSlots()
    {
        for (int i = 0; i < quickSlotIcons.Length; i++)
        {
            InventorySlotData slot = playerData.QuickSlots[i];
            RefreshSingleQuickSlot(slot, i);
        }
    }
    private void OnSelectItem(InventorySlotData slotData, InventoryItemSlotPrefab clickedPrefab)
    {
        if (selectedSlotPrefab != null && selectedSlotPrefab != clickedPrefab)
        {
            selectedSlotPrefab.DeSelect();
        }

        clickedPrefab.Select();

        selectedItemInstance = slotData;
        selectedSlotPrefab = clickedPrefab;


        RefreshMisc(true);
        RefreshAllQuickSlots();
        //효과?
    }
    private void OnSellItem(InventorySlotData slotData, int sellCount)
    {
        onSellItemRequest.Raised(slotData, sellCount);
    }
    private void OnClickSellButton()
    {
        if (!int.TryParse(sellCountInputText.text, out int sellCount)) return;
        OnSellItem(selectedItemInstance, sellCount);
        if (runningFadeOutGoldChangeCo != null) StopCoroutine(runningFadeOutGoldChangeCo);
        runningFadeOutGoldChangeCo = StartCoroutine(FadeOutGoldChangeText(totalSellPriceText.text, fadeOutGoldChangeDuration, '+'));
    }
    public void OnClickEquiptToQuickSlot(int index)
    {
        if (selectedItemInstance == null) return;

        onEquipToQuickSlotRequest.Raised(selectedItemInstance, index);

        if (selectedSlotPrefab != null)
        {
            selectedSlotPrefab.DeSelect();
        }
        selectedItemInstance = null;
        selectedSlotPrefab = null;
    }
    public void OnClickUnEquipQuickSlot(int index)
    {
        if (playerData.QuickSlots[index] == null) return;
        onUnEquipQuickSlotRequest.Raised(index);
    }

    private IEnumerator FadeHighlightQuickSlotCo(float targetAlpha, int index)
    {
        float elapsedTime = 0.0f;
        Color currentColor = quickSlotHighlight[index].color;
        float startAlpha = currentColor.a;
        while (elapsedTime < fadeDuration)
        {
            float t = Mathf.Clamp01(elapsedTime / fadeDuration);
            float newAlpha = Mathf.Lerp(startAlpha, targetAlpha, t);
            quickSlotHighlight[index].color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        quickSlotHighlight[index].color = new Color(currentColor.r, currentColor.g, currentColor.b, targetAlpha);
        runningFadeQuickSlotCo = null;
    }
    private IEnumerator FadeOutGoldChangeText(string price, float duration, char sign)
    {
        float elapsedTime = 0.0f;
        Color currentColor = goldChangeText.color;

        goldChangeText.text = $"{sign} {price}";

        while (elapsedTime < duration)
        {
            float t = Mathf.Clamp01(elapsedTime / duration);
            float newAlpha = Mathf.Lerp(1, 0, t);
            goldChangeText.color = new Color(currentColor.r, currentColor.g, currentColor.b, newAlpha);

            elapsedTime += Time.deltaTime;
            yield return null;
        }
        goldChangeText.color = new Color(currentColor.r, currentColor.g, currentColor.b, 0);
        runningFadeOutGoldChangeCo = null;
    }
}
