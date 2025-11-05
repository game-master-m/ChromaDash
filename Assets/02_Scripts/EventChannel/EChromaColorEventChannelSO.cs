using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEChromaColorEventChannel", menuName = "ChromaDash/Events/EChromaColor Event Channel")]
public class EChromaColorEventChannelSO : ScriptableObject
{
    public event Action<EChromaColor> OnEvent;
    public void Rasied(EChromaColor value)
    {
        OnEvent?.Invoke(value);
    }
}
