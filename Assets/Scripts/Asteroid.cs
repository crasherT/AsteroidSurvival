using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Asteroid : MonoBehaviour
{
    [Header("Requirements")]
    public Transform model;
    MeshRenderer meshRen;

    [Header("Settings")]
    [Tooltip("Asteroid lifetime. Standard is 20.")]
    public int lifetime = 20;
    [Tooltip("Asteroid lifetime. Standard is 15.")]
    public int damage = 15;

    public int pointAmount = 10;

    [Tooltip("Asteroid healthpool. Standard is 100.")]
    public int health = 100;
    [Tooltip("The speed which the android travels at. Standard is 5.")]
    public float travelSpeed = 5;

    public GameObject explosionsSFX;

    [Header("Privates")]
    int randomNumber;
    Color mainColor = Color.white;
    PowerUp powerUpType;

    [SerializeField]
    GameObject explosionParticles;
    [SerializeField]
    GameObject pointTag;

    void Awake()
    {
        meshRen = gameObject.GetComponentInChildren<MeshRenderer>();
    }

    void Start()
    {
        randomNumber = Random.Range(1, 6);
        StartCoroutine(LifetimeCounter());
        HasPickup();
    }

    void Update()
    {
        RandomRotator(randomNumber);
        Move(travelSpeed);
    }
    //function to receive damage
    public void DoDamage(int damage)
    {
        health = health - damage;
        StartCoroutine(HitFlash());

        if (health <= 0)
        {
            Explode();
        }
    }

    void HasPickup()
    {
        int Randomizer = Random.Range(1, 280);
        Debug.Log(Randomizer);
        switch (Randomizer)
        {
            case 10:
                powerUpType = PowerUp.DoubleDamage;
                mainColor = Color.magenta;
                pointAmount += 20;
                Debug.Log("Double Damage");
                break;
            case 35:
                powerUpType = PowerUp.TripleDamage;
                mainColor = Color.cyan;
                Debug.Log("Triple Damage");
                pointAmount += 30;
                break;
            case 45:
                powerUpType = PowerUp.Invincibility;
                Debug.Log("Invincibility");
                mainColor = Color.yellow;
                pointAmount += 40;
                break;
            default:
                powerUpType = PowerUp.None;
                mainColor = Color.white;
                break;
        }
        meshRen.material.color = mainColor;
    }

    public void Explode()
    {
        Instantiate(explosionParticles, transform.position, Quaternion.identity);
        GivePoints();
        GivePowerUp(powerUpType);
        PlayExplosionSFX(SFXStates.Asteroid);
        AddAsteroidsDestroyedToPlayer();
        Destroy(gameObject);
    }

    void PlayExplosionSFX(SFXStates SFXStates)
    {
        GameObject explosionPrefab = Instantiate(explosionsSFX);
        SFXPlayer explosionPlayer = explosionPrefab.GetComponent<SFXPlayer>();
        explosionPlayer.PlaySFX(SFXStates);
    }

    //gives points to the player and spawns a point tag
    void GivePoints()
    {
        Player.instance.points = Player.instance.points + pointAmount;
        WaveSpawner.instance.pointsGainedDuringWave = WaveSpawner.instance.pointsGainedDuringWave + pointAmount;
        UIManager.instance.pointsToGive = UIManager.instance.pointsToGive + pointAmount;

        GameObject pointTagGO = Instantiate(pointTag, transform.position, Quaternion.identity);
        PointTag pointTagScript = pointTagGO.GetComponent<PointTag>();

        pointTagScript.PointAssigner(pointAmount);
    }

    void GivePowerUp(PowerUp powerUp)
    {
        if (Player.instance.powerUpActive != true)
        {
            if (powerUpType != PowerUp.None)
            {
                Player.instance.powerUp = powerUp;
                PlayExplosionSFX(SFXStates.PickUp);
                Player.instance.powerUpActive = true;
            }
        }
    }

    void AddAsteroidsDestroyedToPlayer()
    {
        Player.instance.asteroidsDestroyed++;
    }

    //gets called when the ship is hit
    public void DamageDealer(Player ship)
    {
        ship.DoDamage(damage);
    }

    //rotates the asteroid randomly to give it a realistic feel.
    void RandomRotator(float randomNumber)
    {
        if (randomNumber <= 3)
        {
            model.Rotate(Vector3.up * Time.deltaTime * Random.Range(50, 100));
            model.Rotate(Vector3.left * Time.deltaTime * Random.Range(50, 100));
        }
        if (randomNumber == 4 || randomNumber >= 4)
        {
            model.Rotate(Vector3.down * Time.deltaTime * Random.Range(10, 100));
            model.Rotate(Vector3.right * Time.deltaTime * Random.Range(10, 100));
        }
    }

    //moves the asteroid downwards
    public void Move(float speed)
    {
        transform.Translate(Vector3.down * Time.deltaTime * speed);
    }

    //destroys the Asteroid after a certain amount of seconds to remove any off screen asteroids.
    public IEnumerator LifetimeCounter()
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(gameObject);
    }

    public IEnumerator HitFlash()
    {
        meshRen.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        meshRen.material.color = mainColor;
    }
}
