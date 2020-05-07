using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerController : NetworkBehaviour
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

    private BulletPool<BulletBehaviour> bulletPool;

    private float lastFired;

    private Vector2 aim;

    private SpawnManager spawnManager;
    private bool mounted = false;
    private bool jumping = false;

    void Awake()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        reticleLocation = transform.Find("Reticle");
        spriteRenderer = GetComponent<SpriteRenderer>();
        feetCollider = rigidbody.GetComponent<CircleCollider2D>();
    }

    // Use this for initialization
    void Start()
    {
        spawnManager = GameObject.Find("Bullets").GetComponent<SpawnManager>();
        wingFlapping = false;
        jumping = false;
    }

    public override void OnStartLocalPlayer()
    {
        Camera.main.GetComponent<SmoothCamera2D>().target = transform;
        spriteRenderer.color = Color.green;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isLocalPlayer)
            return;

        var direction = GetControllerDirection();
        if (direction != Vector2.zero)
        {
            aim = direction;
            reticleLocation.transform.localPosition = aim * 2;
        }

        if (mounted)
        {
            MountedMovement(direction);
        }
        else
        {
            //var touchingGround = feetCollider.IsTouchingLayers(LayerMask.GetMask("Ground"));
            //if (Input.GetKey(KeyCode.Space) && touchingGround && !jumping)
            //{
            //    jumping = true;
            //    rigidbody.AddForce(Vector2.up * 2000f);
            //}

            const float runningSpeed = 750f;
            if (direction.x < 0)
                rigidbody.AddForce(Vector2.left * runningSpeed * Time.deltaTime);
            else if (direction.x > 0)
                rigidbody.AddForce(Vector2.right * runningSpeed * Time.deltaTime);
        }
    }

    private void MountedMovement(Vector2 direction)
    {
        if (Input.GetKey(KeyCode.Space) && !wingFlapping)
        {
            wingFlapping = true;
            wingFlapStarted = Time.time;
        }

        if (Input.GetKey(KeyCode.LeftControl) && Time.time - lastFired > 0.5)
        {
            lastFired = Time.time;
            CmdFire(aim, rigidbody.position + aim);
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
                rigidbody.AddForce(GetFlapDirection() * liftCurve.Evaluate(Time.time - wingFlapStarted) * 1000 * Time.deltaTime);
            }
        }
        else if (touchingGround)
        {
            const float runningSpeed = 750f;
            if (direction.x < 0)
                rigidbody.AddForce(Vector2.left * runningSpeed * Time.deltaTime);
            else if (direction.x > 0)
                rigidbody.AddForce(Vector2.right * runningSpeed * Time.deltaTime);
        }
    }

    [Command]
    void CmdFire(Vector2 aim, Vector2 position)
    {
        // Set up bullet on server
        var bullet = spawnManager.GetFromPool(position);
        bullet.GetComponent<Rigidbody2D>().velocity = aim * 20;

        // spawn bullet on client, custom spawn handler will be called
        NetworkServer.Spawn(bullet, spawnManager.assetId);

        // when the bullet is destroyed on the server it wil automatically be destroyed on clients
        StartCoroutine(Destroy(bullet, 2.0f));
    }

    public IEnumerator Destroy(GameObject go, float timer)
    {
        yield return new WaitForSeconds(timer);
        spawnManager.UnSpawnObject(go);
        NetworkServer.UnSpawn(go);
    }

    private Vector2 GetControllerDirection()
    {
        var direction = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
        if (direction.sqrMagnitude > 0.2)
        {
            return direction.normalized;
        }

        return Vector2.zero;
    }

    private Vector2 GetFlapDirection()
    {
        var inputDirection = GetControllerDirection();

        if (inputDirection.x < 0)
        {
            return inputDirection.y < 0 
                ? Vector2.left
                : (Vector2.left + Vector2.up).normalized;
        }

        if (inputDirection.x > 0)
        {
            return inputDirection.y < 0 
                ? Vector2.right
                : (Vector2.right + Vector2.up).normalized;
        }

        return Vector2.up;
    }
}
