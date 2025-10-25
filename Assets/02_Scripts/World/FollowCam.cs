using UnityEngine;

public class FollowCam : MonoBehaviour
{
    [SerializeField] Transform playerTrans;
    [SerializeField] Vector2 offset = new Vector2(2.5f, 1.0f);
    [SerializeField] float follwSpeed = 5.0f;

    private bool follow = true;
    private void LateUpdate()
    {
        if (!playerTrans) return;
        Vector3 targetPos = transform.position;
        if (follow)
        {
            targetPos = new Vector3(playerTrans.position.x + offset.x, playerTrans.position.y + offset.y, transform.position.z);
            transform.position = Vector3.Lerp(transform.position, targetPos, follwSpeed * Time.deltaTime);
        }
    }
}
