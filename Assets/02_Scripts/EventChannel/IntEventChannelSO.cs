using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewIntEvent", menuName = "ChromaDash/Events/Int Event Channel")]
public class IntEventChannelSO : ScriptableObject
{
    public event Action<int> OnEvent;

    public void Raised(int value)
    {
        OnEvent?.Invoke(value);
    }
}
