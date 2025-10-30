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
    //������ ���� ���� �߰�
    [SerializeField] private IntEventChannelSO onUnEquipQuickSlotRequest;

    [Header("������ ä��")]
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

    //�̺�Ʈ �ڵ鷯

    private void HandleBuyItemRequest(ItemData itemFromShop)
    {
        if (playerData.Gold < itemFromShop.buyPrice)
        {
            //���� ����
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
            //�κ��� ����.
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
            //��� �� �ϴ� ������
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
        //����� �ϰ�~
        switch (itemInstance.eItemType)
        {
            case EItemType.Heal:
                onHealPotionRequest.Raised(itemInstance.itemPower);
                itemInstance.itemCount--;
                break;
            case EItemType.SlowHeal:
            //����ȸ��, �÷����� X
            case EItemType.Shield:
            //5�ʰ� ����(ũ�θ��뽬�� ����)
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