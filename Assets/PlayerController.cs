﻿using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public AnimationCurve liftCurve;

    // Hiding deprecated property getter to prevent accidental usage
    private new Rigidbody2D rigidbody;

    private CircleCollider2D feetCollider;

    private float wingFlapStarted;

    private bool wingFlapping;

    private float flapDuration = 0.3f;

    private Transform reticleLocation;

    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        reticleLocation = transform.FindChild("Reticle");
        spriteRenderer = GetComponent<SpriteRenderer>();
        feetCollider = rigidbody.GetComponent<CircleCollider2D>();
    }

    // Use this for initialization
    void Start()
    {
        wingFlapping = false;
    }

    // Update is called once per frame
    void Update()
    {
        var direction = GetControllerDirection();
        if (direction != Vector2.zero)
        {
            reticleLocation.transform.localPosition = direction * 2;
        }

        ColorAtSpeed();

        if (Input.GetKey(KeyCode.Space) && !wingFlapping)
        {
            wingFlapping = true;
            wingFlapStarted = Time.time;
        }

        var touchingGround = feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));

        if (wingFlapping)
        {
            if (Time.time - wingFlapStarted > flapDuration)
            {
                wingFlapping = false;
            }
            else
            {
                rigidbody.AddForce(GetFlapDirection()*liftCurve.Evaluate(Time.time - wingFlapStarted)*10);
            }
        }
        else if (touchingGround)
        {
            if (direction.x < 0)
                rigidbody.AddForce(Vector2.left * 10);
            else if (direction.x > 0)
                rigidbody.AddForce(Vector2.right * 10);
        }
    }

    private void ColorAtSpeed()
    {
        if (rigidbody.velocity.sqrMagnitude > 81)
        {
            spriteRenderer.color = Color.red;
        }
        else
        {
            spriteRenderer.color = Color.white;
        }
    }

    Vector2 GetControllerDirection()
    {
        var horiz = Input.GetAxis("Horizontal");
        var vert = Input.GetAxis("Vertical");
        var tmp = new Vector2(horiz, vert);
        if (tmp.sqrMagnitude > 0.2)
        {
            return tmp.normalized;
        }

        return Vector2.zero;
    }

    Vector2 GetFlapDirection()
    {
        var inputDirection = GetControllerDirection();

        if (inputDirection.x < 0)
        {
            if (inputDirection.y < 0)
                return Vector2.left;
            else
                return (Vector2.left + Vector2.up).normalized;
        }

        if (inputDirection.x > 0)
        {

            if (inputDirection.y < 0)
                return Vector2.right;
            else
                return (Vector2.right + Vector2.up).normalized;
        }

        return Vector2.up;
    }
}
