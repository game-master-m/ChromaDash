using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEChromaColorEventChannel", menuName = "ChromaDash/Events/EChromaColor Event Channel")]
public class EChromaColorEventChannelSO : ScriptableObject
{
    public event Action<EChromaColor> onEvent;
    public void Rasied(EChromaColor value)
    {
        onEvent?.Invoke(value);
    }
}
