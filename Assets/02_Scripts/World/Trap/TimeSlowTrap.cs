using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TimeSlowTrap : MonoBehaviour
{
    [SerializeField] private float slowDuration = 3.5f;
    [SerializeField] private float slowFactor = 0.5f;

    [SerializeField] private SpriteRenderer baseRender;
    [SerializeField] private SpriteRenderer hourglassRender;
    [SerializeField] private SpriteRenderer fastRewindRender;

    [SerializeField] private CircleCollider2D trapCollider;

    [Header("이벤트 발행")]
    [SerializeField] private FloatEventChannelSO onTimeSlowTrapEnter; // PlayerController 가 구독
    [SerializeField] private VoidEventChannelSO onTimeSlowTrapExit;   // PlayerController 가 구독

    private void Awake()
    {
        if (trapCollider == null) trapCollider = GetComponent<CircleCollider2D>();
    }

    private void OnEnable()
    {
        baseRender.enabled = true;
        hourglassRender.enabled = true;
        fastRewindRender.enabled = true;
        trapCollider.enabled = true;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        hourglassRender.enabled = false;
        fastRewindRender.enabled = false;
        Managers.Sound.PlaySFX(ESfxName.TimeSlowTrap);
        onTimeSlowTrapEnter.Raised(slowFactor);
    }
    private void OnTriggerExit2D(Collider2D collision)
    {
        onTimeSlowTrapExit.Raised();
    }


}
