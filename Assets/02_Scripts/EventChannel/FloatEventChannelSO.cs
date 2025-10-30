using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFloatEventChannel", menuName = "ChromaDash/Events/Float Event Channel")]
public class FloatEventChannelSO : ScriptableObject
{
    public event Action<float> onEvent;
    public void Raised(float value)
    {
        onEvent?.Invoke(value);
    }
}
