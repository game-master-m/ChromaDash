using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    private Rigidbody2D rb;

    void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void AddForce(float forceX, float forceY)
    {
        rb.AddForce(new Vector2(forceX, forceY));
    }
    public void AddForceImpulse(float forceX, float forceY)
    {
        rb.AddForce(new Vector2(forceX, forceY), ForceMode2D.Impulse);
    }
    public void AddForceY(float forceY)
    {
        rb.AddForce(new Vector2(0, forceY));
    }
    public void AddForceImpulseY(float forceY)
    {
        rb.AddForce(new Vector2(0, forceY), ForceMode2D.Impulse);
    }

}
