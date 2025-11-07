using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPlayer : MonoBehaviour
{
    private PlayerAnim anim;
    private EChromaColor ECurrentColor = EChromaColor.Red;

    [SerializeField] private Image leftBtn;
    [SerializeField] private Image righttBtn;

    [SerializeField] private Color red;
    [SerializeField] private Color blue;
    [SerializeField] private Color green;
    private void Awake()
    {
        anim = GetComponent<PlayerAnim>();
        red = new Color(red.r, red.g, red.b, 0.8f);
        blue = new Color(blue.r, blue.g, blue.b, 0.8f);
        green = new Color(green.r, green.g, green.b, 0.8f);
        leftBtn.color = green;
        righttBtn.color = blue;
    }

    private void OnEnable()
    {
        anim.PlayAnim(AnimHash.runHash);
    }
    void Update()
    {
        transform.Rotate(Vector3.up * 40f * Time.deltaTime);
        if (Managers.Input.IsColorChangeLeftPressed)
        {
            ChangeColorAsKeyLeft();
        }
        if (Managers.Input.IsColorChangeRightPressed)
        {
            ChangeColorAsKeyRight();
        }
    }

    public void ChangeColorAsKeyLeft()
    {
        switch (ECurrentColor)
        {
            case EChromaColor.Red:
                ECurrentColor = EChromaColor.Green;
                leftBtn.color = blue;
                righttBtn.color = red;
                break;
            case EChromaColor.Blue:
                ECurrentColor = EChromaColor.Red;
                leftBtn.color = green;
                righttBtn.color = blue;
                break;
            case EChromaColor.Green:
                ECurrentColor = EChromaColor.Blue;
                leftBtn.color = red;
                righttBtn.color = green;
                break;
            default:
                ECurrentColor = EChromaColor.Red;
                break;
        }
        anim.ChangeColor(ECurrentColor);

        Managers.Sound.PlaySFX(ESfxName.ColorChange);
    }
    public void ChangeColorAsKeyRight()
    {
        switch (ECurrentColor)
        {
            case EChromaColor.Red:
                ECurrentColor = EChromaColor.Blue;
                righttBtn.color = green;
                leftBtn.color = red;
                break;
            case EChromaColor.Blue:
                ECurrentColor = EChromaColor.Green;
                righttBtn.color = red;
                leftBtn.color = blue;
                break;
            case EChromaColor.Green:
                ECurrentColor = EChromaColor.Red;
                righttBtn.color = blue;
                leftBtn.color = green;
                break;
            default:
                ECurrentColor = EChromaColor.Red;
                break;
        }
        anim.ChangeColor(ECurrentColor);
        Managers.Sound.PlaySFX(ESfxName.ColorChange);
    }
}
