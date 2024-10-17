using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Abahu : MonoBehaviour
{
    // Start is called before the first frame update
    public ZombieHand zombieHand;

    public int zombieDamage;

    private void Start()
    {
        zombieHand.damage = zombieDamage;
    }
}
