using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UFO : MonoBehaviour {

    private GameObject target;
    private GameObject targetmesh;

    public float firingSpeed;
    public float moveSpeed = 9;
    public float offset = 9;
    public float health = 100;
    public float fireRate = 2;
    public int projectileDamage;
    public float projectileSpeed;
    public int pointAmount = 10;

    [SerializeField]
    GameObject pointTag;


    public GameObject explosionParticles;
    public GameObject audioSFX;
    public GameObject projectile;
    public GameObject[] gunTips;

    private bool invincibilityFrames = false;
    private MeshRenderer meshRen;

    void Awake()
    {
        target = GameObject.Find("Player");
        targetmesh = target.transform.GetChild(0).gameObject;
        meshRen = gameObject.GetComponentInChildren<MeshRenderer>();
    }

    void Start()
    {
        StartFiring();
    }

    void Update () {
        Move();
        Destroyed();
	}

    void Move()
    {
        if(target != null && targetmesh != null)
        {
            Vector3 newPos = new Vector3(target.transform.position.x, target.transform.position.y + offset, target.transform.position.z);
            Quaternion newRot = new Quaternion(targetmesh.transform.rotation.x, targetmesh.transform.rotation.y, targetmesh.transform.rotation.z, targetmesh.transform.rotation.w);

            transform.position = Vector3.Lerp(transform.position, newPos, Time.deltaTime * moveSpeed);
            transform.rotation = newRot;
        }
    }

    void Destroyed()
    {
        if (health <= 0)
        {
            Boss.instance.InitiateUFOSpawnSequence();
            Instantiate(explosionParticles, transform.position, Quaternion.identity);
            GivePoints();
            AddUfoKillsToPlayer();
            PlaySFX(SFXStates.SubExplosion);
            Destroy(gameObject);
        }
    }

    public void GetDamaged(int damage)
    {
        PlaySFX(SFXStates.Damage);
        if (invincibilityFrames != true)
        {
            StartCoroutine(HitFlash());
            health = health - damage;
        }
    }
    void StartFiring()
    {
        StartCoroutine(Fire());
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
    void AddUfoKillsToPlayer()
    {
        Player.instance.ufoKills++;
    }
    void PlaySFX(SFXStates SFX)
    {
        GameObject clipGOInstance = Instantiate(audioSFX);
        SFXPlayer clipSFXInstance = clipGOInstance.GetComponent<SFXPlayer>();
        clipSFXInstance.PlaySFX(SFX);
    }

    IEnumerator HitFlash()
    {
        invincibilityFrames = true;
        meshRen.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        meshRen.material.color = Color.white;
        yield return new WaitForSeconds(0.1f);
        invincibilityFrames = false;
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
}
