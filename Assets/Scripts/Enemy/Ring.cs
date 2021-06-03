using UnityEngine;

public class Ring : MonoBehaviour
{
    [SerializeField] private float ringForce;
    public GameObject callingEnemy;
    private bool grow = false;
    private float growRate, goalSize;
    private Vector3 ogScale;

    private void Start()
    {
        ogScale = transform.localScale;
    }
    
    private void Update()
    {
        if (grow)
        {
            Vector3 newScale = transform.localScale;
            newScale += newScale * (growRate * Time.deltaTime);
            newScale.y = ogScale.y;
            if (newScale.x > goalSize)
            {
                transform.localScale = ogScale;
                gameObject.SetActive(false);
                grow = false;
            }

            transform.localScale = newScale;
        }
    }

    // Start is called before the first frame update
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            SendFlying(other.gameObject);
        }
    }
    
    private void SendFlying(GameObject player)
    {
        var away = (player.transform.position - transform.position).normalized;
        player.GetComponent<Rigidbody>().AddForce(away * (ringForce * player.GetComponent<Rigidbody>().mass), ForceMode.Impulse);
    }

    public void Grow(float toSize, float rate, GameObject caller)
    {
        goalSize = toSize;
        grow = true;
        growRate = rate;
        callingEnemy = caller;
    }
}
