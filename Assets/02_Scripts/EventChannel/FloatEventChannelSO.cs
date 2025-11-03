using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewFloatEventChannel", menuName = "ChromaDash/Events/Float Event Channel")]
public class FloatEventChannelSO : ScriptableObject
{
    public event Action<float> OnEvent;
    public void Raised(float value)
    {
        OnEvent?.Invoke(value);
    }
}
