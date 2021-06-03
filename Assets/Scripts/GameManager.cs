using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject standardEnemy, toughEnemy, smallQuickEnemy, ringEnemy;
    public GameObject[] powerups;
    [SerializeField] private float spawnRange=9f;

    private enum WaveTextState
    {
        Grow,
        Shrink,
        None
    }
    [Header("Wave Text")]
    [SerializeField] private Text waveText;
    [SerializeField] private Vector3 waveTextMaxScale = new Vector3(3f, 3f, 3f);
    [SerializeField] private float growRate=1f;
    private WaveTextState waveTextState = WaveTextState.None;
    private Vector3 waveTextOGScale;
    [SerializeField] private int waveCount = 0;

    public static GameObject focalPoint;
    [SerializeField] private Material flashy;
    [SerializeField] private float flashRate;

    // Start is called before the first frame update
    private void Start()
    {
        InvokeRepeating(nameof(CheckEnemies), 0f, .75f);
        focalPoint = GameObject.Find("Focal Point");
        
        var sc = waveText.transform.localScale;
        waveTextOGScale = sc;
        waveTextMaxScale *= sc.x;
        Debug.Log(waveTextMaxScale);
    }

    // Update is called once per frame
    private void Update()
    {
        Flash();
        CheckText();
    }

    private void RandSpawn(GameObject obj, int amount)
    {
        for (var i = 0; i < amount; i++)
        {
            var spawnPos = new Vector3(Random.Range(-spawnRange, spawnRange), 0f, Random.Range(-spawnRange, spawnRange));
            if (obj.CompareTag("Enemy"))
            {
                spawnPos.y = Random.Range(0f, 3f);
            }
            else
            {
                spawnPos.y = .3f;
            }
            Instantiate(obj, spawnPos, obj.transform.rotation);
        }
    }

    private void CheckEnemies()
    {
        int enemyCount = FindObjectsOfType<Enemy>().Length;
        if (enemyCount <= 0)
        {
            waveCount++;
            if (waveCount % 10 == 0)
            {
                RandSpawn(smallQuickEnemy, waveCount * 2);
            }
            else if (waveCount % 5 == 0)
            {
                RandSpawn(ringEnemy, 1);
                RandSpawn(standardEnemy, waveCount / 3);
            }
            else if (waveCount % 3 == 0)
            {
                // spawn one more tough enemy every three rounds
                RandSpawn(toughEnemy, waveCount/3);
            }
            else
            {
                RandSpawn(standardEnemy, waveCount);
            }

            // spawn a power up every second round. if one already exists there is a 50 % chance of another being spawned
            if (waveCount % 2 == 0)
            {
                if (GameObject.FindObjectsOfType<Powerup>().Length >= 1)
                {
                    if (Random.value > .5f)
                    {
                        RandSpawn(powerups[Random.Range(0,powerups.Length)], 1);
                    }
                }
                else
                {
                    RandSpawn(powerups[Random.Range(0,powerups.Length)], 1);
                }    
            }

            waveText.text = "Wave: " + waveCount;
            waveTextState = WaveTextState.Grow;
        }
    }

    private void CheckText()
    {
        switch (waveTextState)
        {
            case (WaveTextState.None):
                break;
            case(WaveTextState.Grow):
                TextGrow();
                break;
            case(WaveTextState.Shrink) :
                TextShrink();
                break;
            default:
                // Debug.Log("Invalid Wave Text State");
                break;
        }
    }
    
    private void TextGrow()
    {
        var tx = waveText.transform;
        tx.localScale += tx.localScale * (growRate * Time.deltaTime);
        if (tx.localScale.x > waveTextMaxScale.x)
        {
            waveTextState = WaveTextState.Shrink;
        }
    }

    private void TextShrink()
    {
        var tx = waveText.transform;
        tx.localScale -= tx.localScale * (growRate * Time.deltaTime);
        if (tx.localScale.x < waveTextOGScale.x)
        {
            waveTextState = WaveTextState.None;
        }
    }

    private static Vector3 vec(float f)
    {
        return new Vector3(f, f, f);
    }

    private void Flash()
    {
        var rate = flashRate * Time.deltaTime;
        flashy.mainTextureOffset -= new Vector2(rate,rate);
    }
}
