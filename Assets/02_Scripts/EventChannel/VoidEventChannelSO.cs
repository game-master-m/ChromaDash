using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewVoidEvent", menuName = "ChromaDash/Events/Void Event Channel")]
public class VoidEventChannelSO : ScriptableObject
{
    public event Action OnEvent;
    public void Raised()
    {
        OnEvent?.Invoke();
    }

}
