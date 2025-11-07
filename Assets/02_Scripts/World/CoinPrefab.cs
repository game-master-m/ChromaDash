using System.Collections;
using UnityEngine;

public class CoinPrefab : MonoBehaviour
{
    [SerializeField] private float checkRadius = 1f;

    [Header("이벤트 발행")]
    [SerializeField] IntEventChannelSO onGetGold;   //InventoryManager 가 구독

    private int coinValue = 5;

    private Transform target;

    private Vector3 originalPosition;
    private Quaternion originalRotation;

    private float moveSpeedMIn = 3.0f;
    private float moveSpeedMax = 10.0f;
    private float durationToMaxSpeed = 0.5f;
    private float rotateDuration = 1.0f;

    private float currentMoveSpeed;

    private float elapsedTime = 0f;

    private Coroutine runningRotateCo;
    private void Awake()
    {
        originalPosition = transform.localPosition;
        originalRotation = transform.localRotation;
    }
    private void OnEnable()
    {
        transform.localPosition = originalPosition;
        transform.localRotation = originalRotation;
        target = null;

        elapsedTime = 0f;

        if (runningRotateCo != null) StopCoroutine(runningRotateCo);
        runningRotateCo = StartCoroutine(RotateCo());
    }
    private void OnDisable()
    {
        if (runningRotateCo != null) StopCoroutine(runningRotateCo);
    }
    void Update()
    {
        if (CheckPlayerOverlapCicle(checkRadius))
        {
            if (target != null)
            {
                elapsedTime += Time.deltaTime;
                Vector3 direction = (target.position - transform.position).normalized;
                currentMoveSpeed = Mathf.Lerp(moveSpeedMIn, moveSpeedMax, elapsedTime / durationToMaxSpeed);
                transform.Translate(direction * currentMoveSpeed * Time.deltaTime, Space.World);
            }
        }
        else elapsedTime = 0.0f;
    }

    private bool CheckPlayerOverlapCicle(float radius)
    {
        Collider2D col = Physics2D.OverlapCircle(transform.position, radius, LayerMask.GetMask("Player"));
        if (col != null)
        {
            target = col.transform;
            return true;
        }
        else
        {
            target = null;
            return false;
        }
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            gameObject.SetActive(false);
            Managers.Sound.PlaySFX(ESfxName.Coin);
            //코인 점수 로직 추가
            onGetGold.Raised(coinValue);
        }
    }

    private IEnumerator RotateCo()
    {
        while (true)
        {
            float elapsedTime = 0.0f;
            float rotateCool = Random.Range(1.0f, 1.2f);
            while (elapsedTime < rotateDuration)
            {
                elapsedTime += Time.deltaTime;
                float angle = 360.0f * Time.deltaTime / rotateDuration;
                transform.Rotate(new Vector3(0, angle, 0));
                yield return null;
            }
            transform.localRotation = originalRotation;
            yield return new WaitForSeconds(rotateCool);
        }
    }
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
    }
}
