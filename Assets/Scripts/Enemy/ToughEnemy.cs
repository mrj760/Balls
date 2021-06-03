using UnityEngine;

public class ToughEnemy : MonoBehaviour
{
    [SerializeField] private GameObject smallQuickEnemy;
    private Vector3 center;
    // Start is called before the first frame update
    void Start()
    {
        center = GameManager.focalPoint.transform.position;
        InvokeRepeating(nameof(SpawnMinion), 1f, 3f);
    }

    private void SpawnMinion()
    {
        var pos = transform.position;
        var towardCenter = center - pos;
        towardCenter /= 2;
        var spawnPos = pos + towardCenter;
        Instantiate(smallQuickEnemy, pos, smallQuickEnemy.transform.rotation);
    }
}
