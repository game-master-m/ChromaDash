using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [Header("������ ������")]
    [SerializeField] private PlayerInventoryData playerData;

    [Header("������ ä��")]
    [SerializeField] private ItemEventChannelSO onBuyItemRequest;
    [SerializeField] private ItemEventChannelSO onSellItemRequest;
    [SerializeField] private ItemSlotEventChannelSO onEquipToQuickSlotRequest;
    [SerializeField] private IntEventChannelSO onUseQuickSlotRequest;

    [Header("������ ä��")]
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

    //�̺�Ʈ �ڵ鷯
    private void HandleBuyItemRequest(ItemData item)
    {
        if (playerData.Gold < item.buyPrice)
        {
            //���� ����
            return;
        }

        playerData.ModifyGold(-item.buyPrice);
        playerData.AddItem(item);
    }
    private void HandleSellItemRequest(ItemData item)
    {
        if (!playerData.MainInventory.Contains(item))
        {
            //�κ��� ����.
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
            //��� �� �ϴ� ������
            return;
        }
        playerData.AssignToQuickSlot(item, index);
    }
    private void HandleUseQuickSlotRequest(int index)
    {
        ItemData item = playerData.QuickSlots[index];
        if (item == null) return;
        //����� �ϰ�~
        switch (item.eItemType)
        {
            case EItemType.Heal:
                onHealPotionRequest.Rasied(item.itemPower);
                item.itemCount--;
                break;
            case EItemType.SlowHeal:
            //����ȸ��, �÷����� X
            case EItemType.Shield:
            //5�ʰ� ����(ũ�θ��뽬�� ����)
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