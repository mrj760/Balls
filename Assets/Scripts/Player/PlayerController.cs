using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour
{
    // World objects
    private Rigidbody rb;
    [SerializeField] private GameObject powerupIndicator;
    [SerializeField] private Text stuckText;
    
    // Movement / Body values
    [SerializeField] private float speed=10f;
    [SerializeField] private float jumpForce = 5f;
    [SerializeField] private float clinginess = .05f;
    private float ogSpeed, ogMass, ogJumpForce;

    // Powerup values
    [SerializeField] private Vector3 powerupIndicatorOffset = new Vector3(0f, -.5f, 0f);
    private Powerup currentPower;
    private Powerup.PowerType currentPowerType;
    // Bounce powerup
    private bool hasBouncePowerup=false;
    // Missile powerup
    [SerializeField] private GameObject missile;
    
    // Input values
    private float vInput, hInput;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        powerupIndicator.SetActive(false);

        ogSpeed = speed;
        ogMass = rb.mass;
        ogJumpForce = jumpForce;

        InvokeRepeating(nameof(CheckBounds), 0f, .1f);
    }

    // Update is called once per frame
    private void Update()
    {
        GetInput();
        Move();
        UpdatePowerupIndicator();
    }

    private void GetInput()
    {
        // WASD to move
        vInput = Input.GetAxis("Vertical");
        hInput = Input.GetAxis("Horizontal");
        
        // Space to jump
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Jump();
        }

        // R to reset position if stuck (not moving)
        if (Input.GetKeyDown(KeyCode.R) && rb.velocity.magnitude < .02f)
        {
            var pos = GameManager.focalPoint.transform.position;
            pos.y += 3f; 
            transform.position = pos;
        }
    }

    private void Move()
    {
        // base move direction of WASD with focal point as reference
        var forceDir = ((GameManager.focalPoint.transform.forward * vInput) + 
                                (GameManager.focalPoint.transform.right * hInput)).normalized;
        forceDir *= speed * Time.deltaTime;
        rb.AddForce(forceDir);
    }

    private void Jump()
    {
        // allow jump if within bounds
        var pos = transform.position;
        if (pos.y < 0f || pos.y > .15f) return;
        if (Mathf.Abs(pos.x) > 13f || Mathf.Abs(pos.z) > 13f) return;
        rb.AddForce(GameManager.focalPoint.transform.up * jumpForce, ForceMode.Impulse);
    }

    private void CheckBounds()
    {
        // reset game
        if (transform.position.y < -15f)
        {
            SceneManager.LoadScene("Prototype 4");
        }

        // handicap. cling to platform if far enough from center
        if (Mathf.Abs(transform.position.x) > 9f || Mathf.Abs(transform.position.z) > 9f)
        {
            var forceDir = (-transform.position).normalized;
            forceDir.y = 0f;
            rb.AddForce( forceDir * (speed * clinginess * Time.deltaTime));
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("Enemy"))
        {
            if (hasBouncePowerup)
            {
                // Send enemy flying away by applying force to their Rigidbody
                SendFlying(other.gameObject);
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Powerup"))
        {
            // end current, start new
            EndPowerup();
            currentPower = other.GetComponent<Powerup>();
            currentPowerType = currentPower.powerType;
            switch (currentPower.powerType)
            {
                case (Powerup.PowerType.bounce):
                    BouncePowerup();
                    break;
                case (Powerup.PowerType.mass):
                    MassPowerup();
                    break;
                case (Powerup.PowerType.missiles):
                    MissilePowerup();
                    break;
                default:
                    Debug.Log("Invalid pickup");
                    break;
            }

            Destroy(other.gameObject);
        }
    }

    private void EndPowerup()
    {
        // Reset all powerup-related values, stop Coroutines
        currentPowerType = Powerup.PowerType.none;
        currentPower = null;
        
        hasBouncePowerup = false;
        
        rb.mass = ogMass;
        speed = ogSpeed;
        jumpForce = ogJumpForce;
        
        powerupIndicator.SetActive(false);
        
        StopCoroutine(nameof(PowerupCountdown));
        StopCoroutine(nameof(PowerupCountdown));
        CancelInvoke(nameof(MakeMissiles));
    }
    
    private IEnumerator PowerupCountdown()
    {
        // End powerup after it's duration
        yield return new WaitForSeconds(currentPower.powerLength);
        EndPowerup();
    }

    private void BouncePowerup()
    {
        // StopCoroutine(nameof(BouncePowerCR));
        hasBouncePowerup = true;
        powerupIndicator.SetActive(true);
        StartCoroutine(nameof(PowerupCountdown));
    }

    private void SendFlying(GameObject enemy)
    {
        // Normalize away, impulse based on power level and enemy mass
        var away = (enemy.transform.position - transform.position).normalized;
        enemy.GetComponent<Rigidbody>().AddForce(away * (currentPower.powerLevel * enemy.GetComponent<Rigidbody>().mass), ForceMode.Impulse);
    }

    private void MassPowerup()
    {
        // increase mass, increase speed and jump force *almost* accordingly
        rb.mass = currentPower.powerLevel;
        var newMass = rb.mass;
        speed = ogSpeed * (newMass/ogMass) * .9f;
        jumpForce = ogJumpForce * (newMass/ogMass);
        powerupIndicator.SetActive(true);
        StartCoroutine(nameof(PowerupCountdown));
    }

    private void MissilePowerup()
    {
        currentPowerType = Powerup.PowerType.missiles;
        InvokeRepeating(nameof(MakeMissiles), 0f, .33f);
    }

    private int missilesMade = 0;
    private void MakeMissiles()
    {
        for (int i = 0; i < currentPower.powerLevel; i++)
        {
            var pos = transform.position;
            pos.y = .5f;
            Instantiate(missile, pos, missile.transform.rotation);
        }
        if (++missilesMade > currentPower.powerLength)
        {
            missilesMade = 0;
            EndPowerup();   
        }
    }

    private void UpdatePowerupIndicator()
    {
        if (currentPowerType == Powerup.PowerType.none) return;
        powerupIndicator.transform.position = transform.position + powerupIndicatorOffset;
        powerupIndicator.transform.Rotate(Vector3.up * (5f * Time.deltaTime));
    }

    
}
