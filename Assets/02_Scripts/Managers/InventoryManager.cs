using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("수정할 데이터")]
    [SerializeField] private PlayerInventoryData playerData;

    [Header("구독할 채널")]
    [SerializeField] private ItemEventChannelSO onBuyItemRequest;
    [SerializeField] private ItemEventChannelSO onSellItemRequest;
    [SerializeField] private ItemSlotEventChannelSO onEquipToQuickSlotRequest;
    [SerializeField] private IntEventChannelSO onUseQuickSlotRequest;
    //퀵슬롯 장착 해제 추가
    [SerializeField] private IntEventChannelSO onUnEquipQuickSlotRequest;

    [Header("발행할 채널")]
    [SerializeField] private FloatEventChannelSO onHealPotionRequest;
    private void OnEnable()
    {
        onBuyItemRequest.OnEvent += HandleBuyItemRequest;
        onSellItemRequest.OnEvent += HandleSellItemRequest;
        onEquipToQuickSlotRequest.OnEvent += HandleEquipToQuickSlotRequest;
        onUseQuickSlotRequest.OnEvent += HandleUseQuickSlotRequest;
        onUnEquipQuickSlotRequest.OnEvent += HandleUnEquipQuickSlotRequest;
    }
    private void OnDisable()
    {
        onBuyItemRequest.OnEvent -= HandleBuyItemRequest;
        onSellItemRequest.OnEvent -= HandleSellItemRequest;
        onEquipToQuickSlotRequest.OnEvent -= HandleEquipToQuickSlotRequest;
        onUseQuickSlotRequest.OnEvent -= HandleUseQuickSlotRequest;
        onUnEquipQuickSlotRequest.OnEvent -= HandleUnEquipQuickSlotRequest;
    }

    //이벤트 핸들러

    private void HandleBuyItemRequest(ItemData itemFromShop)
    {
        if (playerData.Gold < itemFromShop.buyPrice)
        {
            //구매 실패
            return;
        }

        playerData.ModifyGold(-itemFromShop.buyPrice);

        ItemData newItemInstance = Instantiate(itemFromShop);
        newItemInstance.itemCount = 1;
        newItemInstance.itemName = itemFromShop.name;

        playerData.AddItemToMainInventory(newItemInstance);
    }
    private void HandleSellItemRequest(ItemData itemInstance)
    {
        if (!playerData.MainInventory.Contains(itemInstance))
        {
            //인벤에 없음.
            return;
        }
        if (playerData.RemoveItemFromInventory(itemInstance))
        {
            playerData.ModifyGold(itemInstance.sellPrice * itemInstance.itemCount);
            Destroy(itemInstance);
        }
    }
    private void HandleEquipToQuickSlotRequest(ItemData itemFromInven, int index)
    {
        if (!playerData.MainInventory.Contains(itemFromInven)) return;
        if (itemFromInven.eItemType == EItemType.None)
        {
            //등록 못 하는 아이템
            return;
        }
        int itemQuickSlotMax = itemFromInven.maxQuickSlotStack;
        ItemData quickSlotItem = playerData.QuickSlots[index];

        if (quickSlotItem == null)
        {
            int amountToEquip = Mathf.Min(itemQuickSlotMax, itemFromInven.itemCount);

            ItemData newQuickSlotInstance = Instantiate(itemFromInven);
            newQuickSlotInstance.name = itemFromInven.name;
            newQuickSlotInstance.itemCount = amountToEquip;

            itemFromInven.itemCount -= amountToEquip;
            playerData.AssignToQuickSlot(newQuickSlotInstance, index);
        }
        else if (quickSlotItem.name == itemFromInven.name && quickSlotItem.itemCount < itemQuickSlotMax)
        {
            int remainingSpace = itemQuickSlotMax - quickSlotItem.itemCount;
            int equipAmount = Mathf.Min(remainingSpace, itemFromInven.itemCount);

            quickSlotItem.itemCount += equipAmount;
            itemFromInven.itemCount -= equipAmount;

            playerData.AssignToQuickSlot(quickSlotItem, index);
        }
        else
        {
            return;
        }

        if (itemFromInven.itemCount <= 0)
        {
            playerData.RemoveItemFromInventory(itemFromInven);
            Destroy(itemFromInven);
        }
        else
        {
            playerData.NotifyMainInventoryChange();
        }
    }
    private void HandleUseQuickSlotRequest(int index)
    {
        ItemData itemInstance = playerData.QuickSlots[index];
        if (itemInstance == null) return;
        //사용을 하고~
        switch (itemInstance.eItemType)
        {
            case EItemType.Heal:
                onHealPotionRequest.Raised(itemInstance.itemPower);
                itemInstance.itemCount--;
                break;
            case EItemType.SlowHeal:
            //지속회복, 컬러변경 X
            case EItemType.Shield:
            //5초간 무적(크로마대쉬는 가능)
            case EItemType.Rewind:
            default:
                break;
        }
        if (itemInstance.itemCount <= 0)
        {
            playerData.AssignToQuickSlot(null, index);
            Destroy(itemInstance);
        }
        else
        {
            playerData.AssignToQuickSlot(itemInstance, index);
        }
    }
    private void HandleUnEquipQuickSlotRequest(int index)
    {
        ItemData itemToUnEquipFromQuickSlot = playerData.QuickSlots[index];
        if (itemToUnEquipFromQuickSlot == null) return;

        playerData.AssignToQuickSlot(null, index);

        playerData.AddItemToMainInventory(itemToUnEquipFromQuickSlot);
    }
}