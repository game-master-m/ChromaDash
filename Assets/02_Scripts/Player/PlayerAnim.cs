using UnityEngine;
public static class AnimHash
{
    public static readonly int runHash = Animator.StringToHash("Run");
    public static readonly int fastRunHash = Animator.StringToHash("FastRun");
}
public class PlayerAnim : MonoBehaviour
{
    private Animator animator;

    private MaterialPropertyBlock propBlock;
    private SkinnedMeshRenderer rend;
    private void Awake()
    {
        animator = GetComponent<Animator>();
        rend = GetComponentInChildren<SkinnedMeshRenderer>();

        if (rend == null)
        {
            return;
        }
        propBlock = new MaterialPropertyBlock();
    }
    private void Start()
    {
        ChangeColor();
    }
    public void PlayAnim(int hash)
    {
        animator.Play(hash);
    }
    public void ChangeColor()
    {
        rend.GetPropertyBlock(propBlock);
        propBlock.SetColor("_BaseColor", Color.red);
        rend.SetPropertyBlock(propBlock);
    }
}
