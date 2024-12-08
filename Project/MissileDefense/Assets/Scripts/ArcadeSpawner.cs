using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ArcadeSpawner : MonoBehaviour
{
    public float spawnRadius = 50f;
    public GameObject enemyPrefab;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ArcadeSpawning());
    }

    // Update is called once per frame
    void Update()
    {
    }

    private float R(float center)
    {
        return Random.Range(center * 0.75f, center * 1.25f);
    }

    private int IntRange(int min, int max)
    {
        return Random.Range(min, max+1);
    }

    private IEnumerator ArcadeSpawning()
    {
        yield return new WaitForSeconds(R(5f));
        SpawnEnemy(2f);
        yield return new WaitForSeconds(R(10f));

        // --- Wave 1 ---
        float waveSpeed = 2f;
        int subwaves = IntRange(3, 4);
        for (int i = 0; i < subwaves; i++)
        {
            int enemies = IntRange(1, 3);
            for (int j = 0; j < enemies; j++)
            {
                SpawnEnemy(waveSpeed);
                yield return new WaitForSeconds(R(2f));
            }

            yield return new WaitForSeconds(R(5f));
        }
        yield return new WaitForSeconds(R(10f));


        // --- Wave 2 ---
        waveSpeed = 2.5f;
        subwaves = IntRange(3, 5);
        for (int i = 0; i < subwaves; i++)
        {
            int enemies = IntRange(2, 4);
            for (int j = 0; j < enemies; j++)
            {
                SpawnEnemy(waveSpeed);
                yield return new WaitForSeconds(R(1.5f));
            }

            yield return new WaitForSeconds(R(4f));
        }
        yield return new WaitForSeconds(R(10f));


        // --- Wave 3 ---
        waveSpeed = 3f;
        subwaves = 5;
        for (int i = 0; i < subwaves; i++)
        {
            int enemies = IntRange(2, 5);
            for (int j = 0; j < enemies; j++)
            {
                SpawnEnemy(waveSpeed);
                yield return new WaitForSeconds(R(1f));
            }

            yield return new WaitForSeconds(R(4f));
        }
        yield return new WaitForSeconds(R(10f));


        // Wave 4 
        waveSpeed = 4f;
        while (true)
        {
            int enemies = IntRange(2, 5);
            for (int j = 0; j < enemies; j++)
            {
                SpawnEnemy(waveSpeed);
                yield return new WaitForSeconds(R(1f));
            }

            yield return new WaitForSeconds(R(2f));
        }
    }

    public void SpawnEnemy(float speed)
    {
        Vector2 randomPoint = Random.insideUnitCircle.normalized * spawnRadius;
        Vector3 spawnPosition = (Vector3)randomPoint;
        Quaternion spawnRotation = Quaternion.LookRotation(Vector3.forward, -spawnPosition);
        GameObject newEnemy = Instantiate(enemyPrefab, spawnPosition, spawnRotation);
        newEnemy.GetComponent<Enemy>().speed = speed;
    }
}
