using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum EVolumeType { Master, BGM, SFX }

public class VolumeController : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [Header("이 슬라이더가 조절할 대상")]
    [SerializeField] private EVolumeType volumeType;
    [SerializeField] private bool isPlayTest;
    private Slider slider;
    private bool isDragging = false;
    void Start()
    {
        slider = GetComponent<Slider>();

        float savedVolume = 1.0f;
        switch (volumeType)
        {
            case EVolumeType.Master:
                savedVolume = PlayerPrefs.GetFloat("MasterVolume", 1.0f);
                Managers.Sound.SetMasterVolume(savedVolume);
                break;
            case EVolumeType.BGM:
                savedVolume = PlayerPrefs.GetFloat("BgmVolume", 1.0f);
                Managers.Sound.SetBGMVolume(savedVolume);
                break;
            case EVolumeType.SFX:
                savedVolume = PlayerPrefs.GetFloat("SfxVolume", 1.0f);
                Managers.Sound.SetSFXVolume(savedVolume);
                break;
        }

        slider.value = savedVolume;
        slider.onValueChanged.AddListener(OnSliderValueChanged);
    }

    private void OnSliderValueChanged(float value)
    {
        switch (volumeType)
        {
            case EVolumeType.Master:
                Managers.Sound.SetMasterVolume(value);
                break;
            case EVolumeType.BGM:
                Managers.Sound.SetBGMVolume(value);
                break;
            case EVolumeType.SFX:
                Managers.Sound.SetSFXVolume(value);
                break;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (isDragging)
        {
            if (isPlayTest)
            {
                Managers.Sound.PlaySFX(ESfxName.SceneChange);
            }
            isDragging = false;
        }
    }
}