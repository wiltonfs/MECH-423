using System.Collections;
using System.Collections.Generic;
using System.Drawing.Text;
using UnityEngine;

public class Enemy : MonoBehaviour
{

    private Rigidbody2D rb;
    
    // Visual parameters
    public float radarSpeed;
    public GameObject visualPrefab;
    private GameObject visualizer;

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

        visualizer = Instantiate(visualPrefab, transform.position, Quaternion.identity);
        visualizer.name = "Visualizer_" + ID;

    }

    // Update is called once per frame
    void Update()
    {
        // If I get close enough to the center, game over!
        if (Vector3.Distance(transform.position, Vector3.zero) < 1f)
        {
            FindObjectOfType<ScoreManager>().GameOver();
        }

        // If the radar intersects me within a threshold, update my visual representation's location to my current location
        float radarAngle = (Time.time * 2f * Mathf.PI * radarSpeed) - 0f * Mathf.PI;
        float intersectionThreshold_degrees = 5f;

        // Calculate angle towards the center
        float angleToCenter = Mathf.Atan2(transform.position.y, transform.position.x);

        // Check if within radar intersection threshold
        if (Mathf.Abs(Mathf.DeltaAngle(Mathf.Rad2Deg * radarAngle, Mathf.Rad2Deg * angleToCenter)) < intersectionThreshold_degrees)
        {
            visualizer.transform.position = transform.position;
            visualizer.GetComponent<EnemyViz>().Pulse();
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.GetComponent<Missile>() != null)
        {
            FindObjectOfType<MissileLauncher>().DeregisterEnemy(this);
            FindObjectOfType<ScoreManager>().DestroyEnemyGetPoints(myPoints);
            // Consume missile
            Destroy(collision.gameObject);
            DestroyEnemy();
        }
    }

    public void DestroyEnemy()
    {
        Destroy(visualizer);
        Destroy(gameObject);
    }
}
