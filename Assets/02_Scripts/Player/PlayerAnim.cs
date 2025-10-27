using UnityEngine;
public static class AnimHash
{
    //clip name
    public static readonly int runHash = Animator.StringToHash("Run");
    public static readonly int fastRunHash = Animator.StringToHash("FastRun");
    public static readonly int jumpOnGroundHash = Animator.StringToHash("JumpOnGround");
    public static readonly int airIdledHash = Animator.StringToHash("AirIdle");


    //Trigger name
    public static readonly int velocityYHash = Animator.StringToHash("VelocityY");
    public static readonly int canChromaDashDistanceHash = Animator.StringToHash("CanChromaDashDistance");
}
public class PlayerAnim : MonoBehaviour
{
    //component
    private Animator animator;

    //component 참조
    [SerializeField] private SkinnedMeshRenderer surfaceSkinnedMeshRenderer;
    [SerializeField] private SkinnedMeshRenderer jointSkinnedMeshRenderer;
    [SerializeField] private Material redMaterial;
    [SerializeField] private Material blueMaterial;
    [SerializeField] private Material greenMaterial;

    [SerializeField] private Material jointPurpleMaterial;
    [SerializeField] private Material jointOrangeMaterial;
    [SerializeField] private Material jointYellowMaterial;

    //멤버
    private MaterialPropertyBlock propBlock;
    private int colorPropertyID;


    private void Awake()
    {
        animator = GetComponent<Animator>();

        propBlock = new MaterialPropertyBlock();
        colorPropertyID = Shader.PropertyToID("_BaseColor");
        if (surfaceSkinnedMeshRenderer == null)
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
        if (surfaceSkinnedMeshRenderer == null) return;
        if (jointSkinnedMeshRenderer == null) return;
        if (propBlock == null) return;

        Color targetColor;
        Color jointColor;
        switch (newColor)
        {
            case EChromaColor.Red:
                targetColor = redMaterial.GetColor(colorPropertyID);
                jointColor = jointYellowMaterial.GetColor(colorPropertyID);
                break;
            case EChromaColor.Blue:
                targetColor = blueMaterial.GetColor(colorPropertyID);
                jointColor = jointOrangeMaterial.GetColor(colorPropertyID);
                break;
            case EChromaColor.Green:
                targetColor = greenMaterial.GetColor(colorPropertyID);
                jointColor = jointPurpleMaterial.GetColor(colorPropertyID);
                break;
            default:
                targetColor = redMaterial.GetColor(colorPropertyID);
                jointColor = jointYellowMaterial.GetColor(colorPropertyID);
                break;
        }
        surfaceSkinnedMeshRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor(colorPropertyID, targetColor);
        surfaceSkinnedMeshRenderer.SetPropertyBlock(propBlock);

        jointSkinnedMeshRenderer.GetPropertyBlock(propBlock);
        propBlock.SetColor(colorPropertyID, jointColor);
        jointSkinnedMeshRenderer.SetPropertyBlock(propBlock);
    }

    public void SetFloat(int hash, float value)
    {
        animator.SetFloat(hash, value);
    }
    public void SetBool(int hash, bool value)
    {
        animator.SetBool(hash, value);
    }
    public void SetTrigger(int hash)
    {
        animator.SetTrigger(hash);
    }
}
