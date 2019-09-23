using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <Summary>
/// This script that controls the players ship
/// </summary>

public class Player : MonoBehaviour
{
    [Header("Requirements")]
    public GameObject ship;
    [Space]
    public GameObject[] gunTips;

    public GameObject[] particleSpots;
    [Space]
    public GameObject rocket;
    public GameObject laser;
    [Space]
    public MeshRenderer meshRen;
    [Header("Mobile Requirements")]
    public Joystick joystick;
    public bool mobileFireButtonPressed;

    [Header("Settings")]

    public int Health = 100;
    public float fireRate = 0.3f;
    private float nextFire;

    public bool pcControls;
    public bool isdestroyed;
    public int projectileSpeed;
    public float tiltingSpeed;
    public float movementSpeed;
    public int baseDamage = 25;
    [SerializeField]
    private int totalDamage;
    public float shipCenteringSpeed = 10;

    [SerializeField]
    GameObject explosionParticles;
    [SerializeField]
    GameObject fireParticles;

    [Header("Upgrades")]
    public PowerUp powerUp = PowerUp.None;
    public bool powerUpActive = false;

    [Header("Statistics")]
    public int points;
    public int asteroidsDestroyed;
    public int ufoKills;

    [Header("Audio")]
    public AudioSource laserSFX;
    public AudioSource deathExplosionSFX;
    public AudioSource damagedSFX;
    public AudioSource powerDown;

    [Header("Privates")]
    bool invincibilityFrames;
    public static Player instance;


    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        DamageSetter();
    }

    void Update()
    {
        //FireRate and Firing
        FireButtonChecker();
        FireButtonMobile();
        PowerUpChecker();
    }

    void FixedUpdate()
    {
        if (isdestroyed != true)
        {
            MoveShip();
            ClampShip();
        }
    }

    void DamageSetter()
    {
        switch (powerUp)
        {
            case PowerUp.DoubleDamage:
                totalDamage = baseDamage * 3;
                Debug.Log("double damage picked Up");
                break;
            case PowerUp.TripleDamage:
                totalDamage = baseDamage * 4;
                break;
            case PowerUp.None:
                totalDamage = baseDamage;
                break;
        }
    }

    void FireButtonChecker()
    {
        if (Input.GetMouseButton(0) && Time.time > nextFire && isdestroyed != true && pcControls == true)
        {
            nextFire = Time.time + fireRate;
            FireCannon();
        }
    }

    public void FireButtonController()
    {     
        switch (mobileFireButtonPressed)
        {
            case true:
                mobileFireButtonPressed = false;
                break;
            case false:
                mobileFireButtonPressed = true;
                break;
        }
    }

    void FireButtonMobile()
    {
        if (Time.time > nextFire && mobileFireButtonPressed == true && isdestroyed == false)
        {
            nextFire = Time.time + fireRate;
            FireCannon();
        }
    }

    void PowerUpChecker()
    {
        if (powerUpActive == true && powerUp != PowerUp.None)
        {
            float duration = 0f;
            switch (powerUp)
            {
                case PowerUp.DoubleDamage:
                    duration = 6f;
                    break;
                case PowerUp.TripleDamage:
                    duration = 4f;
                    break;
                case PowerUp.Invincibility:
                    duration = 3f;
                    break;
            }
            StartCoroutine(CooldownDuration(duration));
            DamageSetter();
        }
    }

    IEnumerator CooldownDuration(float duration)
    {
        yield return new WaitForSeconds(duration);
        powerUpActive = false;
        powerUp = PowerUp.None;
        powerDown.Play();
        DamageSetter();
    }

    void MoveShip()
    {
        float horizontal = Input.GetAxis("Horizontal") * movementSpeed * Time.deltaTime;
        float vertical = Input.GetAxis("Vertical") * movementSpeed * Time.deltaTime;

        float horizontalJoystick = joystick.Horizontal * movementSpeed * Time.deltaTime;
        float verticalJoystick = joystick.Vertical * movementSpeed * Time.deltaTime;

        //reset bank
        if (horizontal == 0 || horizontalJoystick == 0)
        {
            float desiredAngle = Mathf.LerpAngle(ship.transform.eulerAngles.y, 0, Time.deltaTime * shipCenteringSpeed);
            ship.transform.eulerAngles = new Vector3(ship.transform.eulerAngles.x, desiredAngle, ship.transform.eulerAngles.z);
        }

        //Right banking
        if (horizontal > 0 || horizontalJoystick > 0)
        {
            float desiredAngle = Mathf.LerpAngle(ship.transform.eulerAngles.y, -25, Time.deltaTime * shipCenteringSpeed);
            ship.transform.eulerAngles = new Vector3(ship.transform.eulerAngles.x, desiredAngle, ship.transform.eulerAngles.z);
        }

        //Left banking
        if (horizontal < 0 || horizontalJoystick < 0)
        {
            float desiredAngle = Mathf.LerpAngle(ship.transform.eulerAngles.y, 25, Time.deltaTime * shipCenteringSpeed);
            ship.transform.eulerAngles = new Vector3(ship.transform.eulerAngles.x, desiredAngle, ship.transform.eulerAngles.z);
        }

        //Move left right and up
        transform.Translate(0, vertical, 0);
        transform.Translate(horizontal, 0, 0);

        transform.Translate(0, verticalJoystick, 0);
        transform.Translate(horizontalJoystick, 0, 0);
    }
        //Clamps Ship in screen
    void ClampShip()
    {
        Vector3 position = Camera.main.WorldToViewportPoint(transform.position);
        position.x = Mathf.Clamp(position.x, 0.05f, 0.9f);
        position.y = Mathf.Clamp(position.y, 0.05f, 0.9f);
        transform.position = Camera.main.ViewportToWorldPoint(position);
    }


    //gets called when the fire button is pressed
    void FireCannon()
    {
        laserSFX.Play();
        int arrayOrder = 0;
        foreach (GameObject item in gunTips)
        {

            GameObject laserInstance = Instantiate(laser, gunTips[arrayOrder].transform);
            Projectile projectileScript = laserInstance.GetComponent<Projectile>();
            projectileScript.ProjectileGo(projectileSpeed, totalDamage);
            arrayOrder++;
        }
    }

    //Damages the ship
    public void DoDamage(int Damage)
    {
        if (invincibilityFrames != true)
        {
            switch (powerUp)
            {
                case PowerUp.Invincibility:
                    StartCoroutine(HitFlash(Color.cyan));
                    damagedSFX.Play();
                    break;

                case PowerUp.None:
                    Health = Health - Damage;
                    StartCoroutine(HitFlash(Color.red));
                    damagedSFX.Play();
                    int RandomNumber = Random.Range(0, particleSpots.Length);
                    GameObject fireParticle = Instantiate(fireParticles, particleSpots[RandomNumber].transform.position, Quaternion.identity);
                    fireParticle.transform.parent = particleSpots[RandomNumber].transform;
                    if (Health <= 0)
                    {
                        Die();
                    }
                    break;

            }
        }
    }

    //Destroys the ship when health reaches zero
    void Die()
    {
        isdestroyed = true;
        deathExplosionSFX.Play();
        ship.SetActive(false);
        Instantiate(explosionParticles, transform.position, Quaternion.identity);
        GeneralManager.instance.SaveStats();
        GeneralManager.instance.GoBackToMainMenu();
        UIManager.instance.YouDiedFlash();
    }

    //Flashes the ship red when Hit
    IEnumerator HitFlash(Color whatToFlash)
    {
        invincibilityFrames = true;
        meshRen.material.color = whatToFlash;
        yield return new WaitForSeconds(0.1f);        
        meshRen.material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        invincibilityFrames = false;
    }
}