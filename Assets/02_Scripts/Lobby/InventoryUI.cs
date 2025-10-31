using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [Header("������ ����")]
    [SerializeField] private PlayerInventoryData playerData;

    [Header("�̺�Ʈ ����")]
    [SerializeField] private InventorySlotEventChannelSO onEquipToQuickSlotRequest; //�κ��丮 �Ŵ����� ����
    [SerializeField] private InventoryEventChannelSO onSellItemRequest;             //�κ��丮 �Ŵ����� ����
    [SerializeField] private IntEventChannelSO onUnEquipQuickSlotRequest;           //�κ��丮 �Ŵ����� ����

    [Header("UI ������ �� �θ�")]
    [SerializeField] private GameObject inventorySlotPrefab;
    [SerializeField] private Transform mainInventoryContainer;

    [Header("�κ� ������ UI")]
    [SerializeField] private Image[] quickSlotIcons;
    [SerializeField] private TextMeshProUGUI[] quickSlotAmounts;
    [SerializeField] private Button[] EquipButton;
    [SerializeField] private Button[] UnEquipButton;

    [Header("������ ���̶���Ʈ ����")]
    [SerializeField] private Image[] quickSlotHighlight;
    [SerializeField] private float targetAlpha = 0.7f;
    [SerializeField] private float fadeDuration = 0.2f;
    private Coroutine runningFade;

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
    }

    private void OnEnable()
    {
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
        if (playerData != null)
        {
            playerData.OnMainInventoryChange -= RefreshMainInventory;
            playerData.OnQuickSlotChange -= RefreshSingleQuickSlot;
        }
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
                slotPrefab.Init(slotData, OnSelectItem, OnSellItem);
            }
        }
        selectedItemInstance = null;
        selectedSlotPrefab = null;
        RefreshAllQuickSlots();
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
            runningFade = StartCoroutine(FadeHighlight(0, index));
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
                runningFade = StartCoroutine(FadeHighlight(targetAlpha, index));
            }
            else
            {
                EquipButton[index].gameObject.SetActive(false);
                runningFade = StartCoroutine(FadeHighlight(0, index));
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

        RefreshAllQuickSlots();
        Debug.Log($"����Ʈ! {slotData.itemTemplate.itemName}");
        //ȿ��?
    }
    private void OnSellItem(InventorySlotData slotData)
    {
        onSellItemRequest.Raised(slotData);
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

    private IEnumerator FadeHighlight(float targetAlpha, int index)
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
        runningFade = null;
    }
}
