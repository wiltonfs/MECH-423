using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float spawnDelay = 10f;
    public float spawnRadius = 100f;
    public GameObject enemyPrefab;
    public Transform[] cities = new Transform[4];


    private MissileLauncher MissileLauncher;
    private float spawnTimer = 0f;
    private int nextID = 1;
    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = Time.time + spawnDelay;

        MissileLauncher = FindObjectOfType<MissileLauncher>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= spawnTimer)
        {
            SpawnEnemy(1);
            spawnTimer = Time.time + spawnDelay;
        }

    }

    public void SpawnEnemy(int enemyLevel)
    {
        if (enemyLevel == 1)
        {
            Transform randomCity = cities[Random.Range(0, cities.Length)];
            Vector2 randomPoint = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 spawnPosition = randomCity.position + (Vector3)randomPoint;
            Quaternion spawnRotation = Quaternion.LookRotation(Vector3.forward, randomCity.position - spawnPosition);
            GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, spawnRotation);
            // Set the parameters
            newEnemy.GetComponent<Enemy>().ID = nextID;
            newEnemy.GetComponent<Enemy>().speed = 4f;
            newEnemy.GetComponent<Enemy>().emissivity = Random.Range(0.5f, 1f);
            newEnemy.GetComponent<Enemy>().radarCrossSection = Random.Range(.2f, 2f);
            // Add to the list
            MissileLauncher.TrackNewEnemy(newEnemy.GetComponent<Enemy>());
        } 
        else
        {
            throw new System.NotImplementedException();
        }

        nextID++;
    }
}
