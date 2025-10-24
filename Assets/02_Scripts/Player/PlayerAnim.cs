using UnityEngine;
public static class AnimHash
{
    public static readonly int runHash = Animator.StringToHash("Run");
    public static readonly int fastRunHash = Animator.StringToHash("FastRun");
    public static readonly int jumpOnGroundHash = Animator.StringToHash("JumpOnGround");
    public static readonly int airIdledHash = Animator.StringToHash("AirIdle");
}
public class PlayerAnim : MonoBehaviour
{
    //component
    private Animator animator;

    //component 참조
    [SerializeField] private SkinnedMeshRenderer skinnedMeshRenderer;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material blueMaterial;
    [SerializeField] private Material greenMaterial;

    //멤버
    private MaterialPropertyBlock propBlock;
    private int colorPropertyID;


    private void Awake()
    {
        animator = GetComponent<Animator>();

        propBlock = new MaterialPropertyBlock();
        colorPropertyID = Shader.PropertyToID("_BaseColor");
        if (skinnedMeshRenderer == null)
        {
            Debug.Log("Renderer 없음!!!!!!!");
        }
    }

    public void PlayAnim(int hash)
    {
        animator.Play(hash);
    }
    public void CrossFade(int hash, float fadeTime)
    {
        animator.CrossFade(hash, fadeTime);
    }
    public void ChangeColor(EChromaColor newColor)
    {
        if (skinnedMeshRenderer == null) return;
        if (propBlock == null) return;

        Color targetColor;

        switch (newColor)
        {
            case EChromaColor.Red:
                targetColor = redMaterial.GetColor(colorPropertyID);
                break;
            case EChromaColor.Blue:
                targetColor = blueMaterial.GetColor(colorPropertyID);
                break;
            case EChromaColor.Green:
                targetColor = greenMaterial.GetColor(colorPropertyID);
                break;
            default:
                targetColor = redMaterial.GetColor(colorPropertyID);
                break;
        }
        skinnedMeshRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor(colorPropertyID, targetColor);
        skinnedMeshRenderer.SetPropertyBlock(propBlock);
    }
}
