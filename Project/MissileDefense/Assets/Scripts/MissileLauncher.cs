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

    // Aiming Parameters
    public float reloadTime = 0.5f;
    public float aimLineThickness = 2f;
    public float aimLineLength = 150f;
    private LineRenderer aimLine;
    private float aimRotationDegrees = 0;
    private float aimSpeed = 0f;
    private float aimSpeedCoarse = 100f;
    private float aimSpeedFine = 10f;

    private SerialScanner SerialScanner;
    public float EncoderMultiplier;

    private float reloadTimer;

    public void LaunchMissile()
    {
        if (reloadTimer < 0)
        {
            GameObject missile = Instantiate(missilePrefab, cities[activeCity].position, Quaternion.Euler(0, 0, aimRotationDegrees));
            missile.GetComponent<Missile>().speed = 20f;
            reloadTimer = reloadTime;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        // Configure aiming line
        float opacity = 1f;
        Color aimColor = new Color(1f, 1f, 1f, opacity);
        aimLine = gameObject.GetComponent<LineRenderer>();
        aimLine.startColor = aimColor;
        aimLine.endColor = aimColor;
        aimLine.startWidth = aimLineThickness;
        aimLine.endWidth = aimLineThickness;

        // Grab the serial scanner
        SerialScanner = FindObjectOfType<SerialScanner>();
        if (SerialScanner == null)
        {
            SerialScanner = gameObject.AddComponent<SerialScanner>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        reloadTimer -= Time.deltaTime;
        UpdateAimLine();

        // Fire button
        if (Input.GetKeyDown(KeyCode.Space))
        {
            LaunchMissile();
        }

        // Fine vs coarse aim
        if (Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift) || SerialScanner.FineAdjustmentSwitch)
        {
            aimSpeed = aimSpeedFine;
        } 
        else
        {
            aimSpeed = aimSpeedCoarse;
        }

        // Update aim line
        if (Input.GetKey(KeyCode.RightArrow))
        {
            aimRotationDegrees -= Time.deltaTime * aimSpeed;
        } else if (Input.GetKey(KeyCode.LeftArrow))
        {
            aimRotationDegrees += Time.deltaTime * aimSpeed;
        }

        // Update aim line from Serial Scanner
        aimRotationDegrees += EncoderMultiplier * ((float)SerialScanner.CummulativeEncoderCounts) * aimSpeed;
        SerialScanner.CummulativeEncoderCounts = 0;
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
}
