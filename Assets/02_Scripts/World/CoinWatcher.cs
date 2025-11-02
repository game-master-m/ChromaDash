using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinWatcher : MonoBehaviour
{
    [SerializeField] private List<CoinPrefab> coinPrefabs;

    private void OnEnable()
    {
        foreach (var coin in coinPrefabs)
        {
            coin.gameObject.SetActive(true);
        }
    }

}
