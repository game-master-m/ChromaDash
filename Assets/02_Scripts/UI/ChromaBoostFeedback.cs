using UnityEngine;
using UnityEngine.VFX;

[RequireComponent(typeof(VisualEffect))]
public class ChromaBoostFeedback : MonoBehaviour
{
    [Header("이벤트 구독")]
    [SerializeField] private FloatEventChannelSO onChromaDashSuccess; // PlayerController에서 발행
    [SerializeField] private EChromaColorEventChannelSO onChromaColorChangeRequest; //PlayerStatsManager에서 발행
    [SerializeField] private VoidEventChannelSO onJumpSuccess; // PlayerController에서 발행(JumpInAreState)

    [Header("각 Color에 맞는 Color 값")]
    [SerializeField] private Color redColor;
    [SerializeField] private Color blueColor;
    [SerializeField] private Color greenColor;

    [Header("VFX Graph 설정")]
    private VisualEffect chromaBoostVFX;

    private static readonly int
        VFXEventChromaBoostID = Shader.PropertyToID("OnChromaBoostSuccess"),
        VFXColorPropertyID = Shader.PropertyToID("PlatformColor"),
        VFXEventJumpEffectID = Shader.PropertyToID("OnJumpSuccess");
    private Color currentColor;

    private void Awake()
    {
        chromaBoostVFX = GetComponent<VisualEffect>();
        currentColor = redColor;
    }

    private void OnEnable()
    {
        // PlayerController의 이벤트 구독
        onChromaDashSuccess.OnEvent += PlayBoostEffect;
        onChromaColorChangeRequest.OnEvent += UpdateColor;
        onJumpSuccess.OnEvent += PlayJumpBoostEffect;
    }

    private void OnDisable()
    {
        // 구독 해제
        onChromaDashSuccess.OnEvent -= PlayBoostEffect;
        onChromaColorChangeRequest.OnEvent -= UpdateColor;
    }

    private void UpdateColor(EChromaColor newColor)
    {
        switch (newColor)
        {
            case EChromaColor.Red:
                currentColor = redColor;
                break;
            case EChromaColor.Blue:
                currentColor = blueColor;
                break;
            case EChromaColor.Green:
                currentColor = greenColor;
                break;
            default:
                currentColor = redColor;
                break;
        }
    }
    private void PlayJumpBoostEffect()
    {
        if (chromaBoostVFX == null) return;

        chromaBoostVFX.SetVector4(VFXColorPropertyID, currentColor);
        chromaBoostVFX.SendEvent(VFXEventJumpEffectID);
    }
    private void PlayBoostEffect(float rewardAmount)
    {
        if (chromaBoostVFX == null) return;

        chromaBoostVFX.SetVector4(VFXColorPropertyID, currentColor);
        chromaBoostVFX.SendEvent(VFXEventChromaBoostID);
    }


}