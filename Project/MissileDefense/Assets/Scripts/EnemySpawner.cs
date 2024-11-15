using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public float spawnDelay = 10f;
    public float spawnRadius = 100f;
    public GameObject enemyPrefab;
    public Transform[] cities = new Transform[4];

    private float spawnTimer = 0f;
    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = Time.time + spawnDelay;
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time >= spawnTimer)
        {
            Transform randomCity = cities[Random.Range(0, cities.Length)];
            Vector2 randomPoint = Random.insideUnitCircle.normalized * spawnRadius;
            Vector3 spawnPosition = randomCity.position + (Vector3)randomPoint;
            Quaternion spawnRotation = Quaternion.LookRotation(Vector3.forward, randomCity.position - spawnPosition);
            Instantiate(enemyPrefab, spawnPosition, spawnRotation);
            spawnTimer = Time.time + spawnDelay;
        }

    }
}
