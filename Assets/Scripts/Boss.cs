using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum WhatGotDamaged
{
    ShieldGenerator,
    MainBody,
    Cannon,
}

public class Boss : MonoBehaviour {

    [Header("Requirements")]
    public GameObject body;
    public GameObject shieldGenerator;
    public GameObject turretBase;
    public GameObject turretHead;
    public GameObject ufo;
    public GameObject ufoSpawnPoint;
    public GameObject explosionParticles;
    public GameObject mainHitbox;
    public GameObject[] gunTips;
    public GameObject projectile;
    public GameObject audioSFX;
    [Space]
    public MeshRenderer meshRenMain;
    public MeshRenderer meshRenShield;
    public MeshRenderer meshRenCannon;
    public MeshRenderer meshRenCannonBase;
    [SerializeField]
    GameObject pointTag;
    public int pointAmount = 200;


    [Header("Main Settings")]
    public int bossHealth = 300;
    public int cannonHealth = 200;
    public int shieldHealth = 200;
    [Header("Shield Generator Settings")]
    public int shieldGeneratorRotationSpeed;
    [Header("UFO Settings")]
    public int ufoDelay;
    [Header("Cannon Settings")]
    public int fireRate;
    public int projectileSpeed;
    public int projectileDamage;

    WhatGotDamaged whatGotDamaged;
    bool invincibilityFrames = false;
    bool isShieldGeneratorDestroyed = false;
    bool isTurretDestroyed = false;
    bool mainBodyDestroyed = false;

    UFO ufoInstance;
    public static Boss instance;

    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        IncreaseHealth();
        StartFiring();
        StartCoroutine(SpawnUFO());
    }

    void Update()
    {
        HealthChecker();
    }

    void IncreaseHealth()
    {
        bossHealth *= WaveSpawner.instance.currentWave;
        cannonHealth *= WaveSpawner.instance.currentWave;
        shieldHealth *= WaveSpawner.instance.currentWave;
    }

    void FixedUpdate () {
        TurnShieldGenerator();
        MainHitboxEnabler();
        TurnTurretHead();
        TurnTurretBase();
    }

    void TurnTurretBase()
    {
        if (isTurretDestroyed != true)
        {
            if (Player.instance != null)
            {
                Transform playerPos = Player.instance.transform;
                turretBase.transform.LookAt(new Vector3(playerPos.transform.position.x, playerPos.transform.position.y, turretBase.transform.position.z), turretBase.transform.up);
            }
        }
    }

    void TurnTurretHead()
    {
        if(isTurretDestroyed != true)
        {
            if (Player.instance != null)
            {
                Transform playerPos = Player.instance.transform;
                turretHead.transform.LookAt(playerPos, turretHead.transform.up);
            }
        }
    }

    void TurnShieldGenerator()
    {
        if(isShieldGeneratorDestroyed != true)
        shieldGenerator.transform.Rotate(new Vector3(0, 0, shieldGeneratorRotationSpeed) * Time.deltaTime * shieldGeneratorRotationSpeed);
    }

    void StartFiring()
    {
        if (isTurretDestroyed == false)
        {
            StartCoroutine(Fire());
        }
    }

    IEnumerator Fire()
    {
        PlaySFX(SFXStates.Laser);
        yield return new WaitForSeconds(fireRate);
        for (int i = 0; i < gunTips.Length; i++)
        {
            GameObject projectileInstance = Instantiate(projectile, gunTips[i].transform);
            Projectile projectileScript = projectileInstance.GetComponent<Projectile>();
            projectileScript.ProjectileGo(projectileSpeed, projectileDamage);
        }
        StartFiring();
    }

    public void InitiateUFOSpawnSequence()
    {
        StartCoroutine(SpawnUFO());
    }

    IEnumerator SpawnUFO()
    {
        yield return new WaitForSeconds(ufoDelay);
        GameObject ufoGoInstance = Instantiate(ufo, ufoSpawnPoint.transform);
        ufoInstance = ufoGoInstance.GetComponent<UFO>();
        ufoInstance.health *= WaveSpawner.instance.currentWave / 1.08f;
    }

    void InitiateNextWave()
    {
        WaveSpawner.instance.NextWave();
        PlaySFX(SFXStates.WaveCleared);
    }

    void GivePoints()
    {
        Player.instance.points = Player.instance.points + pointAmount;
        WaveSpawner.instance.pointsGainedDuringWave = WaveSpawner.instance.pointsGainedDuringWave + pointAmount;
        UIManager.instance.pointsToGive = UIManager.instance.pointsToGive + pointAmount;

        GameObject pointTagGO = Instantiate(pointTag, transform.position, Quaternion.identity);
        PointTag pointTagScript = pointTagGO.GetComponent<PointTag>();
        pointTagScript.PointAssigner(pointAmount);
    }

    void PlaySFX(SFXStates SFX)
    {
        GameObject clipGOInstance = Instantiate(audioSFX);
        SFXPlayer clipSFXInstance = clipGOInstance.GetComponent<SFXPlayer>();
        clipSFXInstance.PlaySFX(SFX);
    }

    void HealthChecker()
    {
        if (bossHealth <= 0)
        {
            GetDestroyed();
        }
    }

    //--------------------------------------------------------------Everything below this line is for damage Systems----------------------------------------------------------------------\\
    public void GetDamaged(int damage)
    {
        PlaySFX(SFXStates.Damage);
        whatGotDamaged = WhatGotDamaged.MainBody;
        if (isShieldGeneratorDestroyed != false)
        {
            bossHealth = bossHealth - damage;
            StartCoroutine(HitFlash());
        }
        else
        {
            StartCoroutine(HitFlash());
        }
        Debug.Log("Body damage");
    }

    public void DamageCannon(int damage)
    {
        PlaySFX(SFXStates.Damage);
        whatGotDamaged = WhatGotDamaged.Cannon;
        if (cannonHealth < 0)
        {
            Instantiate(explosionParticles, turretBase.transform.position, turretBase.transform.rotation);
            turretBase.SetActive(false);
            turretHead.SetActive(false);
            PlaySFX(SFXStates.SubExplosion);
            isTurretDestroyed = true;
        }
        else
        {
        if (isShieldGeneratorDestroyed != false)
        {
            cannonHealth = cannonHealth - damage;
        }
            StartCoroutine(HitFlash());
        }
        Debug.Log("turret damage");
    }

    public void DamageShield(int damage)
    {
        PlaySFX(SFXStates.Damage);
        whatGotDamaged = WhatGotDamaged.ShieldGenerator;
        if (shieldHealth <= 0)
        {
            isShieldGeneratorDestroyed = true;
            Instantiate(explosionParticles, shieldGenerator.transform.position, shieldGenerator.transform.rotation);
            PlaySFX(SFXStates.SubExplosion);
            Destroy(shieldGenerator);
        }
        else
        {
            StartCoroutine(HitFlash());
            shieldHealth = shieldHealth - damage;
        }
        Debug.Log("shield damage");
    }

    void MainHitboxEnabler()
    {
        if (isTurretDestroyed == true && isShieldGeneratorDestroyed == true)
        {
            mainHitbox.SetActive(true);
        }
    }

    void GetDestroyed()
    {
        mainBodyDestroyed = true;
        PlaySFX(SFXStates.MainExplosion);
        GivePoints();
        InitiateNextWave();
        Instantiate(explosionParticles, body.transform.position, body.transform.rotation);
        Destroy(gameObject);
    }

    IEnumerator HitFlash()
    {
        if (mainBodyDestroyed == false)
        {
            switch (whatGotDamaged)
            {
                case WhatGotDamaged.ShieldGenerator:
                    if (isShieldGeneratorDestroyed == false)
                    {
                        invincibilityFrames = true;
                        meshRenShield.material.color = Color.red;
                        yield return new WaitForSeconds(0.1f);
                        meshRenShield.material.color = Color.white;
                        yield return new WaitForSeconds(0.1f);
                        invincibilityFrames = false;
                    }
                    break;

                case WhatGotDamaged.MainBody:
                    invincibilityFrames = true;
                    if (isShieldGeneratorDestroyed == true)
                    {
                        meshRenMain.material.color = Color.red;
                        yield return new WaitForSeconds(0.1f);
                        meshRenMain.material.color = Color.white;
                        yield return new WaitForSeconds(0.1f);
                    }
                    else
                    {
                        meshRenMain.material.color = Color.blue;
                        yield return new WaitForSeconds(0.1f);
                        meshRenMain.material.color = Color.white;
                        yield return new WaitForSeconds(0.1f);
                    }
                    invincibilityFrames = false;
                    break;

                case WhatGotDamaged.Cannon:
                    invincibilityFrames = true;
                    if (isShieldGeneratorDestroyed == true)
                    {
                        if (isTurretDestroyed == false)
                        {
                            meshRenCannon.material.color = Color.red;
                            meshRenCannonBase.material.color = Color.red;
                            yield return new WaitForSeconds(0.1f);
                            meshRenCannon.material.color = Color.white;
                            meshRenCannonBase.material.color = Color.white;
                            yield return new WaitForSeconds(0.1f);
                        }
                    }
                    else
                    {
                        meshRenCannon.material.color = Color.blue;
                        meshRenCannonBase.material.color = Color.blue;
                        yield return new WaitForSeconds(0.1f);
                        meshRenCannon.material.color = Color.white;
                        meshRenCannonBase.material.color = Color.white;
                        yield return new WaitForSeconds(0.1f);
                    }
                    invincibilityFrames = false;
                    break;

                default:
                    Debug.LogError("This wasn't Supposed to happen! Error Code: B1");
                    break;
            }
        }
    }
}
