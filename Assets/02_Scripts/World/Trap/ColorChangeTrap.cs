using UnityEngine;

public class ColorChangeTrap : MonoBehaviour
{
    //컴포넌트 참조
    [SerializeField] private EColorChangeTrap eColorChangeTrap;
    [SerializeField] private SpriteRenderer doorSpriteRenderer;
    [SerializeField] private SpriteRenderer arrowSpriteRenderer;
    [SerializeField] private BoxCollider2D boxCollider2D;

    [Header("이벤트 발행")]
    [SerializeField] private VoidEventChannelSO onColorForcedChangeRight;        //PlayerController 구독
    [SerializeField] private VoidEventChannelSO onColorForcedChangeLeft;        //PlayerController 구독

    private void Awake()
    {
        if (arrowSpriteRenderer == null)
        {
            arrowSpriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (doorSpriteRenderer == null)
        {
            doorSpriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (boxCollider2D == null)
        {
            boxCollider2D = GetComponent<BoxCollider2D>();
        }
    }

    private void OnEnable()
    {
        arrowSpriteRenderer.enabled = true;
        doorSpriteRenderer.enabled = true;
        boxCollider2D.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        //if (collision.gameObject.layer != LayerMask.NameToLayer("Player"))
        //{
        //    return;
        //}
        //Physis2D 에서 Player 레이어와만 충돌하도록 설정

        switch (eColorChangeTrap)
        {
            case EColorChangeTrap.Right:
                onColorForcedChangeRight?.Raised();
                break;
            case EColorChangeTrap.Left:
                onColorForcedChangeLeft?.Raised();
                break;
            default:
                break;
        }

        doorSpriteRenderer.enabled = false;
        arrowSpriteRenderer.enabled = false;
        boxCollider2D.enabled = false;
    }
}
