using System;
using System.Collections;
using System.Collections.Generic;
using Random = UnityEngine.Random;
using UnityEngine;
using Unity.VisualScripting;

public class SnowballSpawner : MonoBehaviour
{
    public float spawnRadius;
    public float spawnDelay;
    public GameObject spawnObject;

    private float timer;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > timer)
        {
            timer = Time.time + spawnDelay;
            Spawn();
        }
    }

    private void Spawn()
    {
        float theta = Random.Range(0f, 360f * (float)Math.PI / 180f);
        float r = Random.Range(0f, spawnRadius);

        float x = transform.position.x + Mathf.Cos(theta) * r;
        float z = transform.position.z + Mathf.Sin(theta) * r;
        float y = transform.position.y;

        Vector3 newPos = new Vector3(x, y, z);

        Instantiate(spawnObject, newPos, Quaternion.identity);
    }
}
