using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcadeSpawner : MonoBehaviour
{
    public float[] spawnDelay;
    public float spawnDelayDecrement = 0.2f;
    public float spawnRadius = 100f;
    public GameObject enemyPrefab;

    private MissileLauncher MissileLauncher;
    private float[] spawnTimer;
    // Start is called before the first frame update
    void Start()
    {
        spawnTimer = new float[spawnDelay.Length];
        for (int i = 0; i < spawnDelay.Length; i++)
        {
            spawnTimer[i] = Time.time + spawnDelay[i];
        }
        

        MissileLauncher = FindObjectOfType<MissileLauncher>();
    }

    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < spawnDelay.Length; i++)
        {
            if (Time.time >= spawnTimer[i])
            {
                SpawnEnemy();
                spawnTimer[i] = Time.time + spawnDelay[i];
                spawnDelay[i] -= spawnDelayDecrement;
                if (spawnDelay[i] < 0.2f)
                {
                    spawnDelay[i] = 0.2f;
                }
            }
        }

    }

    public void SpawnEnemy()
    {
        Vector2 randomPoint = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPosition = (Vector3)randomPoint;
        Quaternion spawnRotation = Quaternion.LookRotation(Vector3.forward, -spawnPosition);
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, spawnRotation);
        newEnemy.GetComponent<Enemy>().speed = 2f;
    }
}
