using System;
using UnityEngine;

public class BulletBehaviour : MonoBehaviour, IPoolable {

    private new Rigidbody2D rigidbody;
    private SpriteRenderer spriteRenderer;

    public event EventHandler Recycling;

    void Awake()
    {
    }

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Fire(Vector2 position, Vector2 velocity)
    {
        rigidbody.simulated = true;
        rigidbody.position = position;
        rigidbody.isKinematic = false;
        spriteRenderer.enabled = true;
        enabled = true;
        rigidbody.velocity = velocity;
    }

    private void OnRecycling()
    {
        Recycling?.Invoke(this, new EventArgs());
    }
}