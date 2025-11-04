using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinWatcher : MonoBehaviour
{
    [SerializeField] private List<CoinPrefab> coinPrefabs;

    private void OnEnable()
    {
        if (coinPrefabs == null || coinPrefabs.Count == 0)
        {
            return;
        }
        foreach (var coin in coinPrefabs)
        {
            if (coin == null) continue;
            coin.gameObject.SetActive(true);
        }
    }

}
