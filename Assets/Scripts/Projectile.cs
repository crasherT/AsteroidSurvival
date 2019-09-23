using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour {

    [Header("Settings")]
    public float lifetime = 10;
    float f_Speed;
    public int projectileDamage;

    public bool isEnemy = false;

    void Start()
    {
        StartCoroutine(LifetimeCounter());
    }

    void FixedUpdate()
    {
        Move();
    }

    //Receives the rocketspeed from the player
    public void ProjectileGo(float speed, int damage)
    {
        projectileDamage = damage;
        transform.parent = null;
        f_Speed = speed;
    }

    //moves the projectile in a designated fashion
    void Move()
    {
        transform.Translate(Vector3.up * Time.deltaTime * f_Speed);
    }

    //Destroys the projectile after a certain amount of time has passed and it hasnt hit anything
    public IEnumerator LifetimeCounter()
    {
        yield return new WaitForSeconds(lifetime);
        Destroyed();
    }
    //Destroys the game object
    public void Destroyed()
    {
        Destroy(gameObject);
    }
}
