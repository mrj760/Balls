using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    private Transform ptx;
    
    // Start is called before the first frame update
    void Start()
    {
        ptx = GameObject.Find("Player").transform;
    }

    // Update is called once per frame
    void LateUpdate()
    {
        transform.LookAt(ptx);
    }
}
