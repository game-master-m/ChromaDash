using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class InventoryManager : MonoBehaviour
{
    [Header("수정할 데이터")]
    [SerializeField] private PlayerInventoryData playerData;

    [Header("구독할 채널")]
    [SerializeField] private ItemEventChannelSO onBuyItemRequest;       //ShopUI 가 발행
    [SerializeField] private InventorySlotEventChannelSO onSellItemRequest;
    [SerializeField] private InventorySlotEventChannelSO onEquipToQuickSlotRequest;
    [SerializeField] private IntEventChannelSO onUseQuickSlotRequest;
    //퀵슬롯 장착 해제 추가
    [SerializeField] private IntEventChannelSO onUnEquipQuickSlotRequest;   //InventoryUI 가 발행
    [SerializeField] private IntEventChannelSO onGetGold;           //MapSegment 안 coinPrefab 이 발행

    [Header("발행할 채널")]
    [SerializeField] private FloatEventChannelSO onHealPotionRequest;
    private void OnEnable()
    {
        onBuyItemRequest.OnEvent += HandleBuyItemRequest;
        onSellItemRequest.OnEvent += HandleSellItemRequest;
        onEquipToQuickSlotRequest.OnEvent += HandleEquipToQuickSlotRequest;
        onUseQuickSlotRequest.OnEvent += HandleUseQuickSlotRequest;
        onUnEquipQuickSlotRequest.OnEvent += HandleUnEquipQuickSlotRequest;
        onGetGold.OnEvent += HandleOnGetGold;
    }
    private void OnDisable()
    {
        onBuyItemRequest.OnEvent -= HandleBuyItemRequest;
        onSellItemRequest.OnEvent -= HandleSellItemRequest;
        onEquipToQuickSlotRequest.OnEvent -= HandleEquipToQuickSlotRequest;
        onUseQuickSlotRequest.OnEvent -= HandleUseQuickSlotRequest;
        onUnEquipQuickSlotRequest.OnEvent -= HandleUnEquipQuickSlotRequest;
        onGetGold.OnEvent -= HandleOnGetGold;
    }

    //이벤트 핸들러
    private void HandleOnGetGold(int goldAmount)
    {
        playerData.ModifyGold(goldAmount);
    }
    private void HandleBuyItemRequest(ItemData itemFromShop)
    {
        if (playerData.Gold < itemFromShop.buyPrice)
        {
            //구매 실패
            return;
        }
        Managers.Sound.PlaySFX(ESfxName.SellBtn);
        playerData.ModifyGold(-itemFromShop.buyPrice);

        playerData.AddItemToMainInventory(itemFromShop, 1);
    }
    private void HandleSellItemRequest(InventorySlotData slotToSell, int sellCount)
    {
        if (!playerData.MainInventory.Contains(slotToSell)) return;     //인벤에 없음
        if (sellCount <= 0) return;
        if (sellCount > slotToSell.itemCount) return;

        int sellTotal = slotToSell.itemTemplate.sellPrice * sellCount;
        slotToSell.itemCount -= sellCount;
        playerData.ModifyGold(sellTotal);
        Managers.Sound.PlaySFX(ESfxName.SellBtn);
        if (slotToSell.itemCount <= 0) playerData.RemoveItemFromInventory(slotToSell);
        else playerData.NotifyMainInventoryChange();
    }
    private void HandleEquipToQuickSlotRequest(InventorySlotData slotFromInven, int index)
    {
        if (!playerData.MainInventory.Contains(slotFromInven)) return;
        ItemData itemTemplate = slotFromInven.itemTemplate;

        if (itemTemplate.eItemType == EItemType.None)
        {
            Debug.Log("등록 못 하는 아이템 ");
            //등록 못 하는 아이템
            return;
        }
        int itemQuickSlotMax = itemTemplate.maxQuickSlotStack;
        InventorySlotData quickSlotItem = playerData.QuickSlots[index];

        Managers.Sound.PlaySFX(ESfxName.Equip);

        if (quickSlotItem == null || quickSlotItem.itemTemplate == null)
        {
            int amountToEquip = Mathf.Min(itemQuickSlotMax, slotFromInven.itemCount);

            InventorySlotData newQuickSlot = new InventorySlotData(itemTemplate, amountToEquip);

            slotFromInven.itemCount -= amountToEquip;
            playerData.AssignToQuickSlot(newQuickSlot, index);

        }
        else if (quickSlotItem.itemTemplate == itemTemplate && quickSlotItem.itemCount < itemQuickSlotMax)
        {
            int remainingSpace = itemQuickSlotMax - quickSlotItem.itemCount;
            int equipAmount = Mathf.Min(remainingSpace, slotFromInven.itemCount);

            quickSlotItem.itemCount += equipAmount;
            slotFromInven.itemCount -= equipAmount;

            playerData.AssignToQuickSlot(quickSlotItem, index);
        }
        else
        {
            return;
        }

        if (slotFromInven.itemCount <= 0)
        {
            playerData.RemoveItemFromInventory(slotFromInven);
        }
        else
        {
            playerData.NotifyMainInventoryChange();
        }
    }
    private void HandleUseQuickSlotRequest(int index)
    {
        if (SceneManager.GetActiveScene().name != "PlayScene") return;
        InventorySlotData slotInstance = playerData.QuickSlots[index];

        if (slotInstance == null || slotInstance.itemTemplate == null) return;

        ItemData itemTemplate = slotInstance.itemTemplate;
        //사용을 하고~
        switch (itemTemplate.eItemType)
        {
            case EItemType.Heal:
                onHealPotionRequest.Raised(itemTemplate.itemPower);
                slotInstance.itemCount--;
                break;
            case EItemType.SlowHeal:
            //지속회복, 컬러변경 X
            case EItemType.Shield:
            //5초간 무적(크로마대쉬는 가능)
            case EItemType.Rewind:
            default:
                break;
        }
        if (slotInstance.itemCount <= 0)
        {
            playerData.AssignToQuickSlot(null, index);
        }
        else
        {
            playerData.AssignToQuickSlot(slotInstance, index);
        }
    }
    private void HandleUnEquipQuickSlotRequest(int index)
    {
        InventorySlotData slotToUnEquipFromQuickSlot = playerData.QuickSlots[index];
        if (slotToUnEquipFromQuickSlot == null) return;

        Managers.Sound.PlaySFX(ESfxName.Equip);

        playerData.AssignToQuickSlot(null, index);

        playerData.AddItemToMainInventory(slotToUnEquipFromQuickSlot.itemTemplate, slotToUnEquipFromQuickSlot.itemCount);
    }
}