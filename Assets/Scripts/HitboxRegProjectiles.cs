using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitboxRegProjectiles : MonoBehaviour {

    public Projectile projectile;

    //change this up so its more efficient later
    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Asteroid") && projectile.isEnemy != true)
        {
            Asteroid asteroid = other.GetComponentInParent<Asteroid>();
            asteroid.DoDamage(projectile.projectileDamage);
            projectile.Destroyed();
        }

        if (other.CompareTag("UFO") && projectile.isEnemy != true)
        {
            UFO ufo = other.GetComponentInParent<UFO>();
            ufo.GetDamaged(projectile.projectileDamage);
            projectile.Destroyed();
        }

        if (other.CompareTag("BossShield") && projectile.isEnemy != true)
        {
            Boss boss = other.GetComponentInParent<Boss>();
            boss.DamageShield(projectile.projectileDamage);
            projectile.Destroyed();
        }

        if (other.CompareTag("BossTurret") && projectile.isEnemy != true)
        {
            Boss boss = other.GetComponentInParent<Boss>();
            boss.DamageCannon(projectile.projectileDamage);
            projectile.Destroyed();
        }

        if (other.CompareTag("Boss") && projectile.isEnemy != true)
        {
            Boss boss = other.GetComponentInParent<Boss>();
            boss.GetDamaged(projectile.projectileDamage);
            projectile.Destroyed();
        }

        if (other.CompareTag("Ship") && projectile.isEnemy == true)
        {
            Debug.Log("Hit");
            Player player = other.GetComponentInParent<Player>();
            player.DoDamage(projectile.projectileDamage);
            projectile.Destroyed();
        }
    }
}
