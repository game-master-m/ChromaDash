using System;
using UnityEngine;

[CreateAssetMenu(fileName = "NewEDifficultyModeEventChannel", menuName = "ChromaDash/Events/EDifficultyMode Event Channel")]
public class EDifficultyModeEventChannelSO : ScriptableObject
{
    public event Action<EDifficultyMode> OnEvent;
    public void Rasied(EDifficultyMode value)
    {
        OnEvent?.Invoke(value);
    }
}
