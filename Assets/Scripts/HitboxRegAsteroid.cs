using System.Collections;
using System.Collections.Generic;
using UnityEngine;
/// <summary>
/// This script is used to detect the collisions and give the ship variable to the asteroid script
/// </summary>

public class HitboxRegAsteroid : MonoBehaviour {

    public Asteroid asteroid;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Ship"))
        {
            Player player = other.GetComponentInParent<Player>();
            player.DoDamage(asteroid.damage);
        }
    }
}
