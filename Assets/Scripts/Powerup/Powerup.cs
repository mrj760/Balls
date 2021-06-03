using UnityEngine;

public class Powerup : MonoBehaviour
{
    public enum PowerType
    {
        none,
        bounce,
        mass,
        missiles
    }

    public PowerType powerType;

    public int powerLevel, powerLength;
    
}
