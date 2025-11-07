using Unity.VisualScripting;
using UnityEngine;

public class UnderLimitGround : MonoBehaviour
{
    [SerializeField] private Transform playerTrans;
    void LateUpdate()
    {
        transform.position = new Vector3(playerTrans.position.x, transform.position.y, transform.position.z);
    }
}
