using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour {

    private new Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        rigidbody = GetComponent<Rigidbody2D>();
        enabled = false;
        rigidbody.isKinematic = true;
        spriteRenderer.enabled = false;
    }

	// Use this for initialization
	void Start () {
        Debug.Log("Start");
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Fire(Vector2 position, Vector2 velocity)
    {
        rigidbody.position = position;
        rigidbody.isKinematic = false;
        spriteRenderer.enabled = true;
        enabled = true;
        rigidbody.velocity = velocity;
    }
}
