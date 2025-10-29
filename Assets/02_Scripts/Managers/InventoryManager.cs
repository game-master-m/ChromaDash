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

    [Header("발행할 채널")]
    [SerializeField] private FloatEventChannelSO onHealPotionRequest;
    private void OnEnable()
    {
        onBuyItemRequest.OnEvent += HandleBuyItemRequest;
        onSellItemRequest.OnEvent += HandleSellItemRequest;
        onEquipToQuickSlotRequest.OnEvent += HandleEquipToQuickSlotRequest;
        onUseQuickSlotRequest.OnEvent += HandleUseQuickSlotRequest;
    }
    private void OnDisable()
    {
        onBuyItemRequest.OnEvent -= HandleBuyItemRequest;
        onSellItemRequest.OnEvent -= HandleSellItemRequest;
        onEquipToQuickSlotRequest.OnEvent -= HandleEquipToQuickSlotRequest;
        onUseQuickSlotRequest.OnEvent -= HandleUseQuickSlotRequest;
    }

    //이벤트 핸들러
    private void HandleBuyItemRequest(ItemData item)
    {
        if (playerData.Gold < item.buyPrice)
        {
            //구매 실패
            return;
        }

        playerData.ModifyGold(-item.buyPrice);
        playerData.AddItem(item);
    }
    private void HandleSellItemRequest(ItemData item)
    {
        if (!playerData.MainInventory.Contains(item))
        {
            //인벤에 없음.
            return;
        }
        if (playerData.RemoveItem(item))
        {
            playerData.ModifyGold(item.sellPrice);
        }
    }
    private void HandleEquipToQuickSlotRequest(ItemData item, int index)
    {
        if (item.eItemType == EItemType.None)
        {
            //등록 못 하는 아이템
            return;
        }
        playerData.AssignToQuickSlot(item, index);
    }
    private void HandleUseQuickSlotRequest(int index)
    {
        ItemData item = playerData.QuickSlots[index];
        if (item == null) return;
        //사용을 하고~
        switch (item.eItemType)
        {
            case EItemType.Heal:
                onHealPotionRequest.Rasied(item.itemPower);
                item.itemCount--;
                break;
            case EItemType.SlowHeal:
            //지속회복, 컬러변경 X
            case EItemType.Shield:
            //5초간 무적(크로마대쉬는 가능)
            case EItemType.Rewind:
            default:
                break;
        }
        if (item.itemCount <= 0)
        {
            playerData.AssignToQuickSlot(null, index);
        }
        else
        {
            playerData.AssignToQuickSlot(item, index);
        }

    }
}