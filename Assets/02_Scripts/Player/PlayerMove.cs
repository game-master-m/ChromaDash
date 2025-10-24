using System;
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
        rb.AddForce(new Vector2(0, forceY), ForceMode2D.Force);
    }
    public void AddForceImpulseY(float forceY)
    {
        rb.AddForce(new Vector2(0, forceY), ForceMode2D.Impulse);
    }
    public void AddForceX(float forceX)
    {
        rb.AddForce(new Vector2(forceX, 0), ForceMode2D.Force);
    }
    public void AddForceImpulseX(float forceX)
    {
        rb.AddForce(new Vector2(forceX, 0), ForceMode2D.Impulse);
    }
    public void AddForceLerpY(float firstForce, float targetForce, float duration, float elapsedTime)
    {
        elapsedTime += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);
        rb.AddForce(new Vector2(0, Mathf.Lerp(firstForce, targetForce, t)));
    }
    public void AddForceLerpYEaseIn(float firstForce, float targetForce, float duration, float elapsedTime)
    {
        elapsedTime += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);
        rb.AddForce(new Vector2(0, Mathf.Lerp(firstForce, targetForce, Mathf.Sin(t * Mathf.PI * 0.5f))));
    }
    public void SetVelocity(float velocityX, float velocityY)
    {
        rb.velocity = new Vector2(velocityX, velocityY);
    }
    public void SetVelocityX(float velocityX)
    {
        rb.velocity = new Vector2(velocityX, rb.velocity.y);
    }
    public void SetVelocityY(float velocityY)
    {
        rb.velocity = new Vector2(rb.velocity.x, velocityY);
    }
    public void VelocityYLerpEaseIn(float OriginalVel, float targetVel, float duration, float elapsedTime)
    {
        elapsedTime += Time.fixedDeltaTime;
        float t = Mathf.Clamp01(elapsedTime / duration);
        rb.velocity = (new Vector2(rb.velocity.x, Mathf.Lerp(OriginalVel, targetVel, Mathf.Sin(t * Mathf.PI * 0.5f))));
    }


    public float GetVelocityY()
    {
        return rb.velocity.y;
    }
}
