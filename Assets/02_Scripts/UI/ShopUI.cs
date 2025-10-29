using UnityEngine;
using TMPro; // TextMeshPro

/// <summary>
/// ���� UI�� ǥ���ϴ� View�Դϴ�.
/// 1. PlayerData(��ȭ)�� '����'�Ͽ� �������� ǥ���մϴ�.
/// 2. '����' ��ư Ŭ�� �� OnBuyItemRequest �̺�Ʈ�� '����'�մϴ�.
/// </summary>
public class ShopUI : MonoBehaviour
{
    [Header("������ ���� (SSOT)")]
    [SerializeField] private PlayerInventoryData playerData; // �������� ǥ���ϱ� ����

    [Header("������ �̺�Ʈ ä��")]
    [SerializeField] private ItemEventChannelSO onBuyItemRequest; // ���� ��û�� ������ ����

    [Header("UI ������Ʈ")]
    [SerializeField] private TextMeshProUGUI playerGoldText;

    // (�� �ܿ� ���� ������ ����(ShopItemSlot)���� �ִٰ� ����)

    void OnEnable()
    {
        // 1. �÷��̾� ��ȭ ���� �̺�Ʈ�� ����
        playerData.OnGoldChange += UpdateGoldUI;

        // 2. Ȱ��ȭ �� ���� ��ȭ�� ��� ����
        UpdateGoldUI(playerData.Gold);
    }

    void OnDisable()
    {
        // 3. ��Ȱ��ȭ �� ���� ����
        playerData.OnGoldChange -= UpdateGoldUI;
    }

    /// <summary>
    /// ��ȭ �ؽ�Ʈ�� �����ϴ� �̺�Ʈ �ڵ鷯�Դϴ�.
    /// </summary>
    private void UpdateGoldUI(int newGold)
    {
        playerGoldText.text = $"������: {newGold} G";
    }

    /// <summary>
    /// (����) ���� ������ '����' ��ư�� �� �Լ��� ȣ���մϴ�.
    /// </summary>
    /// <param name="itemToBuy">������ ������</param>
    public void OnBuyButtonClicked(ItemData itemToBuy)
    {
        // "�� �������� ��� �ʹ�"�� �̺�Ʈ ä�ο� ���(Raise)�մϴ�.
        // ���� �� ��û�� ó���ϴ���(InventoryManager) ShopUI�� ���� �𸨴ϴ�.
        onBuyItemRequest.Raised(itemToBuy);

        // UI ����? �� �ʿ� �����ϴ�.
        // InventoryManager�� ��û�� ó���ϰ� playerData.ModifyGold()�� ȣ���ϸ�,
        // OnGoldChanged �̺�Ʈ�� ����Ǿ� UpdateGoldUI�� "�ڵ�����" ȣ��˴ϴ�.
    }
}