using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MissileLauncher : MonoBehaviour
{
    // Cities
    public Transform[] cities = new Transform[4];
    private int activeCity = 0;

    // Missiles
    public GameObject missilePrefab;

    // Enemies
    public Canvas enemiesList;
    public GameObject enemyUIPrefab;
    private List<Enemy> enemies = new List<Enemy>();
    private List<EnemyDescription> descriptions = new List<EnemyDescription>();

    // Aiming Parameters
    public float aimLineThickness = 2f;
    public float aimLineLength = 150f;
    private LineRenderer aimLine;
    private float aimRotationDegrees = 0;
    private float aimSpeed = 0f;
    private float aimSpeedCoarse = 100f;
    private float aimSpeedFine = 10f;

    public void TrackNewEnemy(Enemy enemy)
    {
        enemies.Add(enemy);
        GameObject newEnemyUI = Instantiate(enemyUIPrefab, enemiesList.transform);
        descriptions.Add(newEnemyUI.GetComponent<EnemyDescription>());
    }

    public void DeregisterEnemy(Enemy enemy)
    {
        int i = enemies.IndexOf(enemy);
        if (i >= 0)
        {
            enemies.RemoveAt(i);
            Destroy(descriptions[i].gameObject);
            descriptions.RemoveAt(i);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Configure aiming line
        Color aimColor = new Color(1f, 0f, 0f, 0.25f); // Red with 25% opacity
        aimLine = gameObject.GetComponent<LineRenderer>();
        aimLine.startColor = aimColor;
        aimLine.endColor = aimColor;
        aimLine.startWidth = aimLineThickness;
        aimLine.endWidth = aimLineThickness;
    }

    // Update is called once per frame
    void Update()
    {
        UpdateMissileDescriptions();
        UpdateAimLine();

        // Fire button
        if (Input.GetKeyDown(KeyCode.Space))
        {
            GameObject missile = Instantiate(missilePrefab, cities[activeCity].position, Quaternion.Euler(0, 0, aimRotationDegrees));
            missile.GetComponent<Missile>().speed = 20f;
        }

        // Fine vs coarse aim
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift))
        {
            aimSpeed = aimSpeedFine;
        } 
        else
        {
            aimSpeed = aimSpeedCoarse;
        }


        

        // Update selected city
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            activeCity = 0;
        } else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            activeCity = 1;
        } else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            activeCity = 2;
        } else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            activeCity = 3;
        }

        // Update aim line
        if (Input.GetKey(KeyCode.RightArrow))
        {
            aimRotationDegrees -= Time.deltaTime * aimSpeed;
        } else if (Input.GetKey(KeyCode.LeftArrow))
        {
            aimRotationDegrees += Time.deltaTime * aimSpeed;
        }
    }

    private void UpdateAimLine()
    {
        Vector3 direction = Quaternion.Euler(0, 0, aimRotationDegrees) * Vector3.right;
        Vector3 startPoint = cities[activeCity].position;
        Vector3 endPoint = startPoint + direction.normalized * aimLineLength;
        aimLine.positionCount = 2;
        aimLine.SetPosition(0, startPoint);
        aimLine.SetPosition(1, endPoint);
    }

    private void UpdateMissileDescriptions()
    {
        for (int i = 0; i < enemies.Count; i++)
        {
            Enemy enemy = enemies[i];
            int distance = (int) Mathf.Abs(enemy.transform.position.magnitude);
            descriptions[i].UpdateDescription(enemy.ID, distance, (int)enemy.speed, enemy.radarCrossSection, enemy.emissivity);
        }
    }

}
