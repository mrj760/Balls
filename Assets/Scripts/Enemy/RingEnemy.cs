using System.Collections;
using UnityEngine;

public class RingEnemy : MonoBehaviour
{
    private Enemy enemyComponent;
    
    [SerializeField] private GameObject ring;
    [SerializeField] private float jumpForce;
    
    private Rigidbody rb;
    private bool doJump, hasJumped, doSmash, doGrow;
    [SerializeField] private float ringGrowRate;
    [SerializeField] private float ringMaxGrowth = 7f;
    private Vector3 ogScale;
    
    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        InvokeRepeating(nameof(Jump), 1f, 4f);
        ogScale = ring.transform.localScale;
        enemyComponent = gameObject.GetComponent<Enemy>();
    }

    // Update is called once per frame
    void Update()
    {
        if (hasJumped && rb.velocity.y < -.05f)
        {
            Smash();
        }
    }

    

    private void OnCollisionEnter(Collision other)
    {
        // if queued to smash attack and collide with the ground, then send out the ring
        if (other.gameObject.CompareTag("Ground") && doSmash)
        {
            doSmash = false;
            GameObject ringGo = Instantiate(ring, transform.position, ring.transform.rotation);
            ringGo.GetComponent<Ring>().Grow(ringMaxGrowth, ringGrowRate, gameObject);
        }
    }

    private void Jump()
    {
        // allow jump if within bounds
        var pos = transform.position;
        if (pos.y < 0f || pos.y > .5f) return;
        if (Mathf.Abs(pos.x) > 13f || Mathf.Abs(pos.z) > 13f) return;
        rb.AddForce(GameManager.focalPoint.transform.up * jumpForce, ForceMode.Impulse);
        hasJumped = true;
    }
    
    private void Smash()
    {
        // allow smash if within bounds
        var pos = transform.position;
        if (Mathf.Abs(pos.x) > 8f || Mathf.Abs(pos.z) > 8f) return;
        hasJumped = false;
        rb.AddForce(-GameManager.focalPoint.transform.up * (jumpForce * 2), ForceMode.Impulse);
        doSmash = true;
    }
}
