using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    // Movement and force values
    [SerializeField] private float speed;
    [Header("1N per kg")]
    [SerializeField] private float force;

    private float ogYPos;

    private static int enemyTargetIndex = 0;
    private Transform enemyTarget;
    
    // Start is called before the first frame update
    void Start()
    {
        ogYPos = transform.position.y;
        TrackNewEnemy();
    }
    
    // Update is called once per frame
    void Update()
    {
        // Check if target is Destroyed, if not then move toward it
        if (enemyTarget != null)
        {
            TrackEnemy();
        }
        else
        {
            TrackNewEnemy();
        }
    }

    private void TrackNewEnemy()
    {
        if (Enemy.enemies.Count == 0) return;
        //  each missiles tracks the next enemy in the list of current enemies
        if (enemyTargetIndex >= Enemy.enemies.Count)
        {
            enemyTargetIndex = 0;
        }
        enemyTarget = Enemy.enemies[enemyTargetIndex++];
    }

    private void TrackEnemy()
    {
        // Look and move to
        Transform tx;
        (tx = transform).LookAt(enemyTarget);
        tx.Translate(Vector3.forward * (speed * Time.deltaTime));
        var pos = tx.position;
        pos.y = ogYPos;
        transform.position = pos;
    }

    private void OnCollisionEnter(Collision other)
    {
        // Send enemies away from the missile and destroy self
        if (!other.gameObject.CompareTag("Enemy")) return;
        
        var orb = other.gameObject.GetComponent<Rigidbody>();
        orb.AddForce((orb.position - GameManager.focalPoint.transform.position).normalized * force * orb.mass, ForceMode.Impulse);
        Destroy(gameObject);
    }
}
