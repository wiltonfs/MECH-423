using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private Rigidbody2D rb;

    // Visible Enemy Parameters
    public int ID = 0;
    public float speed = 20f;
    public float radarCrossSection; // square meters
    public float emissivity; // dimensionless

    // Secret Enemy Parameters
    public int myPoints = 10;
    public int level = 1;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        rb.velocity = transform.up * speed;
    }

    // Update is called once per frame
    void Update()
    {
        // If I get close enough to the center, game over!
        if (Vector3.Distance(transform.position, Vector3.zero) < 1f)
        {
            FindObjectOfType<ScoreManager>().GameOver();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Missile>() != null)
        {
            FindObjectOfType<MissileLauncher>().DeregisterEnemy(this);
            FindObjectOfType<ScoreManager>().DestroyEnemyGetPoints(myPoints);
            Destroy(gameObject);
        }
    }
}
