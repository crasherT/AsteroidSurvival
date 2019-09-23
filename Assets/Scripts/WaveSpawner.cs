using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaveSpawner : MonoBehaviour
{
    [Header("Requirements")]

    [Tooltip("The Spawn Area")]
    public Transform spawnArea;
    [Tooltip("The Asteroid Prefabs")]
    public GameObject[] asteroids;
    [Tooltip("The Boss Prefab")]
    public GameObject boss;
    public GameObject bossAudio;
    [Tooltip("The location where the boss is to spawn")]
    public GameObject bossSpawnPoint;

    public GameObject playerSpawn;

    [Header("Settings")]
    public float asteroidTravelSpeed;
    [Tooltip("Spawn Timer (Gets devided by 10 so dont freak out when it isnt the number you put in)")]
    public float timer;
    [Tooltip("Height of the spawn area in correlation to the middle of the screen")]
    public float spawner;
    [Tooltip("Leave this at 15")]
    public float spawnerScaler;
    [SerializeField]
    int requiredPoints = 500;
    public float bossSpawnDelay;

    [Header("Overlap Settings")]
    public Collider[] asteroidColliders;
    public float radius = 2.3f;

    [Header("Current Statistics")]
    private int wave;
    public int currentWave;
    public int asteroidsDestroyed;
    public int pointsGainedDuringWave;
    public int ufosDestroyed;

    [Header("Privates")]
    bool stopAsteroidSpawning;
    bool bossSpawned;

    public static WaveSpawner instance;

    private void Awake()
    {
        instance = this;
    }

    void Start()
    {
        timer = timer / 100;
        InitiateSpawnSequence();
        CameraScale();
    }

    void Update()
    {
        CameraScale();
        PointAmountChecker();
    }

    void CameraScale()
    {
        spawnArea.transform.localScale = new Vector3(Screen.width / spawnerScaler ,transform.localScale.y, transform.localScale.z);
    }

    void IncreaseRequiredPointsCount()
    {
        float integer = requiredPoints;
        integer *= 1.25f;
        requiredPoints = (int)integer;
    }

    //spawner starter
    public void InitiateSpawnSequence()
    {
        if (pointsGainedDuringWave < requiredPoints && stopAsteroidSpawning == false)
        {
            StartCoroutine(SpawnTimer(timer));
        }
    }

    public IEnumerator SpawnTimer(float time)
    {
        yield return new WaitForSeconds(time);

        Vector3 randomArea = GenerateSpawnLocation();
        if (CheckOverlap(randomArea) == false)
        {
            GameObject asteroidInstance = Instantiate(asteroids[Random.Range(0, asteroids.Length)], randomArea, Quaternion.identity);
            Asteroid asteroidScript = asteroidInstance.GetComponent<Asteroid>();
            asteroidScript.travelSpeed = asteroidTravelSpeed;
        }
        InitiateSpawnSequence();
    }

    //Generate location to spawn in gameobject to spawn
    public Vector3 GenerateSpawnLocation()
    {
        Vector3 randomCoordinate = new Vector3(0, 0, 0);

        Vector3 origin = spawnArea.position;
        Vector3 range = spawnArea.localScale / 2.0f;
        Vector3 randomRange = new Vector3(Random.Range(-range.x, range.x), Random.Range(-range.y, range.y), Random.Range(-range.z, range.z));

        randomCoordinate = origin + randomRange;
        return randomCoordinate;
    }

    //checks if asteroids overlap or not
    public bool CheckOverlap(Vector3 randomCoordinate)
    {
        asteroidColliders = Physics.OverlapSphere(randomCoordinate, radius);
        return asteroidColliders.Length > 0;
    }
    //might give errors when the player is destroyed due to the function being dependant on the player being alive

    void PointAmountChecker()
    {
        if (pointsGainedDuringWave > requiredPoints && bossSpawned == false)
        {
            bossSpawned = true;
            stopAsteroidSpawning = true;
            StartCoroutine(SpawnBoss());
        }
    }

    //spawns a boss at the end of the wave
    IEnumerator SpawnBoss()
    {
        yield return new WaitForSeconds(bossSpawnDelay);
        UIManager.instance.Flash(false, 0.1f);
        yield return new WaitForSeconds(0.1f);
        Player.instance.gameObject.transform.position = playerSpawn.transform.position;
        GameObject audioGOInstance = Instantiate(bossAudio);
        SFXPlayer clipInstance = audioGOInstance.GetComponent<SFXPlayer>();
        clipInstance.PlaySFX(SFXStates.teleport);
        Instantiate(boss,bossSpawnPoint.transform.position, Quaternion.identity);
        UIManager.instance.Flash(true, 0.1f);
    }

    public void NextWave()
    {
        stopAsteroidSpawning = false;
        bossSpawned = false;
        IncreaseRequiredPointsCount();
        pointsGainedDuringWave = 0;
        InitiateSpawnSequence();
        currentWave++;
    }  
}
