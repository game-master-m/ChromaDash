using UnityEngine;
using TMPro; // TextMeshPro

/// <summary>
/// 상점 UI를 표시하는 View입니다.
/// 1. PlayerData(재화)를 '구독'하여 소지금을 표시합니다.
/// 2. '구매' 버튼 클릭 시 OnBuyItemRequest 이벤트를 '발행'합니다.
/// </summary>
public class ShopUI : MonoBehaviour
{
    [Header("데이터 참조 (SSOT)")]
    [SerializeField] private PlayerInventoryData playerData; // 소지금을 표시하기 위해

    [Header("발행할 이벤트 채널")]
    [SerializeField] private ItemEventChannelSO onBuyItemRequest; // 구매 요청을 보내기 위해

    [Header("UI 컴포넌트")]
    [SerializeField] private TextMeshProUGUI playerGoldText;

    // (이 외에 상점 아이템 슬롯(ShopItemSlot)들이 있다고 가정)

    void OnEnable()
    {
        // 1. 플레이어 재화 변경 이벤트를 구독
        playerData.OnGoldChange += UpdateGoldUI;

        // 2. 활성화 시 현재 재화로 즉시 갱신
        UpdateGoldUI(playerData.Gold);
    }

    void OnDisable()
    {
        // 3. 비활성화 시 구독 해제
        playerData.OnGoldChange -= UpdateGoldUI;
    }

    /// <summary>
    /// 재화 텍스트를 갱신하는 이벤트 핸들러입니다.
    /// </summary>
    private void UpdateGoldUI(int newGold)
    {
        playerGoldText.text = $"소지금: {newGold} G";
    }

    /// <summary>
    /// (가상) 상점 슬롯의 '구매' 버튼이 이 함수를 호출합니다.
    /// </summary>
    /// <param name="itemToBuy">구매할 아이템</param>
    public void OnBuyButtonClicked(ItemData itemToBuy)
    {
        // "이 아이템을 사고 싶다"고 이벤트 채널에 방송(Raise)합니다.
        // 누가 이 요청을 처리하는지(InventoryManager) ShopUI는 전혀 모릅니다.
        onBuyItemRequest.Raised(itemToBuy);

        // UI 갱신? 할 필요 없습니다.
        // InventoryManager가 요청을 처리하고 playerData.ModifyGold()를 호출하면,
        // OnGoldChanged 이벤트가 발행되어 UpdateGoldUI가 "자동으로" 호출됩니다.
    }
}