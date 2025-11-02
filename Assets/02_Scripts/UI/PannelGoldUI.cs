using System.Collections;
using TMPro;
using UnityEngine;

public class PannelGoldUI : MonoBehaviour
{
    [SerializeField] private PlayerInventoryData playerData;

    [SerializeField] private TextMeshProUGUI goldText;
    [SerializeField] private TextMeshProUGUI changeGoldText;

    [SerializeField] IntEventChannelSO onGetGold;

    private float fadeOutDuration = 0.8f;

    private Coroutine runningFadeOutCo;
    private void Start()
    {
        UpdateGoldUI();
        changeGoldText.color = new Color(changeGoldText.color.r, changeGoldText.color.g, changeGoldText.color.b, 0);
    }
    private void OnEnable()
    {
        if (playerData != null)
        {
            playerData.OnGoldChange += UpdateGoldUI;
        }
        onGetGold.OnEvent += ShowGetGold;
    }
    private void OnDisable()
    {
        if (playerData != null)
        {
            playerData.OnGoldChange -= UpdateGoldUI;
        }
        onGetGold.OnEvent -= ShowGetGold;
    }

    private void UpdateGoldUI()
    {
        goldText.text = $"{playerData.Gold} G";
    }
    private void ShowGetGold(int goldAmount)
    {
        if (runningFadeOutCo != null) StopCoroutine(runningFadeOutCo);
        runningFadeOutCo = StartCoroutine(FadeOutGetGoldCo(goldAmount));
    }

    IEnumerator FadeOutGetGoldCo(int goldAmount)
    {
        changeGoldText.text = $"+ {goldAmount} G";
        float elapsedTime = 0f;
        while (elapsedTime < fadeOutDuration)
        {
            elapsedTime += Time.deltaTime;
            float newAlpha = Mathf.Lerp(1, 0, elapsedTime / fadeOutDuration);
            changeGoldText.color = new Color(changeGoldText.color.r, changeGoldText.color.g, changeGoldText.color.b, newAlpha);
            yield return null;
        }
        changeGoldText.color = new Color(changeGoldText.color.r, changeGoldText.color.g, changeGoldText.color.b, 0);
    }
}
