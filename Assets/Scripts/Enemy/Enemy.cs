using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    private Rigidbody rb;
    private GameObject player;
    [SerializeField] private float speed=10f;
    private float ogSpeed;
    [SerializeField] private float boundsHeight=-25f;
    [SerializeField] private float clinginess=1f;

    
    public static List<Transform> enemies = new List<Transform>();

    // Start is called before the first frame update
    private void Start()
    {
        enemies.Add(transform);
        
        rb = GetComponent<Rigidbody>();
        player = GameObject.Find("Player");
        
        ogSpeed = speed;
    }

    private void OnDestroy()
    {
        enemies.Remove(transform);
    }

    // Update is called once per frame
    internal void Update()
    {
        MoveToward(player.transform.position);
        CheckBounds();
    }

    private void MoveToward(Vector3 pos)
    {
        Vector3 movDir = (pos - transform.position).normalized;
        movDir.y = 0f;
        rb.AddForce(movDir * (speed * Time.deltaTime));
    }

    private void CheckBounds()
    {
        if (Mathf.Abs(transform.position.x) > 9f || Mathf.Abs(transform.position.z) > 9f)
        {
            var forceDir = (-transform.position).normalized;
            forceDir.y = 0f;
            rb.AddForce( forceDir * (speed * clinginess * Time.
                deltaTime));
        }
        
        if (transform.position.y < boundsHeight)
        {
            Destroy(gameObject);
        }
    }

    public void MultiplySpeed(float factor)
    {
        speed = ogSpeed * factor;
    }
    public void ResetSpeed()
    {
        speed = ogSpeed;
    }
}
